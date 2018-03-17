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

        private static GUIStyle defaultStyle = new GUIStyle() {
            padding = new RectOffset(4, 4, 4, 4)
        };

        public virtual GUIStyle Style => defaultStyle;

        public virtual void OnInitialize() { }

        public virtual void OnDestroy() { }

        public abstract void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null);

        public virtual void OnGUILayout(ReflectedProperty property, GUIContent label = null) {
            Rect position = EditorGUILayout.GetControlRect(label != null, property.Drawer.GetPropertyHeight(property));
            OnGUI(position, property, label);
        }

        public static float GetChildHeights(ReflectedProperty property) {
            float height = 0;
            if (property.ChildCount > 0) {
                for (int i = 0; i < property.ChildCount; i++) {
                    ReflectedProperty child = property.ChildAt(i);
                    height += child.Drawer.GetPropertyHeight(child);
                }
            }
            return height;
        }

        public virtual float GetPropertyHeight(ReflectedProperty property) {
            if (property.Value == null) return EditorGUIUtility.singleLineHeight;
            float x = property.IsExpanded ? EditorGUIUtility.singleLineHeight + GetChildHeights(property) : EditorGUIUtility.singleLineHeight;
            return x;
        }

    }

}