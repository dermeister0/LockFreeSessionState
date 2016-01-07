using Heavysoft.Web.SessionState;

namespace Heavysoft.LockFreeSessionState.Test
{
    public class HashMapEnumeratorTest : EnumeratorTest
    {
        public HashMapEnumeratorTest() : base(new ThreadSafeSessionStateItemCollection())
        {
        }
    }
}