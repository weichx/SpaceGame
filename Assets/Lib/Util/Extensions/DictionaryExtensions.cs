using System.Collections.Generic;

namespace Weichx.Util {

    public static class DictionaryExtensions {

        
        public static U Get<T, U>(this Dictionary<T, U> dict, T key) {
            U val;
            dict.TryGetValue(key, out val);
            return val;
        }

        public static U Get<T, U>(this Dictionary<T, U> dict, T key, U defaultValue) {
            U val;
            if (!dict.TryGetValue(key, out val)) {
                val = defaultValue;
            }
            return val;
        }

        public static U AddAndReturn<T, U>(this Dictionary<T, U> dict, T key, U value) {
            dict.Add(key, value);
            return value;
        }

    }

}