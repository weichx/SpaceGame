using SpaceGame.Editor.Reflection;
using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor.Reflection {

    [PropertyDrawerFor(typeof(float))]
    public class FloatDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(ReflectedProperty property, GUIContent label) {
            property.Value = EditorGUILayout.FloatField(label, (float) property.Value);
        }

    }
  

}