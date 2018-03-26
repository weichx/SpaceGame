using SpaceGame.Assets;
using SpaceGame.EditorComponents;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace Drawers {

    [PropertyDrawerFor(typeof(BehaviorSet))]
    public class BehaviorSetDrawer : ReflectedPropertyDrawer{

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            
            EditorGUIX.PropertyField(position, property[nameof(BehaviorSet.name)]);
            
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            return EditorGUIUtility.singleLineHeight;
        }

    }

}