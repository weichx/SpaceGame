using UnityEditor;

namespace SpaceGame.Editor.Reflection {

    using System;
    using UnityEngine;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class PropertyDrawerFor : Attribute {

        public readonly Type type;

        public PropertyDrawerFor(Type type) {
            this.type = type;
        }

    }

    public abstract class ReflectedPropertyDrawer {

        public abstract void OnGUI(ReflectedProperty property, GUIContent label = null);

        public virtual float GetPropertyHeight(ReflectedProperty property, GUIContent label = null) {
            return EditorGUIUtility.singleLineHeight;
        }

    }

}