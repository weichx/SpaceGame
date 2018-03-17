using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.GUIComponents {

    public static class EditorGUILayoutX {

        public static void DrawProperties(ReflectedObject root, string[] skipList = null) {
            for (int i = 0; i < root.ChildCount; i++) {
                ReflectedProperty property = root.GetChildAt(i);
                if ((skipList == null || Array.IndexOf(skipList, property.name) == -1)) {
                    Internal_PropertyField(property, property.GUIContent, property.IsHidden);
                }
            }
        }

        public static void DrawProperties(ReflectedProperty root, string[] skipList = null) {
            List<ReflectedProperty> properties = root.GetChildren();
            for (int i = 0; i < properties.Count; i++) {
                ReflectedProperty property = properties[i];
                if ((skipList == null || Array.IndexOf(skipList, property.name) == -1)) {
                    Internal_PropertyField(property, property.GUIContent, property.IsHidden);
                }
            }
        }

        public static void PropertyField(ReflectedProperty property) {
            Internal_PropertyField(property, property.GUIContent, false);
        }
        
        public static void PropertyField(ReflectedObject obj) {
            Rect rect = EditorGUILayout.GetControlRect(true, obj.Root.Drawer.GetPropertyHeight(obj.Root));
            EditorGUIX.PropertyField(rect, obj.Root);
        }

        private static void Internal_PropertyField(ReflectedProperty property, GUIContent label, bool isHidden) {
            if (isHidden) return;
            
            if (label == null) label = property.GUIContent;

            Debug.Assert(property.Drawer != null, "property.Drawer != null");
            
            property.Drawer.OnGUILayout(property, label);

        }

    }

}