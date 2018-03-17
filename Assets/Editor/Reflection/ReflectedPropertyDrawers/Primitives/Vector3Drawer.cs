using UnityEditor;
using UnityEngine;

namespace Weichx.EditorReflection {

    [PropertyDrawerFor(typeof(Vector3))]
    public class Vector3Drawer : ReflectedPropertyDrawer {

        public override void OnGUI(ReflectedProperty property, GUIContent label) {
            property.Value = EditorGUILayout.Vector3Field(label, (Vector3) property.Value);
        }

    }

}