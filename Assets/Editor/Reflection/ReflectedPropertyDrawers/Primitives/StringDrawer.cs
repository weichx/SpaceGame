using UnityEditor;
using UnityEngine;
using Weichx.ReflectionAttributes;

namespace Weichx.EditorReflection {

    [PropertyDrawerFor(typeof(string))]
    public class StringDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            property.Value = EditorGUI.TextField(position, label, (string) property.Value);
        }

        public override void OnGUILayout(ReflectedProperty property, GUIContent label) {
            property.Value = EditorGUILayout.TextField(label, (string) property.Value);
        }

    }

}