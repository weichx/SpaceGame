using UnityEditor;
using UnityEngine;

namespace Weichx.EditorReflection {

    [PropertyDrawerFor(typeof(bool))]
    public class BooleanDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(Rect rect, ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUI.Toggle(rect, label, (bool) property.Value);
        }

        public override void OnGUILayout(ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUILayout.Toggle(label, property.boolValue);
        }

    }

}