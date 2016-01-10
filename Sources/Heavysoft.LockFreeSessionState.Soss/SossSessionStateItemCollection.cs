using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.SessionState;
using Heavysoft.Web.SessionState.Collections;
using Soss.Client;

namespace Heavysoft.Web.SessionState
{
    internal class SossSessionStateItemCollection : ISessionStateItemCollection, IVersioned
    {
        private const int KeyPrefixIndex = 0;

        private readonly string keyPrefix;
        private readonly string countIndex;
        private readonly string versionIndex;
        private readonly CreatePolicy createPolicy;
        private readonly CreatePolicy infinitePolicy;
        private readonly NamedCache namedCache;
        private readonly IndexValue keyPrefixIndexValue;

        public SossSessionStateItemCollection(string keyPrefix, int timeout)
        {
            this.keyPrefix = keyPrefix + "_";
            countIndex = keyPrefix + ".Count";
            versionIndex = keyPrefix + ".Version";
            createPolicy = new CreatePolicy(TimeSpan.FromMinutes(timeout));
            infinitePolicy = new CreatePolicy(TimeSpan.Zero);

            namedCache = CacheFactory.GetCache("SossSessionState");
            keyPrefixIndexValue = new IndexValue(keyPrefix);

            SyncRoot = new object();
            IsSynchronized = true;
        }

        public IEnumerator GetEnumerator()
        {
            return new VersionedEnumerator(GetKeys().GetEnumerator(), this);
        }

        public void CopyTo(Array array, int index)
        {
            GetKeys().Cast<string>().ToArray().CopyTo(array, index);            
        }

        public int Count => GetCount();
        public object SyncRoot { get; }
        public bool IsSynchronized { get; }
        public void Remove(string name)
        {
            namedCache.Remove(keyPrefix + name);
            IncrementCount(-1);
            IncrementVersion();
        }

        public void RemoveAt(int index)
        {
            var keys = GetKeys();
            if (index < 0 || index >= keys.Count)
                throw new IndexOutOfRangeException("index");

            Remove(keys[index]);
        }

        public void Clear()
        {
            // FIXME: Don't delete all keys in collection. Need to check key prefix.
            namedCache.Clear();
        }

        object ISessionStateItemCollection.this[string name]
        {
            get { return GetItem(name); }
            set { SetItem(name, value); }
        }

        object ISessionStateItemCollection.this[int index]
        {
            get { return GetItem(GetKeys()[index]); }
            set { SetItem(GetKeys()[index], value); }
        }

        public NameObjectCollectionBase.KeysCollection Keys => GetKeys();
        public bool Dirty { get; set; }

        int IVersioned.Version => GetVersion();

        private object GetItem(string name)
        {
            return namedCache.Retrieve(keyPrefix + name, false);
        }

        private void SetItem(string name, object value)
        {
            var fullKey = keyPrefix + name;

            if (namedCache.Contains(fullKey))
            {
                namedCache.Insert(fullKey, value, createPolicy, true, false);
            }
            else
            {
                namedCache.Insert(fullKey, value, createPolicy, true, true);
                IncrementCount(1);

                var metadata = new ObjectMetadata();
                metadata.IndexCollection[KeyPrefixIndex] = keyPrefixIndexValue;

                namedCache.SetMetadata(fullKey, metadata, true);
            }

            IncrementVersion();
        }

        private IEnumerable<CachedObjectId> GetCachedObjects()
        {
            var filter = new FilterCollection();
            filter[KeyPrefixIndex] = keyPrefixIndexValue;

            return namedCache.Query(filter).Cast<CachedObjectId>();
        }

        private NameObjectCollectionBase.KeysCollection GetKeys()
        {
            var dataKeys = new NameValueCollection();

            foreach (var cachedObject in GetCachedObjects())
            {
                dataKeys.Add(cachedObject.Key.GetKeyString().Substring(keyPrefix.Length), null);
            }

            return dataKeys.Keys;
        }

        private void IncrementInternalValue(string key, int increment)
        {
            var countValue = (int?) namedCache.Retrieve(key, true) ?? 0;
            namedCache.Insert(key, countValue + increment, infinitePolicy, true, false);
        }

        private int GetInternalValue(string key)
        {
            return (int?) namedCache.Retrieve(countIndex, false) ?? 0;
        }

        private void IncrementCount(int increment)
        {
            IncrementInternalValue(countIndex, increment);
        }

        private int GetCount()
        {
            return GetInternalValue(countIndex);
        }

        private void IncrementVersion()
        {
            IncrementInternalValue(versionIndex, 1);
        }

        private int GetVersion()
        {
            return GetInternalValue(versionIndex);
        }
    }
}
