using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.SessionState;
using Xunit;

namespace Heavysoft.LockFreeSessionState.Test
{
    public class StandardEnumeratorTest : EnumeratorTest
    {
        public StandardEnumeratorTest() : base(new SessionStateItemCollection())
        {
        }

        [Fact]
        public void TestThreadSafety()
        {
            var task1 = Task.Run(async () =>
            {
                SessionState["D"] = "d";
                await Task.Delay(500);
                SessionState["E"] = "e";
                await Task.Delay(500);
                SessionState["F"] = "f";
            });

            var task2 = Task.Run(async () =>
            {
                await Task.Delay(250);
                Assert.Throws<InvalidOperationException>(() =>
                {
                    foreach (var obj in SessionState)
                    {
                        Thread.Sleep(1000); // Don't use Task.Delay here, because it's not safe.
                    }
                });
            });

            Task.WaitAll(new Task[] { task1, task2 }, -1);
        }

        [Fact]
        public void TestModifyWithExistingEnumerator2()
        {
            var enumerator = SessionState.GetEnumerator();
            enumerator.MoveNext();
            SessionState["D"] = "d";

            var obj = enumerator.Current;
            Assert.Equal("KeyA", obj);
        }
    }
}