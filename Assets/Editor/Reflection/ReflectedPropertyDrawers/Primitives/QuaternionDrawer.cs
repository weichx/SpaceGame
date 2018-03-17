using UnityEditor;
using UnityEngine;

namespace Weichx.EditorReflection {

    [PropertyDrawerFor(typeof(Quaternion))]
    public class QuaternionDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {
            Quaternion q = (Quaternion) property.Value;
            Vector4 vec4 = new Vector4(q.x, q.y, q.z, q.w);
            Vector4 vec4Val = EditorGUI.Vector4Field(position, label, vec4);
            property.Value = new Quaternion(vec4Val.x, vec4Val.y, vec4Val.z, vec4Val.w);
        }
        
        public override void OnGUILayout(ReflectedProperty property, GUIContent label = null) {
            Quaternion q = (Quaternion) property.Value;
            Vector4 vec4 = new Vector4(q.x, q.y, q.z, q.w);
            Vector4 vec4Val = EditorGUILayout.Vector4Field(label, vec4);
            property.Value = new Quaternion(vec4Val.x, vec4Val.y, vec4Val.z, vec4Val.w);
        }

    }

}