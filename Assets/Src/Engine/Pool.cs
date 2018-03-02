using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace SpaceGame {

//    public class P {
//        private Dictionary<Type, Stack<>>
//        public T Spawn<T>() where T : class, new() {
//            return new T();
//        }
//
//        public void Despawn<T>(T despawned) {
//            
//        }
//
//    }
    
    public class Pool<T> where T : class, new() {

        private readonly int batchSize;
        private readonly Stack<T> activeList;

        public Pool(int initialSize, int batchSize = 20) {
            this.batchSize = Mathf.Max(1, batchSize);
            initialSize = Mathf.Max(1, initialSize);
            activeList = new Stack<T>(initialSize);
            Allocate(initialSize);
        }

        private void Allocate(int count) {
            for (int i = 0; i < count; i++) {
                activeList.Push(new T());
            }
        }

        public T Spawn() {
            if (activeList.Count > 0) {
                Allocate(batchSize);
            }
            return activeList.Pop();
        }

        public void Despawn(T obj) {
            activeList.Push(obj);
        }

    }

    public interface IPoolable<T, U> {

        T Spawn(U u);
        
    }

    public class ConstructedPool<T, U> where T : class, IPoolable<T, U>, new() {

        private readonly int batchSize;
        private readonly Stack<T> activeList;

        public ConstructedPool(int initialSize, int batchSize = 20) {
            this.batchSize = Mathf.Max(1, batchSize);
            initialSize = Mathf.Max(1, initialSize);
            activeList = new Stack<T>(initialSize);
            Allocate(initialSize);
        }

        private void Allocate(int count) {
            for (int i = 0; i < count; i++) {
                activeList.Push(new T());
            }
        }

        public T Spawn(U arg) {
            if (activeList.Count > 0) {
                Allocate(batchSize);
            }

            T retn = activeList.Pop();
            retn.Spawn(arg);
            return retn;
        }

        public void Despawn(T obj) {
            activeList.Push(obj);
        }

    }

    public class GameObjectPool {

        private readonly int batchSize;
        private readonly GameObject prefab;
        private readonly List<GameObject> activeList;

        public GameObjectPool(GameObject prefab, int initialSize, int batchSize = 20) {
            this.prefab = prefab;
            this.batchSize = Mathf.Max(1, batchSize);
            initialSize = Mathf.Max(1, initialSize);
            activeList = new List<GameObject>(initialSize);
            Allocate(initialSize);
        }

        private void Allocate(int count) {
            for (int i = 0; i < count; i++) {
                GameObject poolObject = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                poolObject.hideFlags = HideFlags.HideInHierarchy;
                poolObject.SetActive(false);
                activeList.Add(poolObject);
            }
        }

        public GameObject Spawn() {
            GameObject poolObject;
            if (activeList.Count > 0) {
                poolObject = activeList[activeList.Count - 1];
                activeList.RemoveAt(activeList.Count - 1);
            }
            else {
                Allocate(batchSize);
                poolObject = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            }

            poolObject.hideFlags = HideFlags.None;
            poolObject.SetActive(true);
            return poolObject;
        }

        public void Despawn(GameObject obj) {
            activeList.Add(obj);
            obj.hideFlags = HideFlags.HideInHierarchy;
            obj.SetActive(false);
        }

    }

}