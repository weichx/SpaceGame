using UnityEditor;
using UnityEngine;

namespace Weichx.EditorReflection {

    [PropertyDrawerFor(typeof(bool))]
    public class BooleanDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(ReflectedProperty property, GUIContent label = null) {
            property.Value = EditorGUILayout.Toggle(label, (bool) property.Value);
        }

    }

}