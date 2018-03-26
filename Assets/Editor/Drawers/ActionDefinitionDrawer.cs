using SpaceGame.Assets;
using SpaceGame.EditorComponents;
using SpaceGameEditor.Drawers;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace Drawers {

    [PropertyDrawerFor(typeof(ActionDefinition))]
    public class ActionDefinitionDrawer : ReflectedPropertyDrawerX {

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            guiRect.SetRect(position);
            EditorGUIX.PropertyField(guiRect.GetFieldRect(), property[nameof(ActionDefinition.name)]);
            EditorGUIX.PropertyField(guiRect.GetRect(), property[nameof(ActionDefinition.considerations)]);
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            float height = EditorGUIUtility.singleLineHeight;
            height += property[nameof(ActionDefinition.considerations)].GetPropertyHeight();
            return height;
        }

    }

}