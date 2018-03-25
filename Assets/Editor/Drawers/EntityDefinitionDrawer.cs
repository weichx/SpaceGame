using Editor.GUIComponents;
using SpaceGame;
using SpaceGame.EditorComponents;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(EntityDefinition))]
    public class EntityDefinitionDrawer : ReflectedPropertyDrawer{

        private static readonly GUIRect s_guiRect = new GUIRect();
        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            s_guiRect.SetRect(position);
            EditorGUIX.PropertyField(s_guiRect, property["name"]);
            EditorGUIX.PropertyField(s_guiRect, property["shipType"]);
            EditorGUIX.PropertyField(s_guiRect, property["goals"]);
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            float height = 0f;
            height += EditorGUIX.GetChildHeights(property["name"]);
            height += EditorGUIX.GetChildHeights(property["shipType"]);
            height += EditorGUIX.GetChildHeights(property["goals"]);
            return height;
        }

    }

}