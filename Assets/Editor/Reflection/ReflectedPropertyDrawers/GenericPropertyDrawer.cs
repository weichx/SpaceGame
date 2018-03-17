using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Weichx.EditorReflection {

    public class GenericPropertyDrawer : ReflectedPropertyDrawer {

        private static GUIContent tempContent = new GUIContent();

        public override void OnGUI(ReflectedProperty property, GUIContent label = null) {
            Type type = property.Type;

            // todo -- type / struct
            
            if (type.IsSubclassOf(typeof(UnityEngine.Object))) {
                property.Value = EditorGUILayout.ObjectField(label, (UnityEngine.Object) property.Value, type, true);
            }
            else if (property is ReflectedListProperty) {
                ReflectedListProperty listProperty = (ReflectedListProperty) property;
                listProperty.IsExpanded = EditorGUILayout.Foldout(property.IsExpanded, property.GUIContent);
                if (listProperty.IsExpanded) {
                    EditorGUI.indentLevel++;
                    tempContent.text = "Size";
                    listProperty.ElementCount = EditorGUILayout.IntField(tempContent, listProperty.ElementCount);
                    DrawProperties(property);
                    EditorGUI.indentLevel--;
                }
            }
//            else if (type.IsStruct)
            else if (type.IsClass) {
                property.IsExpanded = EditorGUILayout.Foldout(property.IsExpanded, label);
                if (property.IsExpanded) {
                    EditorGUI.indentLevel++;
                    DrawProperties(property);
                    EditorGUI.indentLevel--;
                }
            }

        }

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

        public static void PropertyField(ReflectedProperty property, params GUILayoutOption[] options) {
            Internal_PropertyField(property, property.GUIContent, false, options);
        }

        private static void Internal_PropertyField(ReflectedProperty property, GUIContent label, bool isHidden, params GUILayoutOption[] options) {
            if (isHidden) return;

            if (label == null) label = property.GUIContent;

            Debug.Assert(property.Drawer != null, "property.Drawer != null");

            property.Drawer.OnGUI(property, label);

        }

    }

}