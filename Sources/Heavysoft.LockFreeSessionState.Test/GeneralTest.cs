using System.Web.SessionState;
using Xunit;

namespace Heavysoft.LockFreeSessionState.Test
{
    public abstract class GeneralTest
    {
        protected ISessionStateItemCollection SessionState { get; }

        public GeneralTest(ISessionStateItemCollection sessionState)
        {
            SessionState = sessionState;
        }

        [Fact]
        public void TestCount()
        {
            SessionState.Clear();
            Assert.Equal(0, SessionState.Count);

            SessionState["KeyA"] = 1;
            SessionState["KeyB"] = 2;
            SessionState["KeyC"] = 3;
            Assert.Equal(3, SessionState.Count);

            SessionState["KeyC"] = 0;
            Assert.Equal(3, SessionState.Count);

            SessionState.Remove("KeyB");
            Assert.Equal(2, SessionState.Count);

            SessionState.RemoveAt(1);
            Assert.Equal(1, SessionState.Count);
        }
    }
}