using System;
using System.Collections.Generic;
using System.Linq;
using Editor.GUIComponents;
using SpaceGame.Editor.GUIComponents;
using Weichx.Util;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;
using Weichx.ReflectionAttributes;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(ConstructableSubclass), PropertyDrawerForOption.IncludeSubclasses)]
    public class ConstructableSubclassDrawer : ReflectedPropertyDrawer {

        private bool initialized;
        private Type[] subclasses;
        private string[] subclassNames;
        private GUIRect guiRect = new GUIRect(new Rect());

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {
            guiRect.SetRect(position);
            Initialize(property);

            int index = -1;
            int newIndex;

            if (property.Value != null) {
                index = Array.IndexOf(subclasses, property.Type);
            }

            if (index != -1) {
                Rect rect = guiRect.GetFieldRect();
                float width = EditorGUIUtility.labelWidth;
                Rect r1 = new Rect(rect) {
                    width = width
                };
                Rect r2 = new Rect(rect) {x = rect.x + width, width = rect.width - width};
                property.IsExpanded = EditorGUI.Foldout(r1, property.IsExpanded, property.Label);
                newIndex = EditorGUI.Popup(r2, index + 1, subclassNames) - 1;
            }
            else {
                newIndex = EditorGUI.Popup(guiRect.GetFieldRect(), property.Label, index + 1, subclassNames) - 1;
            }

            if (index != newIndex) {
                property.SetValueAndCopyCompatibleProperties(
                    newIndex == -1 ? null : Activator.CreateInstance(subclasses[newIndex])
                );
                property.IsExpanded = newIndex != -1;

            }

            if (property.IsExpanded && newIndex != -1) {
                EditorGUI.indentLevel += 2;
                EditorGUIX.DrawProperties(guiRect.GetRect(), property);
                EditorGUI.indentLevel -= 2;
            }

        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            float height = EditorGUIX.singleLineHeight;
            
            if (property.IsExpanded) {
                height = base.GetPropertyHeight(property);
            }

            return height;
        }

        public void Initialize(ReflectedProperty source) {
            if (!initialized) {
                subclasses = EditorReflector.FindConstructableSubClasses(source.DeclaredType);
                List<string> names = subclasses.ToList().Map((s) => s.Name);
                names.Insert(0, "-- Null --");
                subclassNames = names.ToArray();
                initialized = true;
            }
        }

    }

}