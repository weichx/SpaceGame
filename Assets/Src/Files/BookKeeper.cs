using System.Collections.Generic;
using System.Linq;
using Weichx.Util;

namespace SpaceGame {

    public class BookKeeper<T, U> {

        private Dictionary<T, U> forward;
        private Dictionary<U, T> reverse;

        public BookKeeper() {
            this.forward = new Dictionary<T, U>();
            this.reverse = new Dictionary<U, T>();
        }

        public int Count => forward.Count;

        private bool IsDefault(T value) {
            return EqualityComparer<T>.Default.Equals(value, default(T));
        }

        private bool IsDefault(U value) {
            return EqualityComparer<U>.Default.Equals(value, default(U));
        }

        public void Set(T t, U u) {
            if (IsDefault(t) && IsDefault(u)) return;
            if (IsDefault(t)) {
                reverse.Remove(u);
            }
            else if (IsDefault(u)) {
                forward.Remove(t);
            }
            else {
                forward[t] = u;
                reverse[u] = t;
            }
        }

        public void Remove(T t) {
            Set(t, default(U));
        }

        public void Remove(U u) {
            Set(default(T), u);
        }

        public void Clear() {
            forward.Clear();
            reverse.Clear();
        }

        public T Get(U u) {
            return reverse.Get(u);
        }

        public U Get(T t) {
            return forward.Get(t);
        }

        public U this[T t] {
            get { return Get(t); }
            set { Set(t, value); }
        }

        public T this[U u] {
            get { return Get(u); }
            set { Set(value, u); }
        }

        public List<T> GetTValues() {
            return forward.Keys.ToList();
        }

        public List<U> GetUValues() {
            return reverse.Keys.ToList();
        }

        public bool TryGetValue(T key, out U outVal) {
            return forward.TryGetValue(key, out outVal);
        }

        public bool TryGetValue(U key, out T outVal) {
            return reverse.TryGetValue(key, out outVal);
        }

        public bool Contains(T key) {
            return forward.ContainsKey(key);
        }

        public bool Contains(U key) {
            return reverse.ContainsKey(key);
        }

    }

}