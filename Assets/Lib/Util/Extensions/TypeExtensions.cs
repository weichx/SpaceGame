using System;
using System.Reflection;
using UnityEngine;

namespace Weichx.Util {

    public static class TypeExtensions {

        public static bool AssertCompleteValueType(this Type type) {
            if (!type.IsValueType) {
                Debug.LogError($"Type {type.Name} is supposed to be a value type but is not");
                return false;
            }
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            bool isValueType = true;
            for (int i = 0; i < fields.Length; i++) {
                if (!fields[i].FieldType.IsValueType) {
                    Debug.LogError($"Type {type.Name} is supposed to contain only value types but \"{fields[i].Name}\" ({fields[i].FieldType.Name}) is not a value type");
                    isValueType = false;
                }
                else {
                    isValueType = fields[i].FieldType.AssertCompleteValueType();
                }
            }
            return isValueType;
        }

    }

}