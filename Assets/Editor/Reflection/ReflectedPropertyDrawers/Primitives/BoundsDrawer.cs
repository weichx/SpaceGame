using UnityEditor;
using UnityEngine;

namespace Weichx.EditorReflection {

    [PropertyDrawerFor(typeof(Bounds))]
    public class BoundsDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUI.BoundsField(position, label, (Bounds) property.Value);
        }
        
        public override void OnGUILayout(ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUILayout.BoundsField(label, (Bounds)property.Value);
        }

    }
    
    [PropertyDrawerFor(typeof(BoundsInt))]
    public class BoundsIntDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUI.BoundsIntField(position, label, (BoundsInt) property.Value);
        }
        
        public override void OnGUILayout(ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUILayout.BoundsIntField(label, (BoundsInt)property.Value);
        }

    }

}