using UnityEditor;
using UnityEngine;

namespace Weichx.EditorReflection {

    [PropertyDrawerFor(typeof(float))]
    public class FloatDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUI.FloatField(position, label, (float) property.Value);
        }
        
        public override void OnGUILayout(ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUILayout.FloatField(label, (float) property.Value);
        }


    }
  

}