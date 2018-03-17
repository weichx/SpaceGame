using UnityEditor;
using UnityEngine;

namespace Weichx.EditorReflection {

    [PropertyDrawerFor(typeof(AnimationCurve))]
    public class AnimationCurveDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUILayout.CurveField(label, (AnimationCurve) property.Value);
        }

    }

}