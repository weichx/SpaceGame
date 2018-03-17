using UnityEditor;
using UnityEngine;

namespace Weichx.EditorReflection {

    [PropertyDrawerFor(typeof(Bounds))]
    public class BoundsDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(ReflectedProperty property, GUIContent label) {
            property.Value = EditorGUILayout.BoundsField(label, (Bounds) property.Value);
        }

    }

}