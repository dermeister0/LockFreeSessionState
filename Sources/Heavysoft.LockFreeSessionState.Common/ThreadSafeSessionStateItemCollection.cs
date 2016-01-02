using Heavysoft.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Web.SessionState;

namespace Heavysoft.Web.SessionState
{
    [Serializable]
    internal class ThreadSafeSessionStateItemCollection : ISessionStateItemCollection, IDeserializationCallback
    {
        /// <summary>
        /// Contains keys only. Values are not used.
        /// </summary>
        private NameValueCollection dataKeys = new NameValueCollection();

        /// <summary>
        /// Contains name-value pairs. Keys must be the same as in <see cref="dataKeys"/> collections.
        /// </summary>
        private Hashtable dataValues = new Hashtable();

        [NonSerialized]
        private ReaderWriterLockSlim dataLock = new ReaderWriterLockSlim();

        private object enumeratorLock = new object();

        public void Clear()
        {
            try
            {
                dataLock.EnterReadLock();
                dataKeys.Clear();
                dataValues.Clear();
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

        public NameObjectCollectionBase.KeysCollection Keys
        {
            get
            {
                try
                {
                    dataLock.EnterReadLock();
                    return dataKeys.Keys;
                }
                finally
                {
                    dataLock.ExitReadLock();
                }
            }
        }

        public void Remove(string name)
        {
            try
            {
                dataLock.EnterWriteLock();
                dataKeys.Remove(name);
                dataValues.Remove(name);
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

                string name = dataKeys.GetKey(index);
                dataKeys.Remove(name);
                dataValues.Remove(name);
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
                    return dataValues[dataKeys.GetKey(index)];
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
                    dataValues[dataKeys.GetKey(index)] = value;
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
                    return dataValues[name];
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
                    dataKeys.Set(name, null);
                    dataValues[name] = value;
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
                dataValues.CopyTo(array, index);
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
                    return dataKeys.Count;
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
                return new SafeEnumerator(dataValues.GetEnumerator(), enumeratorLock);
            }
            finally
            {
                dataLock.ExitReadLock();
            }
        }

        public void OnDeserialization(object sender)
        {
            dataLock = new ReaderWriterLockSlim();
        }
    }
}
