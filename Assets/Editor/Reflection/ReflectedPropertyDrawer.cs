using UnityEditor;

namespace Weichx.EditorReflection {

    using System;
    using UnityEngine;

    public enum PropertyDrawerForOption {

        None,
        IncludeSubclasses

    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class PropertyDrawerFor : Attribute {

        public readonly Type type;
        public readonly PropertyDrawerForOption options;

        public PropertyDrawerFor(Type type, PropertyDrawerForOption option = PropertyDrawerForOption.None) {
            this.type = type;
            this.options = option;
        }

    }

    public abstract class ReflectedPropertyDrawer {

        public abstract void OnGUI(ReflectedProperty property, GUIContent label = null);

        public virtual float GetPropertyHeight(ReflectedProperty property, GUIContent label = null) {
            return EditorGUIUtility.singleLineHeight;
        }

    }

}