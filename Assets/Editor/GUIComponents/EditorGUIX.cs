using UnityEngine;
using System;
using System.Collections.Generic;
using Editor.GUIComponents;
using UnityEditor;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.GUIComponents {

    public class EditorGUIX {

        private static GUIContent tempContent = new GUIContent();
        public static float singleLineHeight => EditorGUIUtility.singleLineHeight;
        public static bool wideMode => EditorGUIUtility.wideMode;

        public static Type TypePopup<T>(Rect position, Type currentType) {
            Type[] subclasses = EditorReflector.FindSubClassesWithNull(typeof(T));
            string[] subclassNames = EditorReflector.FindSubClassNamesWithNull(typeof(T));
            int currentIndex = Array.IndexOf(subclasses, currentType);
            return subclasses[EditorGUI.Popup(position, currentIndex, subclassNames)];
        }
        
        public static Type ConstructableTypePopup<T>(Rect position, Type currentType) {
            Type[] subclasses = EditorReflector.FindConstructableSubClassesWithNull(typeof(T));
            string[] subclassNames = EditorReflector.FindConstructableSubClassNamesWithNull(typeof(T));
            int currentIndex = Array.IndexOf(subclasses, currentType);
            return subclasses[EditorGUI.Popup(position, currentIndex, subclassNames)];
        }

        public static Type TypePopup<T>(GUIRect guiRect, GUIContent label, Type currentType) {
            Rect rect = guiRect.GetFieldRect();
            rect = EditorGUI.PrefixLabel(rect, label);
            return TypePopup<T>(rect, currentType);
        }
        
        public static void TypePopup<T>(GUIRect guiRect, GUIContent label, ReflectedProperty property) {
            Rect rect = guiRect.GetFieldRect();
            rect = EditorGUI.PrefixLabel(rect, label);
            TypePopup<T>(rect, property);
        }
        
        public static Type TypePopup<T>(Rect position, GUIContent label, Type currentType) {
            Rect rect = EditorGUI.PrefixLabel(position, label);
            return TypePopup<T>(rect, currentType);
        }
        
        public static Type TypePopup<T>(GUIRect guiRect, Type currentType) {
            return TypePopup<T>(guiRect.GetFieldRect(), currentType);
        }
        
        public static Type ConstructableTypePopup<T>(GUIRect guiRect, Type currentType) {
            return ConstructableTypePopup<T>(guiRect.GetFieldRect(), currentType);
        }
        
        public static void TypePopup<T>(GUIRect guiRect, ReflectedProperty property) {
            Type currentType = (Type) property.Value;
            property.Value = TypePopup<T>(guiRect.GetFieldRect(), currentType);
        }
        
        public static void TypePopup<T>(Rect position, ReflectedProperty property) {
            Type currentType = (Type) property.Value;
            property.Value = TypePopup<T>(position, currentType);
        }
        
        public static void ConstructableTypePopup<T>(GUIRect guiRect, ReflectedProperty property) {
            Type currentType = (Type) property.Value;
            property.Value = ConstructableTypePopup<T>(guiRect.GetFieldRect(), currentType);
        }

        public static void DrawProperties(Rect position, ReflectedObject obj, string[] skipList = null) {
            DrawProperties(position, obj.Root, skipList);
        }

        public static void DrawProperties(Rect position, ReflectedProperty root, string[] skipList = null) {
            List<ReflectedProperty> properties = root.GetChildren();
            float remainingHeight = position.height;
            for (int i = 0; i < properties.Count; i++) {
                ReflectedProperty property = properties[i];
                if ((skipList == null || Array.IndexOf(skipList, property.name) == -1)) {
                    float height = property.Drawer.GetPropertyHeight(property);
                    remainingHeight -= height;
                    position.height = height;
                    Internal_PropertyField(position, property, property.GUIContent, property.IsHidden);
                    position.y += height;
                    position.height = remainingHeight;
                }
            }
        }

        public static bool Foldout(GUIRect rect, ReflectedProperty property) {
            bool isExpanded = EditorGUI.Foldout(rect.GetFieldRect(), property.IsExpanded, property.GUIContent);
            property.IsExpanded = isExpanded;
            return isExpanded;
        }
        
        public static bool Foldout(Rect rect, ReflectedProperty property) {
            bool isExpanded = EditorGUI.Foldout(rect, property.IsExpanded, property.GUIContent);
            property.IsExpanded = isExpanded;
            return isExpanded;
        }
        
        public static void PropertyField(GUIRect rect, ReflectedProperty property) {
            float height = property.Drawer.GetPropertyHeight(property);
            Internal_PropertyField(rect.SliceHeight(height), property, property.GUIContent, false);
        }

        public static void PropertyField(Rect position, ReflectedProperty property) {
            Internal_PropertyField(position, property, property.GUIContent, false);
        }

        public static void PropertyField(Rect position, ReflectedObject obj) {
            Internal_PropertyField(position, obj.Root, obj.Root.GUIContent, false);
        }

        private static void Internal_PropertyField(Rect position, ReflectedProperty property, GUIContent label, bool isHidden) {
            if (isHidden) return;

            if (label == null) label = property.GUIContent;

            Debug.Assert(property.Drawer != null, "property.Drawer != null");

            property.Drawer.OnGUI(position, property, label);

        }

        public static GUIContent TempLabel(string text) {
            tempContent.text = text;
            return tempContent;
        }

        public static bool LabelHasContent(GUIContent label) {
            return label == null || label.text != string.Empty || label.image != null;
        }

//        public static float GetSinglePropertyHeight(Type type, GUIContent label) {
//            float doubleLineHeight = singleLineHeight * 2f;
//            if (type == typeof(Vector2) || type == typeof(Vector3)) {
//                return ((!LabelHasContent(label) || EditorGUIUtility.wideMode ? 0.0f : singleLineHeight) + singleLineHeight);
//            }
//            else if (type == typeof(Rect)) {
//                return ((!LabelHasContent(label) || EditorGUIUtility.wideMode ? 0.0f : singleLineHeight) + doubleLineHeight);
//            }
//            else if (type == typeof(Bounds)) {
//                return ((LabelHasContent(label) ? singleLineHeight : 0.0f) + doubleLineHeight);
//            }
//            else {
//                return singleLineHeight;
//            }
//        }

        public static float GetChildHeights(ReflectedProperty property) {
            float height = 0;
            if (property.ChildCount > 0) {
                for (int i = 0; i < property.ChildCount; i++) {
                    ReflectedProperty child = property.ChildAt(i);
                    height += child.Drawer.GetPropertyHeight(child);
                }
            }
            return height;
        }

    }

}