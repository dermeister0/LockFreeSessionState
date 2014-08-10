using System;
using System.Web;
using System.Web.SessionState;
using System.Collections;
using System.Threading;
using System.Web.Configuration;
using System.Configuration;

namespace Heavysoft.Web.SessionState
{
    public sealed class LockFreeSessionStateModule : IHttpModule, IDisposable
    {
        private Hashtable sessionItems = new Hashtable();
        private Timer timer;
        private int timerSeconds = 10;
        private bool initialized = false;
        private int timeout;
        private HttpCookieMode cookieMode = HttpCookieMode.UseCookies;
        private ReaderWriterLock hashtableLock = new ReaderWriterLock();
        private ISessionIDManager sessionIDManager;
        private SessionStateSection config;


        // The SessionItem class is used to store data for a particular session along with 
        // an expiration date and time. SessionItem objects are added to the local Hashtable 
        // in the OnReleaseRequestState event handler and retrieved from the local Hashtable 
        // in the OnAcquireRequestState event handler. The ExpireCallback method is called 
        // periodically by the local Timer to check for all expired SessionItem objects in the 
        // local Hashtable and remove them. 

        private class SessionItem
        {
            internal SessionStateItemCollection Items;
            internal HttpStaticObjectsCollection StaticObjects;
            internal DateTime Expires;
        }


        // 
        // IHttpModule.Init 
        // 

        public void Init(HttpApplication app)
        {
            // Add event handlers.
            app.AcquireRequestState += new EventHandler(this.OnAcquireRequestState);
            app.ReleaseRequestState += new EventHandler(this.OnReleaseRequestState);

            // Create a SessionIDManager.
            sessionIDManager = new SessionIDManager();
            sessionIDManager.Initialize();

            // If not already initialized, initialize timer and configuration. 
            if (!initialized)
            {
                lock (typeof(LockFreeSessionStateModule))
                {
                    if (!initialized)
                    {
                        // Create a Timer to invoke the ExpireCallback method based on 
                        // the pTimerSeconds value (e.g. every 10 seconds).

                        timer = new Timer(new TimerCallback(this.ExpireCallback),
                                           null,
                                           0,
                                           timerSeconds * 1000);

                        // Get the configuration section and set timeout and CookieMode values.
                        Configuration cfg =
                          WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                        config = (SessionStateSection)cfg.GetSection("system.web/sessionState");

                        timeout = (int)config.Timeout.TotalMinutes;
                        cookieMode = config.Cookieless;

                        initialized = true;
                    }
                }
            }
        }



        // 
        // IHttpModule.Dispose 
        // 

        public void Dispose()
        {
            if (timer != null)
            {
                this.timer.Dispose();
                ((IDisposable)timer).Dispose();
            }
        }


        // 
        // Called periodically by the Timer created in the Init method to check for  
        // expired sessions and remove expired data. 
        // 

        void ExpireCallback(object state)
        {
            try
            {
                hashtableLock.AcquireWriterLock(Int32.MaxValue);

                this.RemoveExpiredSessionData();

            }
            finally
            {
                hashtableLock.ReleaseWriterLock();
            }
        }


        // 
        // Recursivly remove expired session data from session collection. 
        // 
        private void RemoveExpiredSessionData()
        {
            string sessionID;

            foreach (DictionaryEntry entry in sessionItems)
            {
                SessionItem item = (SessionItem)entry.Value;

                if (DateTime.Compare(item.Expires, DateTime.Now) <= 0)
                {
                    sessionID = entry.Key.ToString();
                    sessionItems.Remove(entry.Key);

                    HttpSessionStateContainer stateProvider =
                      new HttpSessionStateContainer(sessionID,
                                                   item.Items,
                                                   item.StaticObjects,
                                                   timeout,
                                                   false,
                                                   cookieMode,
                                                   SessionStateMode.Custom,
                                                   false);

                    SessionStateUtility.RaiseSessionEnd(stateProvider, this, EventArgs.Empty);
                    this.RemoveExpiredSessionData();
                    break;
                }
            }

        }


        // 
        // Event handler for HttpApplication.AcquireRequestState 
        // 

        private void OnAcquireRequestState(object source, EventArgs args)
        {
            HttpApplication app = (HttpApplication)source;
            HttpContext context = app.Context;
            bool isNew = false;
            string sessionID;
            SessionItem sessionData = null;
            bool supportSessionIDReissue = true;

            sessionIDManager.InitializeRequest(context, false, out supportSessionIDReissue);
            sessionID = sessionIDManager.GetSessionID(context);


            if (sessionID != null)
            {
                try
                {
                    hashtableLock.AcquireReaderLock(Int32.MaxValue);
                    sessionData = (SessionItem)sessionItems[sessionID];

                    if (sessionData != null)
                        sessionData.Expires = DateTime.Now.AddMinutes(timeout);
                }
                finally
                {
                    hashtableLock.ReleaseReaderLock();
                }
            }
            else
            {
                bool redirected, cookieAdded;

                sessionID = sessionIDManager.CreateSessionID(context);
                sessionIDManager.SaveSessionID(context, sessionID, out redirected, out cookieAdded);

                if (redirected)
                    return;
            }

            if (sessionData == null)
            {
                // Identify the session as a new session state instance. Create a new SessionItem 
                // and add it to the local Hashtable.

                isNew = true;

                sessionData = new SessionItem();

                sessionData.Items = new SessionStateItemCollection();
                sessionData.StaticObjects = SessionStateUtility.GetSessionStaticObjects(context);
                sessionData.Expires = DateTime.Now.AddMinutes(timeout);

                try
                {
                    hashtableLock.AcquireWriterLock(Int32.MaxValue);
                    sessionItems[sessionID] = sessionData;
                }
                finally
                {
                    hashtableLock.ReleaseWriterLock();
                }
            }

            // Add the session data to the current HttpContext.
            SessionStateUtility.AddHttpSessionStateToContext(context,
                             new HttpSessionStateContainer(sessionID,
                                                          sessionData.Items,
                                                          sessionData.StaticObjects,
                                                          timeout,
                                                          isNew,
                                                          cookieMode,
                                                          SessionStateMode.Custom,
                                                          false));

            // Execute the Session_OnStart event for a new session. 
            if (isNew && Start != null)
            {
                Start(this, EventArgs.Empty);
            }
        }

        // 
        // Event for Session_OnStart event in the Global.asax file. 
        // 

        public event EventHandler Start;


        // 
        // Event handler for HttpApplication.ReleaseRequestState 
        // 

        private void OnReleaseRequestState(object source, EventArgs args)
        {
            HttpApplication app = (HttpApplication)source;
            HttpContext context = app.Context;
            string sessionID;

            // Read the session state from the context
            HttpSessionStateContainer stateProvider =
              (HttpSessionStateContainer)(SessionStateUtility.GetHttpSessionStateFromContext(context));

            // If Session.Abandon() was called, remove the session data from the local Hashtable 
            // and execute the Session_OnEnd event from the Global.asax file. 
            if (stateProvider.IsAbandoned)
            {
                try
                {
                    hashtableLock.AcquireWriterLock(Int32.MaxValue);

                    sessionID = sessionIDManager.GetSessionID(context);
                    sessionItems.Remove(sessionID);
                }
                finally
                {
                    hashtableLock.ReleaseWriterLock();
                }

                SessionStateUtility.RaiseSessionEnd(stateProvider, this, EventArgs.Empty);
            }

            SessionStateUtility.RemoveHttpSessionStateFromContext(context);
        }
    }
}

