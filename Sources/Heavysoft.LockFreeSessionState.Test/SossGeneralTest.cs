using Heavysoft.Web.SessionState;

namespace Heavysoft.LockFreeSessionState.Test
{
    public class SossGeneralTest : GeneralTest
    {
        public SossGeneralTest() : base(new SossSessionStateItemCollection("SessionTest", 20))
        {
        }
    }
}
