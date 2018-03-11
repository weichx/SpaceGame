using SpaceGame.Editor.Reflection;
using UnityEditor;

namespace SpaceGame.Editor.Reflection {

    using System;
    using UnityEngine;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class PropertyDrawerFor : Attribute {

        public Type type;

        public PropertyDrawerFor(Type type) {
            this.type = type;
        }

    }

    public abstract class ReflectedPropertyDrawer {

        public abstract void OnGUI(ReflectedProperty property, GUIContent label);

        public virtual float GetPropertyHeight(ReflectedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

    }

}