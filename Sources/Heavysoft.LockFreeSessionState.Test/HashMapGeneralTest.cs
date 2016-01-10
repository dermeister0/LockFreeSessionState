using Heavysoft.Web.SessionState;

namespace Heavysoft.LockFreeSessionState.Test
{
    public class HashMapGeneralTest : GeneralTest
    {
        public HashMapGeneralTest() : base(new ThreadSafeSessionStateItemCollection())
        {
        }
    }
}
