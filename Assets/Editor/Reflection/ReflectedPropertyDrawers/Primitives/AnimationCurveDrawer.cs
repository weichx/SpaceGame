using UnityEditor;
using UnityEngine;

namespace Weichx.EditorReflection {

    [PropertyDrawerFor(typeof(AnimationCurve))]
    public class AnimationCurveDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(Rect rect, ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUI.CurveField(rect, label, (AnimationCurve) property.Value);
        }

        public override void OnGUILayout(ReflectedProperty property, GUIContent label = null) {
            Rect rect = EditorGUILayout.GetControlRect(false, 16f, EditorStyles.colorField);
            OnGUI(rect, property, label);
        }

    }

}