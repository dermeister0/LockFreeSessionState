using System;
using System.Collections;
using System.Threading;

namespace Heavysoft.Web.SessionState.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>http://www.codeproject.com/Articles/56575/Thread-safe-enumeration-in-C</remarks>
    public sealed class SafeEnumerator : IEnumerator, IDisposable
    {
        // this is the (thread-unsafe)
        // enumerator of the underlying collection
        private readonly IEnumerator inner;
        // this is the object we shall lock on. 
        private readonly ReaderWriterLockSlim lockSlim;

        public SafeEnumerator(IEnumerator inner, ReaderWriterLockSlim lockSlim)
        {
            this.inner = inner;
            this.lockSlim = lockSlim;
            // entering lock in constructor
            lockSlim.EnterReadLock();
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            // .. and exiting lock on Dispose()
            // This will be called when foreach loop finishes
            lockSlim.ExitReadLock();
        }

        #endregion

        #region Implementation of IEnumerator

        // we just delegate actual implementation
        // to the inner enumerator, that actually iterates
        // over some collection

        public bool MoveNext()
        {
            return inner.MoveNext();
        }

        public void Reset()
        {
            inner.Reset();
        }

        public object Current
        {
            get { return inner.Current; }
        }

        #endregion
    }
}
