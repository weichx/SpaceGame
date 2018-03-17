using UnityEditor;
using UnityEngine;

namespace Weichx.EditorReflection {

    [PropertyDrawerFor(typeof(Vector2))]
    public class Vector2Drawer : ReflectedPropertyDrawer {

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUI.Vector2Field(position, label, (Vector2) property.Value);
        }
        
        public override void OnGUILayout(ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUILayout.Vector2Field(label, (Vector2) property.Value);
        }
        
    }
    
    [PropertyDrawerFor(typeof(Vector2Int))]
    public class Vector2IntDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUI.Vector2IntField(position, label, (Vector2Int) property.Value);
        }
        
        public override void OnGUILayout(ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUILayout.Vector2IntField(label, (Vector2Int) property.Value);
        }
        
    }

}