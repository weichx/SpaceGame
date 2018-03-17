﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Src.Editor {

    public static class SerializedPropertyUtil {

        public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty property) {
            property = property.Copy();
            var nextElement = property.Copy();
            bool hasNextElement = nextElement.NextVisible(false);
            if (!hasNextElement) {
                nextElement = null;
            }

            property.NextVisible(true);
            while (true) {
                if ((SerializedProperty.EqualContents(property, nextElement))) {
                    yield break;
                }

                yield return property;

                bool hasNext = property.NextVisible(false);
                if (!hasNext) {
                    break;
                }
            }
        }

        public static System.Type GetTargetType(this SerializedObject obj) {
            if (obj == null) return null;

            if (obj.isEditingMultipleObjects) {
                var c = obj.targetObjects[0];
                return c.GetType();
            }
            else {
                return obj.targetObject.GetType();
            }
        }

        /// <summary>
        /// Gets the object the property represents.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static object GetTargetObjectOfProperty(SerializedProperty prop) {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements) {
                if (element.Contains("[")) {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else {
                    obj = GetValue_Imp(obj, element);
                }
            }

            return obj;
        }

        /// <summary>
        /// Gets the object that the property is a member of
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static object GetTargetObjectWithProperty(SerializedProperty prop) {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1)) {
                if (element.Contains("[")) {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else {
                    obj = GetValue_Imp(obj, element);
                }
            }

            return obj;
        }

        private static object GetValue_Imp(object source, string name) {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null) {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }

            return null;
        }

        private static object GetValue_Imp(object source, string name, int index) {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();
            //while (index-- >= 0)
            //    enm.MoveNext();
            //return enm.Current;

            for (int i = 0; i <= index; i++) {
                if (!enm.MoveNext()) return null;
            }

            return enm.Current;
        }

//        public static void SetEnumValue<T>(this SerializedProperty prop, T value) where T : struct {
//            if (prop == null) throw new System.ArgumentNullException("prop");
//            if (prop.propertyType != SerializedPropertyType.Enum) throw new System.ArgumentException("SerializedProperty is not an enum type.", "prop");
//
//            //var tp = typeof(T);
//            //if(tp.IsEnum)
//            //{
//            //    prop.enumValueIndex = prop.enumNames.IndexOf(System.Enum.GetName(tp, value));
//            //}
//            //else
//            //{
//            //    int i = ConvertUtil.ToInt(value);
//            //    if (i < 0 || i >= prop.enumNames.Length) i = 0;
//            //    prop.enumValueIndex = i;
//            //}
//            prop.intValue = ConvertUtil.ToInt(value);
//        }

//        public static void SetEnumValue(this SerializedProperty prop, System.Enum value) {
//            if (prop == null) throw new System.ArgumentNullException("prop");
//            if (prop.propertyType != SerializedPropertyType.Enum) throw new System.ArgumentException("SerializedProperty is not an enum type.", "prop");
//
//            if (value == null) {
//                prop.enumValueIndex = 0;
//                return;
//            }
//
//            //int i = prop.enumNames.IndexOf(System.Enum.GetName(value.GetType(), value));
//            //if (i < 0) i = 0;
//            //prop.enumValueIndex = i;
//            prop.intValue = ConvertUtil.ToInt(value);
//        }

//        public static void SetEnumValue(this SerializedProperty prop, object value) {
//            if (prop == null) throw new System.ArgumentNullException("prop");
//            if (prop.propertyType != SerializedPropertyType.Enum) throw new System.ArgumentException("SerializedProperty is not an enum type.", "prop");
//
//            if (value == null) {
//                prop.enumValueIndex = 0;
//                return;
//            }
//
//            //var tp = value.GetType();
//            //if (tp.IsEnum)
//            //{
//            //    int i = prop.enumNames.IndexOf(System.Enum.GetName(tp, value));
//            //    if (i < 0) i = 0;
//            //    prop.enumValueIndex = i;
//            //}
//            //else
//            //{
//            //    int i = ConvertUtil.ToInt(value);
//            //    if (i < 0 || i >= prop.enumNames.Length) i = 0;
//            //    prop.enumValueIndex = i;
//            //}
//            prop.intValue = ConvertUtil.ToInt(value);
//        }

