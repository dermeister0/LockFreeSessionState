using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.SessionState;
using Heavysoft.Web.SessionState;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Heavysoft.LockFreeSessionState.Test
{
    [TestClass]
    public class SafeEnumeratorTest
    {
        private ISessionStateItemCollection sessionState;

        [TestInitialize]
        public void Initialize()
        {
            sessionState = new ThreadSafeSessionStateItemCollection();
            sessionState["A"] = "a";
            sessionState["B"] = "b";
            sessionState["C"] = "c";
        }

        [TestMethod]
        public void TestBasic()
        {
            var result = new List<object>();
            foreach (var obj in sessionState)
            {
                result.Add(obj);
            }

            Assert.AreEqual(3, sessionState.Count);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void TestAsync()
        {
            var result = new List<object>();
            var task = Task.Run(() =>
            {
                foreach (var obj in sessionState)
                {
                    result.Add(obj);
                }
            });

            task.Wait();

            Assert.AreEqual(3, sessionState.Count);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void TestAsync2()
        {
            var result = new List<object>();

            Action action = () =>
            {
                foreach (var obj in sessionState)
                {
                    result.Add(obj);
                }
            };

            var task1 = Task.Run(action);
            var task2 = Task.Run(action);

            Task.WaitAll(new[] { task1, task2 }, -1);

            Assert.AreEqual(3, sessionState.Count);
            Assert.AreEqual(6, result.Count);
        }

        [TestMethod]
        public void TestModifyWithExistingEnumerator()
        {
            var enumerator = sessionState.GetEnumerator();
            sessionState["D"] = "d";
        }

        [TestMethod]
        public void TestThreadSafety()
        {
            var result = new List<object>();

            var task1 = Task.Run(async () =>
            {
                sessionState["D"] = "d";
                await Task.Delay(500);
                sessionState["E"] = "e";
                await Task.Delay(500);
                sessionState["F"] = "f";
            });

            var task2 = Task.Run(async () =>
            {
                await Task.Delay(250);
                foreach (var obj in sessionState)
                {
                    result.Add(obj);
                    Thread.Sleep(1000); // Don't use Task.Delay here, because it's not safe.
                }
            });
            
            Task.WaitAll(new Task[] { task1, task2 }, -1);

            Assert.AreEqual(4, result.Count);
        }
    }
}
