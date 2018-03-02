using System;

namespace SpaceGame.Util {

    public static class ArrayExtensions {

        public static void ForEach<T>(this T[] self, Action<T> callback) {
            for (int i = 0; i < self.Length; i++) {
                callback(self[i]);
            }
        }
        
        public static void ForEachIndex<T>(this T[] self, Action<T, int> callback) {
            for (int i = 0; i < self.Length; i++) {
                callback(self[i], i);
            }
        }

        public static U[] Map<T, U>(this T[] self, Func<T, U> callback) {
            U[] result = new U[self.Length];
            for (int i = 0; i < self.Length; i++) {
                result[i] = callback(self[i]);
            }

            return result;
        }
        
        public static U[] MapIndex<T, U>(this T[] self, Func<T, int, U> callback) {
            U[] result = new U[self.Length];
            for (int i = 0; i < self.Length; i++) {
                result[i] = callback(self[i], i);
            }

            return result;
        }
                
    }

}