using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor.Reflection {

    [PropertyDrawerFor(typeof(Rect))]
    public class RectDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(ReflectedProperty property, GUIContent label) {
            property.Value = EditorGUILayout.RectField(label, (Rect) property.Value);
        }

    }

}