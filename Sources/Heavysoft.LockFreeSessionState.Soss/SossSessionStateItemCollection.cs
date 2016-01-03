﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.SessionState;
using Soss.Client;

namespace Heavysoft.Web.SessionState
{
    internal class SossSessionStateItemCollection : ISessionStateItemCollection
    {
        private const int KeyPrefixIndex = 0;

        private readonly string keyPrefix;
        private readonly CreatePolicy createPolicy;
        private readonly NamedCache namedCache;
        private readonly IndexValue keyPrefixIndexValue;

        public SossSessionStateItemCollection(string keyPrefix, int timeout)
        {
            this.keyPrefix = keyPrefix + "_";
            createPolicy = new CreatePolicy(TimeSpan.FromMinutes(timeout));

            namedCache = CacheFactory.GetCache("SossSessionState");
            keyPrefixIndexValue = new IndexValue(keyPrefix);

            SyncRoot = new object();
            IsSynchronized = true;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count => GetCount();
        public object SyncRoot { get; }
        public bool IsSynchronized { get; }
        public void Remove(string name)
        {
            namedCache.Remove(keyPrefix + name);
            IncrementCount(-1);
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
            throw new NotImplementedException();
        }

        object ISessionStateItemCollection.this[string name]
        {
            get { return namedCache.Retrieve(keyPrefix + name, false); }
            set { SetItem(name, value); }
        }

        object ISessionStateItemCollection.this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public NameObjectCollectionBase.KeysCollection Keys => GetKeys();
        public bool Dirty { get; set; }

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
        }

        private NameObjectCollectionBase.KeysCollection GetKeys()
        {
            var dataKeys = new NameValueCollection();
            var filter = new FilterCollection();
            filter[KeyPrefixIndex] = keyPrefixIndexValue;

            foreach (CachedObjectId cachedObject in namedCache.Query(filter))
            {
                dataKeys.Add(cachedObject.Key.GetKeyString().Substring(keyPrefix.Length), null);
            }

            return dataKeys.Keys;
        }

        private void IncrementCount(int count)
        {
            // @@
        }

        private int GetCount()
        {
            return 0; // @@
        }
    }
}
