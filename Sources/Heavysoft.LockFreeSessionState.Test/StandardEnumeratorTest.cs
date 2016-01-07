using System.Web.SessionState;

namespace Heavysoft.LockFreeSessionState.Test
{
    public class StandardEnumeratorTest : EnumeratorTest
    {
        public StandardEnumeratorTest() : base(new SessionStateItemCollection())
        {
        }
    }
}