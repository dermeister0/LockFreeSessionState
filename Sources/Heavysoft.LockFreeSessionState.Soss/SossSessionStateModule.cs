using Heavysoft.Web.SessionState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heavysoft.Web.SessionState
{
    public class SossSessionStateModule : LockFreeSessionStateModule
    {
        protected override SessionItem AddNewSessionItem(string sessionId, System.Web.SessionState.ISessionStateItemCollection items, System.Web.HttpStaticObjectsCollection staticObjects)
        {
            throw new NotImplementedException();
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
