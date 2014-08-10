using Heavysoft.Web.SessionState;
using Soss.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace Heavysoft.Web.SessionState
{
    public class SossSessionStateModule : LockFreeSessionStateModule
    {
        NamedCache namedCache;

        CreatePolicy createPolicy;

        protected override void OnInit()
        {
            base.OnInit();

            namedCache = CacheFactory.GetCache("SossSessionState");
            createPolicy = new CreatePolicy(TimeSpan.FromMinutes(timeout));
        }

        protected override SessionItem AddNewSessionItem(string sessionId,
                                                         ISessionStateItemCollection items,
                                                         HttpStaticObjectsCollection staticObjects)
        {
            var sessionItem = new SessionItem();
            sessionItem.Items = items;
            sessionItem.StaticObjects = staticObjects;

            namedCache.Insert(sessionId, sessionItem, createPolicy, true, false);

            return sessionItem;
        }

        protected override SessionItem GetSessionItem(string sessionId)
        {
            throw new NotImplementedException();
        }

        protected override void RemoveSessionItem(string sessionId)
        {
            throw new NotImplementedException();
        }
    }
}
