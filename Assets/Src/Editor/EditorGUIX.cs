using UnityEngine;
using UnityEditor;
using System;
using SpaceGame.Editor.Reflection;

namespace SpaceGame.EditorGUIX {


    public static class EditorGUILayoutX {

        public static void DrawProperties(ReflectedObject root) {
            for (int i = 0; i < root.ChildCount; i++) {
                ReflectedProperty property = root.GetChildAt(i);
                PropertyField(property, property.guiContent, property.IsExpanded);
            }
        }

        public static Rect GetControlRect(ReflectedProperty property, GUIContent label = null) {
            label = label ?? property.guiContent;
            return EditorGUILayout.GetControlRect(true, EditorGUIUtilityX.GetHeight(property, label, property.IsExpanded));
        }

        public static void PropertyField(ReflectedProperty property, params GUILayoutOption[] options) {
            if (property == null) return;
            PropertyField(property, property.guiContent, true, options);
        }

        public static void PropertyField(ReflectedProperty property, bool includeChildren, params GUILayoutOption[] options) {
            PropertyField(property, property.guiContent, includeChildren, options);
        }

        public static void PropertyField(ReflectedProperty property, GUIContent label, bool includeChildren, params GUILayoutOption[] options) {
            Type type = property.Type;

            Type drawerType = EditorReflector.GetPropertyDrawerForType(property);

            if (drawerType != null) {
                ((ReflectedPropertyDrawer)Activator.CreateInstance(drawerType)).OnGUI(property, label);
                return;
            }
            
            if (type.IsSubclassOf(typeof(UnityEngine.Object))) {
                property.Value = EditorGUILayout.ObjectField(label, (UnityEngine.Object) property.Value, type, true, options);
            }
            else if (type.IsArray) {
                property.IsExpanded = EditorGUILayout.Foldout(property.IsExpanded, label.text);
                if (property.IsExpanded) {
                    EditorGUI.indentLevel++;
                    property.ArraySize = EditorGUILayout.IntField(EditorGUIUtilityX.TempLabel("Size"), property.ArraySize);
                    for (int i = 0; i < property.ArraySize; i++) {
                        ReflectedProperty child = property.ChildAt(i);
                        PropertyField(child, child.guiContent, child.IsExpanded, options);
                    }
                    EditorGUI.indentLevel--;
                }
            }
            else if (type.IsEnum) {
                property.Value = EditorGUILayout.EnumPopup(label, (Enum) property.Value, options);
            }
            else if (type == typeof(Color)) {
                property.Value = EditorGUILayout.ColorField(label, (Color) property.Value);
            }
            else if (type == typeof(Bounds)) {
                Bounds b = (Bounds) property.Value;
                property.Value = EditorGUILayout.BoundsField(label, b, options);
            }
            else if (type == typeof(AnimationCurve)) {
                if (property.Value == null) property.Value = new AnimationCurve();
                property.Value = EditorGUILayout.CurveField(label, (AnimationCurve) property.Value, options);
            }
            else if (type == typeof(double)) {
                property.Value = EditorGUILayout.DoubleField(label, (double) property.Value);
            }
            else if (type == typeof(float)) {
                property.Value = EditorGUILayout.FloatField(label, (float) property.Value);
            }
            else if (type == typeof(int)) {
                property.Value = EditorGUILayout.IntField(label, (int) property.Value, options);
            }
            else if (type == typeof(long)) {
                property.Value = EditorGUILayout.LongField(label, (long) property.Value, options);
            }
            else if (type == typeof(Rect)) {
                property.Value = EditorGUILayout.RectField(label, (Rect) property.Value, options);
            }
            else if (type == typeof(bool)) {
                property.Value = EditorGUILayout.Toggle(label, (bool) property.Value, options);
            }
            else if (type == typeof(Vector2)) {
                property.Value = EditorGUILayout.Vector2Field(label, (Vector2) property.Value, options);
            }
            else if (type == typeof(Vector3)) {
                property.Value = EditorGUILayout.Vector3Field(label, (Vector3) property.Value, options);
            }
            else if (type == typeof(Vector4)) {
                property.Value = EditorGUILayout.Vector4Field(label.text, (Vector4) property.Value, options);
            }
            else if (type == typeof(Quaternion)) {
                Quaternion q = (Quaternion) property.Value;
                Vector4 vec4 = new Vector4(q.x, q.y, q.z, q.w);
                Vector4 vec4Val = EditorGUILayout.Vector4Field(label.text, vec4, options);
                property.Value = new Quaternion(vec4Val.x, vec4Val.y, vec4Val.z, vec4Val.w);
            }
            else if (type == typeof(string)) {
                property.Value = EditorGUILayout.TextField(label, (string) property.Value, options);
            }
            else {
                property.IsExpanded = EditorGUILayout.Foldout(property.IsExpanded, label);
                if (property.IsExpanded) {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < property.ChildCount; i++) {
                        PropertyField(property.ChildAt(i), options);
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }

    }

}