using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.SessionState;
using Xunit;

namespace Heavysoft.LockFreeSessionState.Test
{
    public abstract class EnumeratorTest
    {
        protected ISessionStateItemCollection SessionState { get; }

        public EnumeratorTest(ISessionStateItemCollection sessionState)
        {
            SessionState = sessionState;
            SessionState["KeyA"] = "a";
            SessionState["B"] = "b";
            SessionState["C"] = "c";
        }

        [Fact]
        public void TestBasic()
        {
            var result = new List<object>();
            foreach (var obj in SessionState)
            {
                result.Add(obj);
            }

            Assert.Equal(3, SessionState.Count);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void TestAsync()
        {
            var result = new List<object>();
            var task = Task.Run(() =>
            {
                foreach (var obj in SessionState)
                {
                    result.Add(obj);
                }
            });

            task.Wait();

            Assert.Equal(3, SessionState.Count);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void TestAsync2()
        {
            var result = new List<object>();

            Action action = () =>
            {
                foreach (var obj in SessionState)
                {
                    result.Add(obj);
                }
            };

            var task1 = Task.Run(action);
            var task2 = Task.Run(action);

            Task.WaitAll(new[] { task1, task2 }, -1);

            Assert.Equal(3, SessionState.Count);
            Assert.Equal(6, result.Count);
        }

        [Fact]
        public void TestModifyWithExistingEnumerator1()
        {
            var enumerator = SessionState.GetEnumerator();
            SessionState["D"] = "d";
            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
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

        [Fact]
        public void TestModifyWithExistingEnumerator3()
        {
            var enumerator = SessionState.GetEnumerator();
            enumerator.MoveNext();
            enumerator.MoveNext();
            enumerator.MoveNext();
            SessionState.RemoveAt(2);
            object obj;
            Assert.Throws<InvalidOperationException>(() => obj = enumerator.Current);
        }
    }
}
