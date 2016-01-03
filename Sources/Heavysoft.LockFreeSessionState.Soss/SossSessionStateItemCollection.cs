using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.SessionState;

namespace Heavysoft.Web.SessionState
{
    internal class SossSessionStateItemCollection : ISessionStateItemCollection
    {
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count { get; }
        public object SyncRoot { get; }
        public bool IsSynchronized { get; }
        public void Remove(string name)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        object ISessionStateItemCollection.this[string name]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        object ISessionStateItemCollection.this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public NameObjectCollectionBase.KeysCollection Keys { get; }
        public bool Dirty { get; set; }
    }
}
