using System;
using Editor.GUIComponents;
using Rotorz.ReorderableList;
using SpaceGame.AI;
using SpaceGame.EditorComponents;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace Editor.Drawers {

    [PropertyDrawerFor(typeof(Goal), PropertyDrawerForOption.IncludeSubclasses)]
    public class GoalDrawer : ReflectedPropertyDrawer {

        private static readonly string[] SkipList = {
            nameof(Goal.name), nameof(Goal.priority), nameof(Goal.considerations)
        };

        private const string NameField = nameof(Goal.name);

        private ReflectedPropertyAdapter adapter;

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            position = EditorGUI.IndentedRect(position);
            GUIRect guiRect = new GUIRect(position);

            string goalName = property.Type.Name.Replace("Goal", "");
            goalName = $"[{ObjectNames.NicifyVariableName(goalName)}]";

            GUIContent content = EditorGUIX.TempLabel(goalName);

            Rect rect = guiRect.GetFieldRect();
            rect.width *= 0.25f;
            EditorGUIX.Foldout(rect, property, content);
            rect.x += rect.width;
            rect.width = rect.width * 3f;

            EditorGUIX.PropertyField(rect, property[NameField], GUIContent.none);

            if (property.IsExpanded) {
                EditorGUIX.DrawProperties(guiRect, property, SkipList);
                ReorderableListGUI.ListFieldAbsolute(EditorGUI.IndentedRect(guiRect.GetRect()), adapter);
            }
        }

        public override void OnInitialize() {
            adapter = new ReflectedPropertyAdapter((ReflectedListProperty) property[nameof(Goal.considerations)]);
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            float height = EditorGUIUtility.singleLineHeight;
            if (property?.Value != null && property.IsExpanded) {
                height += GetChildHeights(property, SkipList);
                height += ReorderableListGUI.CalculateListFieldHeight(adapter);
            }
            return height;
        }

    }

}