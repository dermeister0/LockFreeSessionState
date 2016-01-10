using Heavysoft.Web.SessionState;

namespace Heavysoft.LockFreeSessionState.Test
{
    public class SossEnumeratorTest : EnumeratorTest
    {
        public SossEnumeratorTest() : base(new SossSessionStateItemCollection("SessionTest", 20))
        {
        }
    }
}
