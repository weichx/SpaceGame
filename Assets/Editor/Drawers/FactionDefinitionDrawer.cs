using Editor.GUIComponents;
using SpaceGame;
using SpaceGame.EditorComponents;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(FactionDefinition))]
    public class FactionDefinitionDrawer : ReflectedPropertyDrawer {

        private const string GoalsField = nameof(FactionDefinition.goals);
        private const string NameField = nameof(FactionDefinition.name);

        private GUIRect guiRect = new GUIRect();

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {

            guiRect.SetRect(position);

            float oldLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = 100;
            EditorGUIX.PropertyField(guiRect.GetFieldRect(), property[NameField]);
            EditorGUIUtility.labelWidth = oldLabelWidth;

            EditorGUIX.PropertyField(guiRect, property[GoalsField]);
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            float height = 2f * EditorGUIUtility.singleLineHeight;
            height += property[GoalsField].GetPropertyHeight();
            return height;
        }

    }

}