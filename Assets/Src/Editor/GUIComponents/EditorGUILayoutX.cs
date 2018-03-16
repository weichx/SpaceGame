using UnityEngine;
using UnityEditor;
using System;
using SpaceGame.Editor.Reflection;

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
            for (int i = 0; i < root.ChildCount; i++) {
                ReflectedProperty property = root[i];
                if ((skipList == null || Array.IndexOf(skipList, property.name) == -1)) {
                    Internal_PropertyField(property, property.GUIContent, property.IsHidden);
                }
            }
        }

        public static void PropertyField(ReflectedProperty property, params GUILayoutOption[] options) {
            Internal_PropertyField(property, property.GUIContent, false, options);
        }

        private static void Internal_PropertyField(ReflectedProperty property, GUIContent label, bool isHidden, params GUILayoutOption[] options) {
            if (isHidden) return;
            Type type = property.Type;

            if (label == null) label = property.GUIContent;

            if (property.Drawer != null) {
                property.Drawer.OnGUI(property, label);
                return;
            }

            Type drawerType = EditorReflector.GetPropertyDrawerForType(property);

            if (drawerType != null) {
                property.Drawer = ((ReflectedPropertyDrawer) Activator.CreateInstance(drawerType));
                property.Drawer.OnGUI(property, label);
                return;
            }

            if (type.IsSubclassOf(typeof(UnityEngine.Object))) {
                property.Value = EditorGUILayout.ObjectField(label, (UnityEngine.Object) property.Value, type, true, options);
            }

            //todo untested
            else if (type.IsArray) {
                property.IsExpanded = EditorGUILayout.Foldout(property.IsExpanded, label.text);
                if (property.IsExpanded) {
                    EditorGUI.indentLevel++;
                    property.ArraySize = EditorGUILayout.IntField(GUIComponentUtil.TempLabel("Size"), property.ArraySize);
                    for (int i = 0; i < property.ArraySize; i++) {
                        ReflectedProperty child = property.ChildAt(i);
                        Internal_PropertyField(child, child.GUIContent, child.IsHidden, options);
                    }
                    EditorGUI.indentLevel--;
                }
            }

            //todo untested
            else {
                property.IsExpanded = EditorGUILayout.Foldout(property.IsExpanded, label);
                if (property.IsExpanded) {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < property.ChildCount; i++) {
                        Internal_PropertyField(property.ChildAt(i), property.GUIContent, property.IsHidden, options);
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }

    }

}