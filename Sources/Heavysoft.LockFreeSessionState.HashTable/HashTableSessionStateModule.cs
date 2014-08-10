using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private Hashtable sessionItems = new Hashtable();
        private ReaderWriterLockSlim hashtableLock = new ReaderWriterLockSlim();

        protected override void OnInit()
        {
            base.OnInit();

            // Create a Timer to invoke the ExpireCallback method based on 
            // the pTimerSeconds value (e.g. every 10 seconds).

            timer = new Timer(new TimerCallback(this.ExpireCallback),
                               null,
                               0,
                               timerSeconds * 1000);
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
                                                         ISessionStateItemCollection items,
                                                         HttpStaticObjectsCollection staticObjects)
        {
            var sessionData = new SessionItemEx();

            sessionData.Items = items;
            sessionData.StaticObjects = staticObjects;
            sessionData.Expires = DateTime.Now.AddMinutes(timeout);

            try
            {
                hashtableLock.EnterWriteLock();
                sessionItems[sessionId] = sessionData;
            }
            finally
            {
                hashtableLock.ExitWriteLock();
            }

            return sessionData;
        }

        protected override SessionItem GetSessionItem(string sessionId)
        {
            SessionItemEx sessionData = null;

            try
            {
                hashtableLock.EnterReadLock();
                sessionData = (SessionItemEx)sessionItems[sessionId];

                if (sessionData != null)
                    sessionData.Expires = DateTime.Now.AddMinutes(timeout);
            }
            finally
            {
                hashtableLock.ExitReadLock();
            }

            return sessionData;
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
                hashtableLock.EnterWriteLock();

                this.RemoveExpiredSessionData();

            }
            finally
            {
                hashtableLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Recursivly remove expired session data from session collection. 
        /// </summary>
        private void RemoveExpiredSessionData()
        {
            string sessionID;

            foreach (DictionaryEntry entry in sessionItems)
            {
                SessionItemEx item = (SessionItemEx)entry.Value;

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

        protected override void RemoveSessionItem(string sessionId)
        {
            try
            {
                hashtableLock.EnterWriteLock();
                sessionItems.Remove(sessionId);
            }
            finally
            {
                hashtableLock.ExitWriteLock();
            }
        }
    }
}
