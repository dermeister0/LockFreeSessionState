using System.Web.SessionState;

namespace Heavysoft.LockFreeSessionState.Test
{
    public class StandardGeneralTest : GeneralTest
    {
        public StandardGeneralTest() : base(new SessionStateItemCollection())
        {
        }
    }
}
