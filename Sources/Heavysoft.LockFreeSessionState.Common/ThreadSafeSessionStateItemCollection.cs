using Heavysoft.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.SessionState;

namespace Heavysoft.Web.SessionState
{
    internal class ThreadSafeSessionStateItemCollection : ISessionStateItemCollection
    {
        private OrderedDictionary data = new OrderedDictionary();

        private ReaderWriterLockSlim dataLock = new ReaderWriterLockSlim();

        private object enumeratorLock = new object();

        public void Clear()
        {
            try
            {
                dataLock.EnterReadLock();
                data.Clear();
            }
            finally
            {
                dataLock.ExitReadLock();
            }
        }

        bool dirty;

        public bool Dirty
        {
            get
            {
                return dirty;
            }
            set
            {
                dirty = true;
            }
        }

        public System.Collections.Specialized.NameObjectCollectionBase.KeysCollection Keys
        {
            get { throw new NotImplementedException(); }
        }

        public void Remove(string name)
        {
            try
            {
                dataLock.EnterWriteLock();
                data.Remove(name);
            }
            finally
            {
                dataLock.ExitWriteLock();
            }
        }

        public void RemoveAt(int index)
        {
            try
            {
                dataLock.EnterWriteLock();
                data.RemoveAt(index);
            }
            finally
            {
                dataLock.ExitWriteLock();
            }
        }

        public object this[int index]
        {
            get
            {
                try
                {
                    dataLock.EnterReadLock();
                    return data[index];
                }
                finally
                {
                    dataLock.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    dataLock.EnterWriteLock();
                    data[index] = value;
                }
                finally
                {
                    dataLock.ExitWriteLock();
                }
            }
        }

        public object this[string name]
        {
            get
            {
                try
                {
                    dataLock.EnterReadLock();
                    return data[name];
                }
                finally
                {
                    dataLock.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    dataLock.EnterWriteLock();
                    data[name] = value;
                }
                finally
                {
                    dataLock.ExitWriteLock();
                }
            }
        }

        public void CopyTo(Array array, int index)
        {
            try
            {
                dataLock.EnterReadLock();
                data.CopyTo(array, index);
            }
            finally
            {
                dataLock.ExitReadLock();
            }
        }

        public int Count
        {
            get
            {
                try
                {
                    dataLock.EnterReadLock();
                    return data.Count;
                }
                finally
                {
                    dataLock.ExitReadLock();
                }
            }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { throw new InvalidOperationException(); }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            try
            {
                dataLock.EnterReadLock();
                return new SafeEnumerator(data.GetEnumerator(), enumeratorLock);
            }
            finally
            {
                dataLock.ExitReadLock();
            }
        }
    }
}
