using UnityEditor;
using UnityEngine;

namespace Weichx.EditorReflection {

    [PropertyDrawerFor(typeof(Vector3))]
    public class Vector3Drawer : ReflectedPropertyDrawer {

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUI.Vector3Field(position, label, (Vector3) property.Value);
        }
        
        public override void OnGUILayout(ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUILayout.Vector3Field(label, (Vector3) property.Value);
        }

    }
    
    [PropertyDrawerFor(typeof(Vector3Int))]
    public class Vector3IntDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUI.Vector3IntField(position, label, (Vector3Int) property.Value);
        }
        
        public override void OnGUILayout(ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUILayout.Vector3IntField(label, (Vector3Int) property.Value);
        }

    }

}