using SpaceGame.AI;
using SpaceGame.Assets;
using SpaceGame.EditorComponents;
using SpaceGameEditor.Drawers;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;
using Weichx.Util;

namespace Drawers {

    [PropertyDrawerFor(typeof(AIAction), PropertyDrawerForOption.IncludeSubclasses)]
    public class ActionDefinitionDrawer : ReflectedPropertyDrawerX {

        private const string NameField = nameof(AIAction.name);

        private static readonly string[] SkipList = {
            nameof(AIAction.name), nameof(AIAction.considerations)
        };

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            guiRect.SetRect(position);

            string typeName = $"[{StringUtil.NicifyName(property.Type.Name, "Action")}]";
            GUIContent content = EditorGUIX.TempLabel(typeName);

            Rect rect = guiRect.GetFieldRect();
            rect.width *= 0.25f;
            EditorGUIX.Foldout(rect, property, content);
            rect.x += rect.width;
            rect.width = rect.width * 3f;

            EditorGUIX.PropertyField(rect, property[NameField], GUIContent.none);

            if (property.IsExpanded) {
                EditorGUIX.DrawProperties(guiRect, property, SkipList);
                EditorGUIX.PropertyField(guiRect, property[nameof(AIAction.considerations)]);
            }

        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            float height = EditorGUIUtility.singleLineHeight;
            if (property?.Value != null && property.IsExpanded) {
                height += GetChildHeights(property, SkipList);
                height += property[nameof(AIAction.considerations)].GetPropertyHeight();
            }
            return height;
        }

    }

}