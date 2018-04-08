using System;
using System.Collections.Generic;
using UnityEngine;
using Weichx.Util;

namespace SpaceGame {

    public class AssetMap<T> : AssetMap where T : Asset {

        [SerializeField] private ListX<T> list;
        private Dictionary<int, T> map;
        private Func<T, object> groupingFn;
        
        public AssetMap() {
            this.list = new ListX<T>();
            this.map = new Dictionary<int, T>();
        }

        public T Add(T asset) {
            list = list ?? new ListX<T>();
            map = map ?? new Dictionary<int, T>();
            list.Add(asset);
            map[asset.id] = asset;
            return asset;
        }

        public override void Remove(object asset) {
            Remove(asset as T);
        }

        public T Remove(T asset) {
            list.Remove(asset);
            map.Remove(asset.id);
            return asset;
        }

        public void Clear() {
            list.Clear();
            map.Clear();
        }

        public T Get(int id) {
            EnsureMapEqualsList();
            return map.Get(id);
        }

        public IReadonlyListX<T> GetList() {
            return list ?? (list = new ListX<T>());
        }

        private void EnsureMapEqualsList() {
            if (list.Count == map.Count) return;
            map.Clear();
            T[] rawList = list.RawArray;
            for (int i = 0; i < list.Count; i++) {
                map[rawList[i].id] = rawList[i];
            }
        }

    }

    public abstract class AssetMap {

        public abstract void Remove(object aset);

    }

}