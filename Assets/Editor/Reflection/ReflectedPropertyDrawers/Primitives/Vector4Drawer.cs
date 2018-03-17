using UnityEditor;
using UnityEngine;

namespace Weichx.EditorReflection {

    [PropertyDrawerFor(typeof(Vector4))]
    public class Vector4Drawer : ReflectedPropertyDrawer {

        public override void OnGUI(ReflectedProperty property, GUIContent label) {
            property.Value = EditorGUILayout.Vector4Field(label.text, (Vector4) property.Value);
        }

    }

}