using SpaceGame.Assets;
using SpaceGame.EditorComponents;
using SpaceGameEditor.Drawers;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace Drawers {

    [PropertyDrawerFor(typeof(BehaviorDefinition))]
    public class BehaviorDefinitionDrawer : ReflectedPropertyDrawerX {

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            guiRect.SetRect(position);
            EditorGUIX.PropertyField(guiRect.GetFieldRect(), property[nameof(BehaviorDefinition.name)]);
            EditorGUIX.PropertyField(guiRect.GetRect(), property[nameof(BehaviorDefinition.considerations)]);
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            float height = EditorGUIUtility.singleLineHeight;
            height += property[nameof(BehaviorDefinition.considerations)].GetPropertyHeight();
            return height;
        }

    }

}