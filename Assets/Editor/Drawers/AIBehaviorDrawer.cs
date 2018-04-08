using SpaceGame.AI.Behaviors;
using SpaceGame.EditorComponents;
using SpaceGameEditor.Drawers;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace Drawers {

    [PropertyDrawerFor(typeof(AIBehavior))]
    public class AIBehaviorDrawer : ReflectedPropertyDrawerX {

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            guiRect.SetRect(position);
            EditorGUIX.PropertyField(guiRect, property[nameof(AIBehavior.name)]);
            EditorGUIX.PropertyField(guiRect, property[nameof(AIBehavior.considerations)]);
            EditorGUIX.PropertyField(guiRect, property[nameof(AIBehavior.actions)]);
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            float height = EditorGUIUtility.singleLineHeight;
            height += property[nameof(AIBehavior.considerations)].GetPropertyHeight();
            height += property[nameof(AIBehavior.actions)].GetPropertyHeight();
            return height;
        }

    }

}