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
            Initialize();
        }

        private void Initialize()
        {
            SessionState.Clear();
        }

        private void InsertThreeValues()
        {
            SessionState["KeyA"] = 1;
            SessionState["KeyB"] = 2;
            SessionState["KeyC"] = 3;
        }

        [Fact]
        public void TestCount()
        {
            Assert.Equal(0, SessionState.Count);

            InsertThreeValues();
            Assert.Equal(3, SessionState.Count);

            SessionState["KeyC"] = 0;
            Assert.Equal(3, SessionState.Count);

            SessionState.Remove("KeyB");
            Assert.Equal(2, SessionState.Count);

            SessionState.RemoveAt(1);
            Assert.Equal(1, SessionState.Count);
        }

        [Fact]
        public void TestCopyToArray()
        {
            InsertThreeValues();

            var result = new object[3];
            SessionState.CopyTo(result, 0);

            Assert.Equal("KeyA", result[0]);
            Assert.Equal("KeyB", result[1]);
            Assert.Equal("KeyC", result[2]);
        }
    }
}