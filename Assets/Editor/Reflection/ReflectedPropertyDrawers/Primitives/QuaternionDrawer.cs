using UnityEditor;
using UnityEngine;

namespace Weichx.EditorReflection {

    [PropertyDrawerFor(typeof(Quaternion))]
    public class QuaternionDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(ReflectedProperty property, GUIContent label) {
            Quaternion q = (Quaternion) property.Value;
            Vector4 vec4 = new Vector4(q.x, q.y, q.z, q.w);
            Vector4 vec4Val = EditorGUILayout.Vector4Field(label.text, vec4);
            property.Value = new Quaternion(vec4Val.x, vec4Val.y, vec4Val.z, vec4Val.w);
        }

    }

}