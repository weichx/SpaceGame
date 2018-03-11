using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor.Reflection {

    [PropertyDrawerFor(typeof(string))]
    public class StringDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(ReflectedProperty property, GUIContent label) {
            property.Value = EditorGUILayout.TextField(label, (string) property.Value);
        }

    }

}