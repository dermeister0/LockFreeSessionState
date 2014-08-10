using System;
using System.Web;
using System.Web.SessionState;
using System.Collections;
using System.Threading;
using System.Web.Configuration;
using System.Configuration;

namespace Heavysoft.Web.SessionState
{
    /// <summary>
    /// The SessionItem class is used to store data for a particular session.
    /// </summary>
    public class SessionItem
    {
        public ISessionStateItemCollection Items;
        public HttpStaticObjectsCollection StaticObjects;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Based on example from MSDN:
    /// http://msdn.microsoft.com/en-us/library/system.web.sessionstate.sessionstateutility.aspx
    /// </remarks>
    public abstract class LockFreeSessionStateModule : IHttpModule, IDisposable
    {       
        protected int timeout;
        protected HttpCookieMode cookieMode = HttpCookieMode.UseCookies;

        private bool initialized = false;
        private ISessionIDManager sessionIDManager;
        private SessionStateSection config;
        private static object lockObject = new object();

        /// <summary>
        /// IHttpModule.Init  
        /// </summary>
        /// <param name="app"></param>
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
                lock (lockObject)
                {
                    if (!initialized)
                    {
                        OnInit();

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

        /// <summary>
        /// IHttpModule.Dispose 
        /// </summary>
         public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Event handler for HttpApplication.AcquireRequestState 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnAcquireRequestState(object source, EventArgs args)
        {
            HttpApplication app = (HttpApplication)source;
            HttpContext context = app.Context;
            bool isNew = false;
            string sessionId;
            SessionItem sessionData = null;
            bool supportSessionIDReissue = true;

            sessionIDManager.InitializeRequest(context, false, out supportSessionIDReissue);
            sessionId = sessionIDManager.GetSessionID(context);


            if (sessionId != null)
            {
                sessionData = GetSessionItem(sessionId);
            }
            else
            {
                bool redirected, cookieAdded;

                sessionId = sessionIDManager.CreateSessionID(context);
                sessionIDManager.SaveSessionID(context, sessionId, out redirected, out cookieAdded);

                if (redirected)
                    return;
            }

            if (sessionData == null)
            {
                // Identify the session as a new session state instance. Create a new SessionItem 
                // and add it to the local Hashtable.

                isNew = true;

                sessionData = AddNewSessionItem(sessionId,
                                                new ThreadSafeSessionStateItemCollection(),
                                                SessionStateUtility.GetSessionStaticObjects(context));
            }

            // Add the session data to the current HttpContext.
            SessionStateUtility.AddHttpSessionStateToContext(context,
                             new HttpSessionStateContainer(sessionId,
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

        /// <summary>
        /// Event for Session_OnStart event in the Global.asax file. 
        /// </summary>
        public event EventHandler Start;

        /// <summary>
        /// Event handler for HttpApplication.ReleaseRequestState 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
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
                sessionID = sessionIDManager.GetSessionID(context);
                RemoveSessionItem(sessionID);

                SessionStateUtility.RaiseSessionEnd(stateProvider, this, EventArgs.Empty);
            }

            SessionStateUtility.RemoveHttpSessionStateFromContext(context);
        }

        protected virtual void OnInit()
        {
        }

        protected abstract SessionItem AddNewSessionItem(string sessionId,
                                                         ISessionStateItemCollection items,
                                                         HttpStaticObjectsCollection staticObjects);

        protected abstract SessionItem GetSessionItem(string sessionId);

        protected abstract void RemoveSessionItem(string sessionId);
    }
}
