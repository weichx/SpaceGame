using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor.Reflection {

    [PropertyDrawerFor(typeof(AnimationCurve))]
    public class AnimationCurveDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(ReflectedProperty property, GUIContent label) {
            property.Value = EditorGUILayout.CurveField(label, (AnimationCurve) property.Value);        }

    }

}