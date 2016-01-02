using Soss.Client;
using System;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace Heavysoft.Web.SessionState
{
    [Serializable]
    internal class SessionItemEx
    {
        readonly ISessionStateItemCollection items;

        readonly byte[] staticObjects;

        public SessionItemEx(SessionItem data)
        {
            items = data.Items;

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    data.StaticObjects.Serialize(writer);
                    staticObjects = stream.ToArray();
                }
            }
        }

        public SessionItem GetSessionItem()
        {
            var result = new SessionItem();
            result.Items = items;

            if (staticObjects != null)
            {
                using (var stream = new MemoryStream())
                {
                    stream.Write(staticObjects, 0, staticObjects.Length);
                    stream.Seek(0, SeekOrigin.Begin);

                    using (var reader = new BinaryReader(stream))
                    {
                        result.StaticObjects = HttpStaticObjectsCollection.Deserialize(reader);
                    }
                }
            }

            return result;
        }
    }

    public class SossSessionStateModule : LockFreeSessionStateModule
    {
        NamedCache namedCache;

        CreatePolicy createPolicy;

        protected override void OnInit()
        {
            base.OnInit();

            namedCache = CacheFactory.GetCache("SossSessionState");
            createPolicy = new CreatePolicy(TimeSpan.FromMinutes(Timeout));
        }

        protected override SessionItem AddNewSessionItem(string sessionId,
                                                         ISessionStateItemCollection items,
                                                         HttpStaticObjectsCollection staticObjects)
        {
            var sessionItem = new SessionItem();
            sessionItem.Items = items;
            sessionItem.StaticObjects = staticObjects;

            var data = new SessionItemEx(sessionItem);

            namedCache.Insert(sessionId, data, createPolicy, true, false);

            return sessionItem;
        }

        protected override SessionItem GetSessionItem(string sessionId)
        {
            SessionItem result = null;
            var data = namedCache.Retrieve(sessionId, false) as SessionItemEx;

            if (data != null)
                result = data.GetSessionItem();

            return result;
        }

        protected override void RemoveSessionItem(string sessionId)
        {
            namedCache.Remove(sessionId);
        }
    }
}
