using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Util {

    public static class ListExtensions {

        public static void EnsureCapacity<T>(this List<T> list, int minCapacity, T c = default(T)) {
            if (list.Capacity <= minCapacity) {
                Resize(list, minCapacity, c);
            }
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
        public static void InsertIntoSortedList<T>(this IList<T> list,T value,Comparison<T> comparison) {
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