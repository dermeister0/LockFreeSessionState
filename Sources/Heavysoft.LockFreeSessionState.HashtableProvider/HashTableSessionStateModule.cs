using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Heavysoft.Web.SessionState
{
    public class HashTableSessionStateModule : LockFreeSessionStateModule
    {
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
    }
}
