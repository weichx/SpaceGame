using System;
using SpaceGame.Editor.Reflection;
using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor.GUIComponents {

    public static class GUIComponentUtil {

        private static GUIContent tempContent = new GUIContent();

        public static GUIContent TempLabel(string text) {
            tempContent.text = text;
            return tempContent;
        }

        public static bool LabelHasContent(GUIContent label) {
            return label == null || label.text != string.Empty || label.image != null;
        }

        private static float GetSinglePropertyHeight(Type type, GUIContent label) {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float doubleLineHeight = singleLineHeight * 2f;
            if (type == typeof(Vector2) || type == typeof(Vector3)) {
                return ((!LabelHasContent(label) || EditorGUIUtility.wideMode ? 0.0f : singleLineHeight) + singleLineHeight);
            }
            else if (type == typeof(Rect)) {
                return ((!LabelHasContent(label) || EditorGUIUtility.wideMode ? 0.0f : singleLineHeight) + doubleLineHeight);
            }
            else if (type == typeof(Bounds)) {
                return ((LabelHasContent(label) ? singleLineHeight : 0.0f) + doubleLineHeight);
            }
            else {
                return singleLineHeight;
            }
        }

        //todo account for decorators eventually
        public static float GetHeight(ReflectedProperty property, GUIContent label, bool includeChildren) {
            Type drawerType = EditorReflector.GetPropertyDrawerForType(property);
            if (drawerType != null) {
                return ((ReflectedPropertyDrawer)Activator.CreateInstance(drawerType)).GetPropertyHeight(property, label);
            }
            else if (!includeChildren || property.IsPrimitiveLike || property.IsBuiltInType) {
                return GetSinglePropertyHeight(property.Type, label);
            }
            else if (property.IsArray) {
                if (property.IsExpanded) {
                    float height = EditorGUIUtility.singleLineHeight * 2f;
                    for (int i = 0; i < property.ChildCount; i++) {
                        ReflectedProperty child = property.ChildAt(i);
                        height += GetHeight(child, child.GUIContent, child.IsExpanded);
                    }
                    return height;
                }
                return EditorGUIUtility.singleLineHeight;
            }
            else {
                float height = EditorGUIUtility.singleLineHeight;
                for (int i = 0; i < property.ChildCount; i++) {
                    ReflectedProperty child = property.ChildAt(i);
                    height += GetHeight(child, child.GUIContent, child.IsExpanded);
                }
                return height;
            }
        }

    }

}