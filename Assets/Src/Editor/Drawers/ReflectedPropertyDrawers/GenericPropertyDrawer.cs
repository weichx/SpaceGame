using System;
using SpaceGame.Editor.GUIComponents;
using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor.Reflection {

    public class GenericPropertyDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(ReflectedProperty property, GUIContent label = null) {
            Type type = property.Type;
            
            // todo -- type / struct
            
            if (type.IsSubclassOf(typeof(UnityEngine.Object))) {
                property.Value = EditorGUILayout.ObjectField(label, (UnityEngine.Object) property.Value, type, true);
            }
            else if (type.IsArray) {
                property.IsExpanded = EditorGUILayout.Foldout(property.IsExpanded, property.GUIContent);
                if (property.IsExpanded) {
                    EditorGUI.indentLevel++;
                    property.ArraySize = EditorGUILayout.IntField(GUIComponentUtil.TempLabel("Size"), property.ArraySize);
                    for (int i = 0; i < property.ArraySize; i++) {
                        ReflectedProperty child = property.ChildAt(i);
                        child.OnGUILayout();
                    }
                    EditorGUI.indentLevel--;
                }
            }
//            else if (type.IsStruct)
            else if (type.IsClass) {
                property.IsExpanded = EditorGUILayout.Foldout(property.IsExpanded, label);
                if (property.IsExpanded) {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < property.ChildCount; i++) {
                        property.ChildAt(i).OnGUILayout();;
                    }
                    EditorGUI.indentLevel--;
                }
            }
            
        }

    }

}