using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor.Reflection {

    [PropertyDrawerFor(typeof(double))]
    public class DoubleDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(ReflectedProperty property, GUIContent label) {
            property.Value = EditorGUILayout.DoubleField(label, (double) property.Value);
        }

    }

}