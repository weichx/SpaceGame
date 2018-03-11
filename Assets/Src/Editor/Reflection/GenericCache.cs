using System;
using System.Collections;
using System.Collections.Generic;

namespace SpaceGame.Editor.Reflection {

    public class GenericCache {

        private Hashtable cacheRoot;

        public GenericCache() {
            this.cacheRoot = new Hashtable();
        }

        public U GetOrAddToCache<T, U>(string cacheName, T key, Func<U> callback) {
            Dictionary<T, U> cache = GetCache<T, U>(cacheName);
            U retn;
            if (cache.TryGetValue(key, out retn)) {
                return retn;
            }
            retn = callback();
            cache[key] = retn;
            return retn;
        }

        public bool GetItemFromCache<T, U>(string cacheName, T key, out U outVal) {
            Dictionary<T, U> cache = GetCache<T, U>(cacheName);
            U retn;
            if (cache.TryGetValue(key, out retn)) {
                outVal = retn;
                return true;
            }
            outVal = default(U);
            return false;
        }

        public U GetItemFromCache<T, U>(string cacheName, T key) {
            U retn;
            if (GetItemFromCache(cacheName, key, out retn)) {
                return retn;
            }
            return default(U);
        }

        public void AddItemToCache<T, U>(string cacheName, T key, U value) {
            Dictionary<T, U> cache = GetCache<T, U>(cacheName);
            cache[key] = value;
        }

        public Dictionary<T, U> GetCache<T, U>(string cacheName) {
            Dictionary<T, U> subCache = cacheRoot[cacheName] as Dictionary<T, U>;

            if (subCache == null) {
                subCache = new Dictionary<T, U>();
                cacheRoot[cacheName] = subCache;
            }
            return subCache;
        }

    }

}