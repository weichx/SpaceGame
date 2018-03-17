using UnityEditor;
using UnityEngine;

namespace Weichx.EditorReflection {

    [PropertyDrawerFor(typeof(Vector4))]
    public class Vector4Drawer : ReflectedPropertyDrawer {

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUI.Vector4Field(position, label.text, (Vector4) property.Value);
        }
        
        public override void OnGUILayout(ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUILayout.Vector4Field(label.text, (Vector4) property.Value);
        }

    }

}