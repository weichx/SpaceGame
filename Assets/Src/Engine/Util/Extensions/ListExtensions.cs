using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Util {

    public class ReadonlySearchableList<T> : List<T> {

        public ReadonlySearchableList(List<T> list) {
            
        }

    }
    
    public static class ListExtensions {

        public static void EnsureCapacity<T>(this List<T> list, int minCapacity, T c = default(T)) {
            if (list.Capacity <= minCapacity) {
                Resize(list, minCapacity, c);
            }
        }

        public static T Find<T, U>(this List<T> list, U target, Func<T, U, bool> predicate) {
            for (int i = 0; i < list.Count; i++) {
                if (predicate(list[i], target)) {
                    return list[i];
                }
            }
            return default(T);
        }

        public static T FindOrDefault<T>(this List<T> list, T defaultValue, Predicate<T> predicate) {
            int resultIndex = list.FindIndex(predicate);
            if (resultIndex == -1) return defaultValue;
            return list[resultIndex];
        }

        public static int FindByIndex<T, U>(this List<T> list, U target, Func<T, U, bool> predicate) {
            for (int i = 0; i < list.Count; i++) {
                if (predicate(list[i], target)) {
                    return i;
                }
            }
            return -1;
        }

        public static List<T> FindAll<T, U>(this List<T> list, U target, Func<T, U, bool> predicate) {
            List<T> retn = new List<T>(4);
            for (int i = 0; i < list.Count; i++) {
                if (predicate(list[i], target)) {
                    retn.Add(list[i]);
                }
            }
            return retn;
        }

        public static List<U> Map<T, U>(this List<T> list, Func<T, U> mapFn) {
            List<U> retn = new List<U>(list.Count);
            for (int i = 0; i < list.Count; i++) {
                retn.Add(mapFn(list[i]));
            }
            return retn;
        }

        public static List<V> Map<T, U, V>(this List<T> list, U target, Func<T, U, V> mapFn) {
            List<V> retn = new List<V>(list.Count);
            for (int i = 0; i < list.Count; i++) {
                retn.Add(mapFn(list[i], target));
            }
            return retn;
        }

        public static U[] MapArray<T, U>(this List<T> list, Func<T, U> mapFn) {
            U[] retn = new U[list.Count];
            for (int i = 0; i < list.Count; i++) {
                retn[i] = mapFn(list[i]);
            }
            return retn;
        }

        public static V[] MapArray<T, U, V>(this List<T> list, U target, Func<T, U, V> mapFn) {
            V[] retn = new V[list.Count];
            for (int i = 0; i < list.Count; i++) {
                retn[i] = mapFn(list[i], target);
            }
            return retn;
        }

        public static bool Contains<T, U>(this List<T> list, U value, Func<T, U, bool> predicate) {
            return list.FindByIndex(value, predicate) != -1;
        }

        public static void Resize<T>(this List<T> list, int sz, T c = default(T)) {
            int cur = list.Count;
            if (sz < cur)
                list.RemoveRange(sz, cur - sz);
            else if (sz > cur)
                list.AddRange(Enumerable.Repeat(c, sz - cur));
        }

        public static T First<T>(this List<T> list) {
            return list.Count > 0 ? list[0] : default(T);
        }

        public static T Last<T>(this List<T> list) {
            return list.Count > 0 ? list[list.Count - 1] : default(T);
        }

        /// <summary>
        /// Insert a value into an IList{T} that is presumed to be already sorted such that sort
        /// ordering is preserved
        /// </summary>
        /// <param name="list">List to insert into</param>
        /// <param name="value">Value to insert</param>
        /// <typeparam name="T">Type of element to insert and type of elements in the list</typeparam>
        public static void InsertIntoSortedList<T>(this IList<T> list, T value)
            where T : IComparable<T> {
            InsertIntoSortedList(list, value, (a, b) => a.CompareTo(b));
        }

        /// <summary>
        /// Insert a value into an IList{T} that is presumed to be already sorted such that sort
        /// ordering is preserved
        /// </summary>
        /// <param name="list">List to insert into</param>
        /// <param name="value">Value to insert</param>
        /// <param name="comparison">Comparison to determine sort order with</param>
        /// <typeparam name="T">Type of element to insert and type of elements in the list</typeparam>
        public static void InsertIntoSortedList<T>(this IList<T> list, T value, Comparison<T> comparison) {
            int startIndex = 0;
            int endIndex = list.Count;
            while (endIndex > startIndex) {
                int windowSize = endIndex - startIndex;
                int middleIndex = startIndex + (windowSize / 2);
                T middleValue = list[middleIndex];
                int compareToResult = comparison(middleValue, value);
                if (compareToResult == 0) {
                    list.Insert(middleIndex, value);
                    return;
                }
                else if (compareToResult < 0) {
                    startIndex = middleIndex + 1;
                }
                else {
                    endIndex = middleIndex;
                }
            }

            list.Insert(startIndex, value);
        }

        /// <summary>
        /// Insert a value into an IList that is presumed to be already sorted such that sort ordering is preserved
        /// </summary>
        /// <param name="list">List to insert into</param>
        /// <param name="value">Value to insert</param>
        public static void InsertIntoSortedList(this IList list, IComparable value) {
            InsertIntoSortedList(list, value, (a, b) => a.CompareTo(b));
        }

        /// <summary>
        /// Insert a value into an IList that is presumed to be already sorted such that sort ordering is preserved
        /// </summary>
        /// <param name="list">List to insert into</param>
        /// <param name="value">Value to insert</param>
        /// <param name="comparison">Comparison to determine sort order with</param>
        public static void InsertIntoSortedList(this IList list, IComparable value, Comparison<IComparable> comparison) {
            int startIndex = 0;
            int endIndex = list.Count;
            while (endIndex > startIndex) {
                int windowSize = endIndex - startIndex;
                int middleIndex = startIndex + (windowSize / 2);
                IComparable middleValue = (IComparable) list[middleIndex];
                int compareToResult = comparison(middleValue, value);
                if (compareToResult == 0) {
                    list.Insert(middleIndex, value);
                    return;
                }
                else if (compareToResult < 0) {
                    startIndex = middleIndex + 1;
                }
                else {
                    endIndex = middleIndex;
                }
            }

            list.Insert(startIndex, value);
        }

    }

}