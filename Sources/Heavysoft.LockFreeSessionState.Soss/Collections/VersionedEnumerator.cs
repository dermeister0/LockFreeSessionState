using System;
using System.Collections;

namespace Heavysoft.Web.SessionState.Collections
{
    public class VersionedEnumerator : IEnumerator
    {
        private readonly IVersioned collection;
        private readonly IEnumerator inner;
        private readonly int version;

        public VersionedEnumerator(IEnumerator inner, IVersioned collection)
        {
            this.inner = inner;
            this.collection = collection;

            version = collection.Version;
        }

        private void CheckVersion()
        {
            if (version != collection.Version)
                throw new InvalidOperationException("Collection was modified.");
        }

        public bool MoveNext()
        {
            CheckVersion();
            return inner.MoveNext();
        }

        public void Reset()
        {
            CheckVersion();
            inner.Reset();
        }

        public object Current
        {
            get
            {
                CheckVersion();
                return inner.Current;
            }
        }
    }
}