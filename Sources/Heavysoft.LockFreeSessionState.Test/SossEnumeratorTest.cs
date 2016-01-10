using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Heavysoft.Web.SessionState;
using Xunit;

namespace Heavysoft.LockFreeSessionState.Test
{
    public class SossEnumeratorTest : EnumeratorTest
    {
        public SossEnumeratorTest() : base(new SossSessionStateItemCollection("SessionTest", 20))
        {
        }

        [Fact]
        public void TestModifyWithExistingEnumerator2()
        {
            var enumerator = SessionState.GetEnumerator();
            enumerator.MoveNext();
            SessionState["D"] = "d";

            Assert.Throws<InvalidOperationException>(() => { var obj = enumerator.Current; });
        }
    }
}
