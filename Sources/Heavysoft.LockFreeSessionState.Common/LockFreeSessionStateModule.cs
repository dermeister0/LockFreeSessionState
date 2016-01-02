using System;
using System.Web;
using System.Web.SessionState;
using System.Web.Configuration;
using System.Configuration;

namespace Heavysoft.Web.SessionState
{
    /// <summary>
    /// The SessionItem class is used to store data for a particular session.
    /// </summary>
    [Serializable]
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
        protected int Timeout;
        protected HttpCookieMode CookieMode = HttpCookieMode.UseCookies;

        private bool initialized;
        private ISessionIDManager sessionIdManager;
        private SessionStateSection config;
        private static readonly object LockObject = new object();

        /// <summary>
        /// IHttpModule.Init  
        /// </summary>
        /// <param name="app"></param>
        public void Init(HttpApplication app)
        {
            // Add event handlers.
            app.AcquireRequestState += OnAcquireRequestState;
            app.ReleaseRequestState += OnReleaseRequestState;

            // Create a SessionIDManager.
            sessionIdManager = new SessionIDManager();
            sessionIdManager.Initialize();

            // If not already initialized, initialize timer and configuration. 
            if (!initialized)
            {
                lock (LockObject)
                {
                    if (!initialized)
                    {
                        OnInit();

                        // Get the configuration section and set timeout and CookieMode values.
                        Configuration cfg =
                          WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                        config = (SessionStateSection)cfg.GetSection("system.web/sessionState");

                        Timeout = (int)config.Timeout.TotalMinutes;
                        CookieMode = config.Cookieless;

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
            SessionItem sessionData = null;
            bool supportSessionIdReissue;

            sessionIdManager.InitializeRequest(context, false, out supportSessionIdReissue);
            var sessionId = sessionIdManager.GetSessionID(context);


            if (sessionId != null)
            {
                sessionData = GetSessionItem(sessionId);
            }
            else
            {
                bool redirected, cookieAdded;

                sessionId = sessionIdManager.CreateSessionID(context);
                sessionIdManager.SaveSessionID(context, sessionId, out redirected, out cookieAdded);

                if (redirected)
                    return;
            }

            if (sessionData == null)
            {
                // Identify the session as a new session state instance. Create a new SessionItem 
                // and add it to the local Hashtable.

                isNew = true;

                sessionData = AddNewSessionItem(sessionId,
                                                SessionStateUtility.GetSessionStaticObjects(context));
            }

            // Add the session data to the current HttpContext.
            SessionStateUtility.AddHttpSessionStateToContext(context,
                             new HttpSessionStateContainer(sessionId,
                                                          sessionData.Items,
                                                          sessionData.StaticObjects,
                                                          Timeout,
                                                          isNew,
                                                          CookieMode,
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

            // Read the session state from the context
            HttpSessionStateContainer stateProvider = (HttpSessionStateContainer)(SessionStateUtility.GetHttpSessionStateFromContext(context));

            var sessionId = sessionIdManager.GetSessionID(context);

            // If Session.Abandon() was called, remove the session data from the local Hashtable 
            // and execute the Session_OnEnd event from the Global.asax file. 
            if (stateProvider.IsAbandoned)
            {
                RemoveSessionItem(sessionId);

                SessionStateUtility.RaiseSessionEnd(stateProvider, this, EventArgs.Empty);
            }
            else
            {
                // Save session data.
                SaveSessionItem(sessionId, stateProvider);
            }

            SessionStateUtility.RemoveHttpSessionStateFromContext(context);
        }

        protected virtual void OnInit()
        {
        }

        protected abstract SessionItem AddNewSessionItem(string sessionId,
                                                         HttpStaticObjectsCollection staticObjects);

        protected abstract SessionItem GetSessionItem(string sessionId);

        protected abstract void SaveSessionItem(string sessionId, IHttpSessionState state);

        protected abstract void RemoveSessionItem(string sessionId);
    }
}
