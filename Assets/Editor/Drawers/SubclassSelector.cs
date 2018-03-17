using System;
using System.Collections.Generic;
using System.Linq;
using SpaceGame.AI;
using SpaceGame.Editor.GUIComponents;
using Weichx.Util;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace Src.Editor.Drawers {

    [PropertyDrawerFor(typeof(ContextCreator), PropertyDrawerForOption.IncludeSubclasses)]
    public class SubclassSelector : ReflectedPropertyDrawer {

        private bool initialized;
        private List<Type> subclasses;
        private string[] subclassNames;

        public override void OnGUI(ReflectedProperty property, GUIContent label = null) {

            Initialize(property);
            
            int index = -1;
            int newIndex;
            
            if (property.Value != null) {
                index = subclasses.IndexOf(property.Type);
            }

            EditorGUILayout.BeginHorizontal();

            if (index != -1) {
                property.IsExpanded = EditorGUILayout.Foldout(property.IsExpanded, label);
                newIndex = EditorGUILayout.Popup(index + 1, subclassNames) - 1;
            }
            else {
                newIndex = EditorGUILayout.Popup(property.Label, index + 1, subclassNames) - 1;
            }
            
            if (index != newIndex) {
                property.Value = newIndex == -1 ? null : Activator.CreateInstance(subclasses[newIndex]);
                property.IsExpanded = newIndex != -1;
            }

            EditorGUILayout.EndHorizontal();

            if (property.IsExpanded && newIndex != -1) {
                EditorGUI.indentLevel += 2;
                EditorGUILayoutX.DrawProperties(property);
                EditorGUI.indentLevel -= 2;
            }

        }

        private void Initialize(ReflectedProperty source) {
            if (!initialized) {
                subclasses = EditorReflector.FindConstructableSubClasses(source.DeclaredType).ToList();
                List<string> names = subclasses.ToList().Map((s) => s.Name);
                names.Insert(0, "null");
                subclassNames = names.ToArray();
                initialized = true;
            }
        }

    }

}