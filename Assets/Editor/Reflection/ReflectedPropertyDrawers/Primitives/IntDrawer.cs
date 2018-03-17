using UnityEditor;
using UnityEngine;

namespace Weichx.EditorReflection {

    [PropertyDrawerFor(typeof(int))]
    public class IntDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUI.IntField(position, label, (int) property.Value);
        }
        
        public override void OnGUILayout(ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUILayout.IntField(label, (int) property.Value);
        }

    }

}