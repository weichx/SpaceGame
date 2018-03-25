using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;

namespace Lib.Util {

    public interface IReadonlyListX<T> {

        T this[int index] {  get; }
        int Count { get; }
        int BinarySearch(int index, int count, T item, IComparer<T> comparer);
        int BinarySearch(T item);
        bool Contains(T item);
        bool ContainsReference<U>(U item) where U : class, T;
        void CopyTo(T[] array);
        void CopyTo(Array array, int arrayIndex);
        void CopyTo(int index, T[] array, int arrayIndex, int count);
        void CopyTo(T[] array, int arrayIndex);
        bool Exists(Predicate<T> match);
        T Find(Predicate<T> match);
        ListX<T> FindAll(Predicate<T> match);
        int FindIndex(Predicate<T> match);
        int FindIndex(int startIndex, Predicate<T> match);
        int FindIndex(int startIndex, int count, Predicate<T> match);
        T FindLast(Predicate<T> match);
        int FindLastIndex(Predicate<T> match);
        int FindLastIndex(int startIndex, Predicate<T> match);
        int FindLastIndex(int startIndex, int count, Predicate<T> match);
        void ForEach(Action<T> action);
        ListX<T>.Enumerator GetEnumerator();
        ListX<T> GetRange(int index, int count);
        int IndexOf(T item);
        int IndexOf(object item);
        int IndexOf(T item, int index, int count);
        int LastIndexOf(T item);
        int LastIndexOf(T item, int index);
        int LastIndexOf(T item, int index, int count);
        T[] ToArray();
        List<T> ToList();
        bool TrueForAll(Predicate<T> match);
        T Find<U>(U target, Func<T, U, bool> predicate);
        T Find(T target);
        T FindOrDefault(T defaultValue, Predicate<T> predicate);
        int FindByIndex<U>(U target, Func<T, U, bool> predicate);
        ListX<T> FindAll<U>(U target, Func<T, U, bool> predicate);
        ListX<U> Map<U>(Func<T, U> mapFn);
        ListX<V> Map<U, V>(U target, Func<T, U, V> mapFn);
        U[] MapArray<U>(Func<T, U> mapFn);
        V[] MapArray<U, V>(U target, Func<T, U, V> mapFn);
        bool Contains<U>(U value, Func<T, U, bool> predicate);
        T[] RawArray { get; }
        T First { get; }
        T Last { get; }
    }

    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class ListX<T> : IList<T>, IList, IReadOnlyList<T>, IReadonlyListX<T> {

        private static readonly T[] _emptyArray = new T[0];
        private T[] _items;
        private int _size;
        [NonSerialized] private object _syncRoot;

        public ListX() {
            this._items = _emptyArray;
        }

        public ListX(int capacity) {
            if (capacity < 0) capacity = 0;
            this._items = capacity == 0 ? _emptyArray : new T[capacity];
        }

        public ListX(IEnumerable<T> collection) {
            if (collection == null) {
                this._items = _emptyArray;
                return;
            }
            ICollection<T> objs = collection as ICollection<T>;
            if (objs != null) {
                int count = objs.Count;
                if (count == 0) {
                    this._items = _emptyArray;
                }
                else {
                    this._items = new T[count];
                    objs.CopyTo(this._items, 0);
                    this._size = count;
                }
            }
            else {
                this._size = 0;
                this._items = _emptyArray;
                foreach (T obj in collection)
                    this.Add(obj);
            }
        }

        public int Capacity {
            get { return this._items.Length; }
            set {
                if (value < this._size) {
                    return;
                }
                if (value == this._items.Length)
                    return;
                if (value > 0) {
                    T[] objArray = new T[value];
                    if (this._size > 0)
                        Array.Copy((Array) this._items, 0, (Array) objArray, 0, this._size);
                    this._items = objArray;
                }
                else
                    this._items = ListX<T>._emptyArray;
            }
        }

        public int Count {
            get { return this._size; }
        }

        bool IList.IsFixedSize {
            get { return false; }
        }

        bool ICollection<T>.IsReadOnly {
            get { return false; }
        }

        bool IList.IsReadOnly {
            get { return false; }
        }

        bool ICollection.IsSynchronized {
            get { return false; }
        }

        object ICollection.SyncRoot {
            get {
                if (this._syncRoot == null)
                    Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), (object) null);
                return this._syncRoot;
            }
        }

        public T this[int index] {
            get { return this._items[index]; }
            set { this._items[index] = value; }
        }

        private static bool IsCompatibleObject(object value) {
            if (value is T)
                return true;
            if (value == null)
                return (object) default(T) == null;
            return false;
        }

        object IList.this[int index] {
            get { return (object) this[index]; }
            set { this[index] = (T) value; }
        }

        public void Add(T item) {
            if (this._size == this._items.Length)
                this.EnsureCapacity(this._size + 1);
            this._items[this._size++] = item;
        }

        int IList.Add(object item) {
            this.Add((T) item);
            return this.Count - 1;
        }

        [PublicAPI]
        public void AddRange(IEnumerable<T> collection) {
            this.InsertRange(this._size, collection);
        }

        [PublicAPI]
        public ReadOnlyCollection<T> AsReadOnly() {
            return new ReadOnlyCollection<T>(this);
        }

        [PublicAPI]
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer) {
            return Array.BinarySearch<T>(this._items, index, count, item, comparer);
        }

        [PublicAPI]
        public int BinarySearch(T item) {
            return this.BinarySearch(0, this.Count, item, (IComparer<T>) null);
        }

        [PublicAPI]
        public int BinarySearch(T item, IComparer<T> comparer) {
            return this.BinarySearch(0, this.Count, item, comparer);
        }

        public void Clear() {
            if (this._size > 0) {
                Array.Clear((Array) this._items, 0, this._size);
                this._size = 0;
            }
        }

        public bool ContainsReference<U>(U item) where U : class, T {
            if (item == null) {
                for (int index = 0; index < this._size; ++index) {
                    if ((U)this._items[index] == null)
                        return true;
                }
                return false;
            }
            for (int index = 0; index < this._size; ++index) {
                if ((U)_items[index] ==  item)
                    return true;
            }
            return false;
        }
        
        public bool Contains(T item) {
            if (item == null) {
                for (int index = 0; index < this._size; ++index) {
                    if (this._items[index] == null)
                        return true;
                }
                return false;
            }
            EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
            for (int index = 0; index < this._size; ++index) {
                if (equalityComparer.Equals(this._items[index], item))
                    return true;
            }
            return false;
        }

        bool IList.Contains(object item) {
            if (ListX<T>.IsCompatibleObject(item))
                return this.Contains((T) item);
            return false;
        }

        public void CopyTo(T[] array) {
            this.CopyTo(array, 0);
        }

        public void CopyTo(Array array, int arrayIndex) {
            Array.Copy((Array) this._items, 0, array, arrayIndex, this._size);
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count) {
            Array.Copy((Array) this._items, index, (Array) array, arrayIndex, count);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            Array.Copy((Array) this._items, 0, (Array) array, arrayIndex, this._size);
        }

        private void EnsureCapacity(int min) {
            if (this._items.Length >= min)
                return;
            int num = this._items.Length == 0 ? 4 : this._items.Length * 2;
            if ((uint) num > 2146435071U)
                num = 2146435071;
            if (num < min)
                num = min;
            this.Capacity = num;
        }

        public bool Exists(Predicate<T> match) {
            return this.FindIndex(match) != -1;
        }

        public T Find(Predicate<T> match) {
            for (int index = 0; index < this._size; ++index) {
                if (match(this._items[index]))
                    return this._items[index];
            }
            return default(T);
        }

        public ListX<T> FindAll(Predicate<T> match) {
            ListX<T> objListX = new ListX<T>();
            for (int index = 0; index < this._size; ++index) {
                if (match(this._items[index]))
                    objListX.Add(this._items[index]);
            }
            return objListX;
        }

        public int FindIndex(Predicate<T> match) {
            return this.FindIndex(0, this._size, match);
        }

        public int FindIndex(int startIndex, Predicate<T> match) {
            return this.FindIndex(startIndex, this._size - startIndex, match);
        }

        public int FindIndex(int startIndex, int count, Predicate<T> match) {
            int num = startIndex + count;
            for (int index = startIndex; index < num; ++index) {
                if (match(this._items[index]))
                    return index;
            }
            return -1;
        }

        public T FindLast(Predicate<T> match) {
            for (int index = this._size - 1; index >= 0; --index) {
                if (match(this._items[index]))
                    return this._items[index];
            }
            return default(T);
        }

        public int FindLastIndex(Predicate<T> match) {
            return this.FindLastIndex(this._size - 1, this._size, match);
        }

        public int FindLastIndex(int startIndex, Predicate<T> match) {
            return this.FindLastIndex(startIndex, startIndex + 1, match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> match) {
            int num = startIndex - count;
            for (int index = startIndex; index > num; --index) {
                if (match(this._items[index]))
                    return index;
            }
            return -1;
        }

        public void ForEach(Action<T> action) {
            int length = this._size;
            for (int index = 0; index < length; ++index)
                action(this._items[index]);
        }

        public ListX<T>.Enumerator GetEnumerator() {
            return new ListX<T>.Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return (IEnumerator<T>) new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return (IEnumerator) new Enumerator(this);
        }

        public ListX<T> GetRange(int index, int count) {
            ListX<T> objListX = new ListX<T>(count);
            Array.Copy((Array) this._items, index, (Array) objListX._items, 0, count);
            objListX._size = count;
            return objListX;
        }

        public int IndexOf(T item) {
            return Array.IndexOf<T>(this._items, item, 0, this._size);
        }

        public int IndexOf(object item) {
            if (ListX<T>.IsCompatibleObject(item))
                return this.IndexOf((T) item);
            return -1;
        }

        public int IndexOf(T item, int index) {
            return Array.IndexOf<T>(this._items, item, index, this._size - index);
        }

        public int IndexOf(T item, int index, int count) {
            return Array.IndexOf<T>(this._items, item, index, count);
        }

        public void Insert(int index, T item) {
            if ((uint) index > (uint) this._size) {
                Add(item);
                return;
            }
            if (this._size == this._items.Length)
                this.EnsureCapacity(this._size + 1);
            if (index < this._size)
                Array.Copy((Array) this._items, index, (Array) this._items, index + 1, this._size - index);
            this._items[index] = item;
            ++this._size;
        }

        void IList.Insert(int index, object item) {
            this.Insert(index, (T) item);
        }

        public void InsertRange(int index, IEnumerable<T> collection) {
            if ((uint) index > (uint) this._size) {
                foreach (T item in collection) {
                    Add(item);
                }
                return;
            }
            ICollection<T> objs = collection as ICollection<T>;
            if (objs != null) {
                int count = objs.Count;
                if (count > 0) {
                    this.EnsureCapacity(this._size + count);
                    if (index < this._size)
                        Array.Copy((Array) this._items, index, (Array) this._items, index + count, this._size - index);
                    if (this == objs) {
                        Array.Copy((Array) this._items, 0, (Array) this._items, index, index);
                        Array.Copy((Array) this._items, index + count, (Array) this._items, index * 2, this._size - index);
                    }
                    else {
                        T[] array = new T[count];
                        objs.CopyTo(array, 0);
                        array.CopyTo((Array) this._items, index);
                    }
                    this._size += count;
                }
            }
            else {
                foreach (T obj in collection)
                    this.Insert(index++, obj);
            }
        }

        public int LastIndexOf(T item) {
            if (this._size == 0)
                return -1;
            return this.LastIndexOf(item, this._size - 1, this._size);
        }

        public int LastIndexOf(T item, int index) {
            if (index >= this._size) {
                return -1;
            }
            return this.LastIndexOf(item, index, index + 1);
        }

        public int LastIndexOf(T item, int index, int count) {
            if (this._size == 0 || (uint) index >= this._size) {
                return -1;
            }
            return Array.LastIndexOf<T>(this._items, item, index, count);
        }

        public bool Remove(T item) {
            int index = this.IndexOf(item);
            if (index < 0)
                return false;
            this.RemoveAt(index);
            return true;
        }

        void IList.Remove(object item) {
            if (!ListX<T>.IsCompatibleObject(item))
                return;
            this.Remove((T) item);
        }

        public int RemoveAll(Predicate<T> match) {

            int index1 = 0;
            while (index1 < this._size && !match(this._items[index1]))
                ++index1;
            if (index1 >= this._size)
                return 0;
            int index2 = index1 + 1;
            while (index2 < this._size) {
                while (index2 < this._size && match(this._items[index2]))
                    ++index2;
                if (index2 < this._size)
                    this._items[index1++] = this._items[index2++];
            }
            Array.Clear((Array) this._items, index1, this._size - index1);
            int num = this._size - index1;
            this._size = index1;
            return num;
        }

        public void RemoveAt(int index) {
            if ((uint) index >= (uint) this._size) {
                return;
            }
            --this._size;
            if (index < this._size)
                Array.Copy((Array) this._items, index + 1, (Array) this._items, index, this._size - index);
            this._items[this._size] = default(T);
        }

        public void RemoveRange(int index, int count) {
            if (index < 0 || count < 0 || this._size - index < count) {
                return;
            }
            int size = this._size;
            this._size -= count;
            if (index < this._size)
                Array.Copy((Array) this._items, index + count, (Array) this._items, index, this._size - index);
            Array.Clear((Array) this._items, this._size, count);
        }

        public void Reverse() {
            this.Reverse(0, this.Count);
        }

        public void Reverse(int index, int count) {
            if (index < 0 || count < 0 || this._size - index < count) {
                return;
            }
            Array.Reverse((Array) this._items, index, count);
        }

        public void Sort() {
            this.Sort(0, this.Count, (IComparer<T>) null);
        }

        public void Sort(IComparer<T> comparer) {
            this.Sort(0, this.Count, comparer);
        }

        public void Sort(int index, int count, IComparer<T> comparer) {
            if (index < 0 || count < 0 || this._size - index < count) {
                return;
            }
            Array.Sort<T>(this._items, index, count, comparer);
        }

        public T[] ToArray() {
            T[] objArray = new T[this._size];
            Array.Copy((Array) this._items, 0, (Array) objArray, 0, this._size);
            return objArray;
        }

        public List<T> ToList() {
            return new List<T>(this);
        }

        public void TrimExcess() {
            if (this._size >= (int) ((double) this._items.Length * 0.9))
                return;
            this.Capacity = this._size;
        }

        public bool TrueForAll(Predicate<T> match) {
            for (int index = 0; index < this._size; ++index) {
                if (!match(this._items[index]))
                    return false;
            }
            return true;
        }

        [DebuggerStepThrough]
        public T Find<U>(U target, Func<T, U, bool> predicate) {
            T[] items = _items;
            int length = _size;
            for (int i = 0; i < length; i++) {
                if (predicate(items[i], target)) {
                    return items[i];
                }
            }
            return default(T);
        }

        public T Find(T target) {
            for (int i = 0; i < Count; i++) {
                if ((Equals(target, _items[i]))) {
                    return _items[i];
                }
            }
            return default(T);
        }

        [DebuggerStepThrough]
        public T FindOrDefault(T defaultValue, Predicate<T> predicate) {
            int resultIndex = FindIndex(predicate);
            if (resultIndex == -1) return defaultValue;
            return _items[resultIndex];
        }

        [DebuggerStepThrough]
        public int FindByIndex<U>(U target, Func<T, U, bool> predicate) {
            T[] items = _items;
            int length = _size;
            for (int i = 0; i < length; i++) {
                if (predicate(items[i], target)) {
                    return i;
                }
            }
            return -1;
        }

        [DebuggerStepThrough]
        public ListX<T> FindAll<U>(U target, Func<T, U, bool> predicate) {
            ListX<T> retn = new ListX<T>(4);
            int length = _size;
            for (int i = 0; i < length; i++) {
                if (predicate(_items[i], target)) {
                    retn.Add(_items[i]);
                }
            }
            return retn;
        }

        [DebuggerStepThrough]
        public ListX<U> Map<U>(Func<T, U> mapFn) {
            ListX<U> retn = new ListX<U>(_size);
            for (int i = 0; i < _size; i++) {
                retn.Add(mapFn(_items[i]));
            }
            return retn;
        }

        [DebuggerStepThrough]
        public ListX<V> Map<U, V>(U target, Func<T, U, V> mapFn) {
            ListX<V> retn = new ListX<V>(_size);
            for (int i = 0; i < _size; i++) {
                retn.Add(mapFn(_items[i], target));
            }
            return retn;
        }

        [DebuggerStepThrough]
        public U[] MapArray<U>(Func<T, U> mapFn) {
            U[] retn = new U[_size];
            for (int i = 0; i < _size; i++) {
                retn[i] = mapFn(_items[i]);
            }
            return retn;
        }

        [DebuggerStepThrough]
        public V[] MapArray<U, V>(U target, Func<T, U, V> mapFn) {
            V[] retn = new V[_size];
            for (int i = 0; i < _size; i++) {
                retn[i] = mapFn(_items[i], target);
            }
            return retn;
        }

        [DebuggerStepThrough]
        public bool Contains<U>(U value, Func<T, U, bool> predicate) {
            return FindByIndex(value, predicate) != -1;
        }

        [DebuggerStepThrough]
        public void Resize(int sz, T c = default(T)) {
            int cur = Count;
            if (sz < cur)
                RemoveRange(sz, cur - sz);
            else if (sz > cur)
                AddRange(Enumerable.Repeat(c, sz - cur));
        }

        /// <summary>
        /// Moves an item within this list to another index, shifting other items as needed.
        /// </summary>
        [DebuggerStepThrough]
        public bool MoveToIndex(int oldIndex, int insertIndex) {
            if (insertIndex == -1) {
                insertIndex = _size - 1;
            }
            if ((uint) oldIndex >= _size) return false;
            if (insertIndex > oldIndex) insertIndex--;
            if ((uint) insertIndex >= _size) return false;
            T item = _items[oldIndex];
            RemoveAt(oldIndex);
            Insert(insertIndex, item);
            return true;
        }

        /// <summary>
        /// Moves an item within this list to another index, shifting other items as needed.
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public bool MoveToIndex(T item, int insertIndex) {
            return MoveToIndex(IndexOf(item), insertIndex);
        }

        public T RemoveAndReturn(T item) {
            int index = IndexOf(item);
            if (index == -1) return default(T);
            T retn = _items[index];
            RemoveAt(index);
            return retn;
        }

        public T RemoveAndReturnAtIndex(int index) {
            if ((uint) index >= _size) return default(T);
            T retn = _items[index];
            RemoveAt(index);
            return retn;
        }

        public T[] RawArray => _items;

        public T First => _items[0];
        public T Last => _items[_size - 1];

        [Serializable]
        public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator {

            private ListX<T> listX;
            private int index;
            private T current;

            internal Enumerator(ListX<T> listX) {
                this.listX = listX;
                this.index = 0;
                this.current = default(T);
            }

            public void Dispose() { }

            public bool MoveNext() {
                if ((uint) this.index >= (uint) listX._size)
                    return this.MoveNextRare();
                this.current = listX._items[this.index];
                ++this.index;
                return true;
            }

            private bool MoveNextRare() {
                this.index = this.listX._size + 1;
                this.current = default(T);
                return false;
            }

            public T Current {
                get { return this.current; }
            }

            object IEnumerator.Current {
                get {
                    if (this.index == 0 || this.index == this.listX._size + 1) {
                        return null;
                    }
                    return (object) this.Current;
                }
            }

            void IEnumerator.Reset() {
                this.index = 0;
                this.current = default(T);
            }

        }

    }

}