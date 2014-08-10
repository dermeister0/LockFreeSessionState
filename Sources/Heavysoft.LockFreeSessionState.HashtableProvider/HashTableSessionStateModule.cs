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
    public class HashTableSessionStateModule : LockFreeSessionStateModule
    {
        private Timer timer;
        private int timerSeconds = 10;
        private Hashtable sessionItems = new Hashtable();
        private ReaderWriterLock hashtableLock = new ReaderWriterLock();

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
        
        protected override void AddNewSessionItem(string sessionId, SessionStateItemCollection items, HttpStaticObjectsCollection staticObjects)
        {
            var sessionData = new SessionItem();

            sessionData.Items = items;
            sessionData.StaticObjects = staticObjects;
            sessionData.Expires = DateTime.Now.AddMinutes(timeout);

            try
            {
                hashtableLock.AcquireWriterLock(Int32.MaxValue);
                sessionItems[sessionId] = sessionData;
            }
            finally
            {
                hashtableLock.ReleaseWriterLock();
            }
        }

        protected override SessionItem GetSessionItem(string sessionId)
        {
            SessionItem sessionData = null;

            try
            {
                hashtableLock.AcquireReaderLock(Int32.MaxValue);
                sessionData = (SessionItem)sessionItems[sessionId];

                if (sessionData != null)
                    sessionData.Expires = DateTime.Now.AddMinutes(timeout);
            }
            finally
            {
                hashtableLock.ReleaseReaderLock();
            }

            return sessionData;
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

        protected override void RemoveSessionItem(string sessionId)
        {
            try
            {
                hashtableLock.AcquireWriterLock(Int32.MaxValue);
                sessionItems.Remove(sessionId);
            }
            finally
            {
                hashtableLock.ReleaseWriterLock();
            }
        }
    }
}
