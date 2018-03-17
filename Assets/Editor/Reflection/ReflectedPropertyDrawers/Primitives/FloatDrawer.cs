using UnityEditor;
using UnityEngine;

namespace Weichx.EditorReflection {

    [PropertyDrawerFor(typeof(float))]
    public class FloatDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUILayout.FloatField(label, (float) property.Value);
        }

    }
  

}