//        public static T GetEnumValue<T>(this SerializedProperty prop) where T : struct, System.IConvertible {
//            if (prop == null) throw new System.ArgumentNullException("prop");
//
//            try {
//                //var name = prop.enumNames[prop.enumValueIndex];
//                //return ConvertUtil.ToEnum<T>(name);
//                return ConvertUtil.ToEnum<T>(prop.intValue);
//            }
//            catch {
//                return default(T);
//            }
//        }
//
//        public static System.Enum GetEnumValue(this SerializedProperty prop, System.Type tp) {
//            if (prop == null) throw new System.ArgumentNullException("prop");
//            if (tp == null) throw new System.ArgumentNullException("tp");
//            if (!tp.IsEnum) throw new System.ArgumentException("Type must be an enumerated type.");
//
//            try {
//                //var name = prop.enumNames[prop.enumValueIndex];
//                //return System.Enum.Parse(tp, name) as System.Enum;
//                return ConvertUtil.ToEnumOfType(tp, prop.intValue);
//            }
//            catch {
//                return System.Enum.GetValues(tp).Cast<System.Enum>().First();
//            }
//        }
//
//        public static void SetPropertyValue(SerializedProperty prop, object value) {
//            if (prop == null) throw new System.ArgumentNullException("prop");
//
//            switch (prop.propertyType) {
//                case SerializedPropertyType.Integer:
//                    prop.intValue = ConvertUtil.ToInt(value);
//                    break;
//                case SerializedPropertyType.Boolean:
//                    prop.boolValue = ConvertUtil.ToBool(value);
//                    break;
//                case SerializedPropertyType.Float:
//                    prop.floatValue = ConvertUtil.ToSingle(value);
//                    break;
//                case SerializedPropertyType.String:
//                    prop.stringValue = ConvertUtil.ToString(value);
//                    break;
//                case SerializedPropertyType.Color:
//                    prop.colorValue = ConvertUtil.ToColor(value);
//                    break;
//                case SerializedPropertyType.ObjectReference:
//                    prop.objectReferenceValue = value as Object;
//                    break;
//                case SerializedPropertyType.LayerMask:
//                    prop.intValue = (value is LayerMask) ? ((LayerMask) value).value : ConvertUtil.ToInt(value);
//                    break;
//                case SerializedPropertyType.Enum:
//                    //prop.enumValueIndex = ConvertUtil.ToInt(value);
//                    prop.SetEnumValue(value);
//                    break;
//                case SerializedPropertyType.Vector2:
//                    prop.vector2Value = ConvertUtil.ToVector2(value);
//                    break;
//                case SerializedPropertyType.Vector3:
//                    prop.vector3Value = ConvertUtil.ToVector3(value);
//                    break;
//                case SerializedPropertyType.Vector4:
//                    prop.vector4Value = ConvertUtil.ToVector4(value);
//                    break;
//                case SerializedPropertyType.Rect:
//                    prop.rectValue = (Rect) value;
//                    break;
//                case SerializedPropertyType.ArraySize:
//                    prop.arraySize = ConvertUtil.ToInt(value);
//                    break;
//                case SerializedPropertyType.Character:
//                    prop.intValue = ConvertUtil.ToInt(value);
//                    break;
//                case SerializedPropertyType.AnimationCurve:
//                    prop.animationCurveValue = value as AnimationCurve;
//                    break;
//                case SerializedPropertyType.Bounds:
//                    prop.boundsValue = (Bounds) value;
//                    break;
//                case SerializedPropertyType.Gradient:
//                    throw new System.InvalidOperationException("Can not handle Gradient types.");
//            }
//        }
//
//        public static object GetPropertyValue(SerializedProperty prop) {
//            if (prop == null) throw new System.ArgumentNullException("prop");
//
//            switch (prop.propertyType) {
//                case SerializedPropertyType.Integer:
//                    return prop.intValue;
//                case SerializedPropertyType.Boolean:
//                    return prop.boolValue;
//                case SerializedPropertyType.Float:
//                    return prop.floatValue;
//                case SerializedPropertyType.String:
//                    return prop.stringValue;
//                case SerializedPropertyType.Color:
//                    return prop.colorValue;
//                case SerializedPropertyType.ObjectReference:
//                    return prop.objectReferenceValue;
//                case SerializedPropertyType.LayerMask:
//                    return (LayerMask) prop.intValue;
//                case SerializedPropertyType.Enum:
//                    return prop.enumValueIndex;
//                case SerializedPropertyType.Vector2:
//                    return prop.vector2Value;
//                case SerializedPropertyType.Vector3:
//                    return prop.vector3Value;
//                case SerializedPropertyType.Vector4:
//                    return prop.vector4Value;
//                case SerializedPropertyType.Rect:
//                    return prop.rectValue;
//                case SerializedPropertyType.ArraySize:
//                    return prop.arraySize;
//                case SerializedPropertyType.Character:
//                    return (char) prop.intValue;
//                case SerializedPropertyType.AnimationCurve:
//                    return prop.animationCurveValue;
//                case SerializedPropertyType.Bounds:
//                    return prop.boundsValue;
//                case SerializedPropertyType.Gradient:
//                    throw new System.InvalidOperationException("Can not handle Gradient types.");
//            }
//
//            return null;
//        }
//
//        public static SerializedPropertyType GetPropertyType(System.Type tp) {
//            if (tp == null) throw new System.ArgumentNullException("tp");
//
//            if (tp.IsEnum) return SerializedPropertyType.Enum;
//
//            var code = System.Type.GetTypeCode(tp);
//            switch (code) {
//                case System.TypeCode.SByte:
//                case System.TypeCode.Byte:
//                case System.TypeCode.Int16:
//                case System.TypeCode.UInt16:
//                case System.TypeCode.Int32:
//                    return SerializedPropertyType.Integer;
//                case System.TypeCode.Boolean:
//                    return SerializedPropertyType.Boolean;
//                case System.TypeCode.Single:
//                    return SerializedPropertyType.Float;
//                case System.TypeCode.String:
//                    return SerializedPropertyType.String;
//                case System.TypeCode.Char:
//                    return SerializedPropertyType.Character;
//                default: {
//                    if (TypeUtil.IsType(tp, typeof(Color)))
//                        return SerializedPropertyType.Color;
//                    else if (TypeUtil.IsType(tp, typeof(UnityEngine.Object)))
//                        return SerializedPropertyType.ObjectReference;
//                    else if (TypeUtil.IsType(tp, typeof(LayerMask)))
//                        return SerializedPropertyType.LayerMask;
//                    else if (TypeUtil.IsType(tp, typeof(Vector2)))
//                        return SerializedPropertyType.Vector2;
//                    else if (TypeUtil.IsType(tp, typeof(Vector3)))
//                        return SerializedPropertyType.Vector3;
//                    else if (TypeUtil.IsType(tp, typeof(Vector4)))
//                        return SerializedPropertyType.Vector4;
//                    else if (TypeUtil.IsType(tp, typeof(Quaternion)))
//                        return SerializedPropertyType.Quaternion;
//                    else if (TypeUtil.IsType(tp, typeof(Rect)))
//                        return SerializedPropertyType.Rect;
//                    else if (TypeUtil.IsType(tp, typeof(AnimationCurve)))
//                        return SerializedPropertyType.AnimationCurve;
//                    else if (TypeUtil.IsType(tp, typeof(Bounds)))
//                        return SerializedPropertyType.Bounds;
//                    else if (TypeUtil.IsType(tp, typeof(Gradient)))
//                        return SerializedPropertyType.Gradient;
//                }
//                    return SerializedPropertyType.Generic;
//            }
//        }
//
//        public static int GetChildPropertyCount(SerializedProperty property, bool includeGrandChildren = false) {
//            var pstart = property.Copy();
//            var pend = property.GetEndProperty();
//            int cnt = 0;
//
//            pstart.Next(true);
//            while (!SerializedProperty.EqualContents(pstart, pend)) {
//                cnt++;
//                pstart.Next(includeGrandChildren);
//            }
//
//            return cnt;
//        }

    }

}