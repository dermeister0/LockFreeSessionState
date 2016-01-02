using System;
using System.Collections;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace Heavysoft.Web.SessionState
{
    /// <summary>
    /// The SessionItemEx class is used to store data for a particular session along with 
    /// an expiration date and time. SessionItem objects are added to the local Hashtable 
    /// in the OnReleaseRequestState event handler and retrieved from the local Hashtable 
    /// in the OnAcquireRequestState event handler. The ExpireCallback method is called 
    /// periodically by the local Timer to check for all expired SessionItem objects in the 
    /// local Hashtable and remove them. 
    /// </summary>
    internal class SessionItemEx : SessionItem
    {
        public DateTime Expires;
    }

    public class HashTableSessionStateModule : LockFreeSessionStateModule
    {
        private Timer timer;
        private int timerSeconds = 10;
        private static readonly Hashtable SessionItems = new Hashtable();
        private static readonly ReaderWriterLockSlim HashtableLock = new ReaderWriterLockSlim();

        protected override void OnInit()
        {
            base.OnInit();

            // Create a Timer to invoke the ExpireCallback method based on 
            // the pTimerSeconds value (e.g. every 10 seconds).

            timer = new Timer(ExpireCallback, null, 0, timerSeconds * 1000);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && timer != null)
            {
                timer.Dispose();
            }
        }
        
        protected override SessionItem AddNewSessionItem(string sessionId,
                                                         HttpStaticObjectsCollection staticObjects)
        {
            var sessionData = new SessionItemEx();

            sessionData.Items = new ThreadSafeSessionStateItemCollection();
            sessionData.StaticObjects = staticObjects;
            sessionData.Expires = DateTime.Now.AddMinutes(Timeout);

            try
            {
                HashtableLock.EnterWriteLock();
                SessionItems[sessionId] = sessionData;
            }
            finally
            {
                HashtableLock.ExitWriteLock();
            }

            return sessionData;
        }

        protected override SessionItem GetSessionItem(string sessionId)
        {
            SessionItemEx sessionData;

            try
            {
                HashtableLock.EnterReadLock();
                sessionData = (SessionItemEx)SessionItems[sessionId];

                if (sessionData != null)
                    sessionData.Expires = DateTime.Now.AddMinutes(Timeout);
            }
            finally
            {
                HashtableLock.ExitReadLock();
            }

            return sessionData;
        }

        protected override void SaveSessionItem(string sessionId, IHttpSessionState state)
        {
            // This method is not required, because the hash table implementation stores data inside app domain.
        }

        /// <summary>
        /// Called periodically by the Timer created in the Init method to check for  
        /// expired sessions and remove expired data. 
        /// </summary>
        /// <param name="state"></param>
        void ExpireCallback(object state)
        {
            try
            {
                HashtableLock.EnterWriteLock();

                RemoveExpiredSessionData();

            }
            finally
            {
                HashtableLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Recursivly remove expired session data from session collection. 
        /// </summary>
        private void RemoveExpiredSessionData()
        {
            foreach (DictionaryEntry entry in SessionItems)
            {
                SessionItemEx item = (SessionItemEx)entry.Value;

                if (DateTime.Compare(item.Expires, DateTime.Now) <= 0)
                {
                    var sessionId = entry.Key.ToString();
                    SessionItems.Remove(entry.Key);

                    HttpSessionStateContainer stateProvider =
                      new HttpSessionStateContainer(sessionId,
                                                   item.Items,
                                                   item.StaticObjects,
                                                   Timeout,
                                                   false,
                                                   CookieMode,
                                                   SessionStateMode.Custom,
                                                   false);

                    SessionStateUtility.RaiseSessionEnd(stateProvider, this, EventArgs.Empty);
                    RemoveExpiredSessionData();
                    break;
                }
            }

        }

        protected override void RemoveSessionItem(string sessionId)
        {
            try
            {
                HashtableLock.EnterWriteLock();
                SessionItems.Remove(sessionId);
            }
            finally
            {
                HashtableLock.ExitWriteLock();
            }
        }
    }
}
