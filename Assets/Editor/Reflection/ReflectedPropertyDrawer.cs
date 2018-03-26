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

        protected ReflectedProperty property;
        private bool isInitialized;

        private static GUIStyle defaultStyle = new GUIStyle() {
            padding = new RectOffset(4, 4, 4, 4)
        };

        public void SetProperty(ReflectedProperty property) {
            this.property = property;
        }

        public ReflectedListProperty propertyAsList => property as ReflectedListProperty;
            
        public bool IsInitialized => isInitialized;

        public virtual void OnInitialize() { }

        public virtual void OnDestroy() { }

        public abstract void OnGUI(Rect position, ReflectedProperty property, GUIContent label);

        public virtual void OnGUILayout(ReflectedProperty property, GUIContent label) {
            property.Drawer.Initialize();
            Rect position = EditorGUILayout.GetControlRect(label != null, property.Drawer.GetPropertyHeight(property));
            OnGUI(position, property, label);
        }

        public static float GetChildHeights(ReflectedProperty property, string[] skipList) {
            if (skipList == null) return GetChildHeights(property);
            property.Drawer.Initialize();
            float height = 0;
            for (int i = 0; i < property.ChildCount; i++) {
                ReflectedProperty child = property.ChildAt(i);
                if (child.IsHidden) continue;
                int idx = Array.IndexOf(skipList, child.name);
                if (idx == -1) {
                    height += child.GetPropertyHeight();
                }
            }
            return height;
        }

        public static float GetChildHeights(ReflectedProperty property) {
            property.Drawer.Initialize();
            float height = 0;
            for (int i = 0; i < property.ChildCount; i++) {
                ReflectedProperty child = property.ChildAt(i);
                if (!child.IsHidden) {
                    height += child.GetPropertyHeight();
                }
            }
            return height;
        }

        public virtual float GetPropertyHeight(ReflectedProperty property) {
            Initialize();
            if (property.Value == null) return EditorGUIUtility.singleLineHeight;
            return property.IsExpanded ? EditorGUIUtility.singleLineHeight + GetChildHeights(property) : EditorGUIUtility.singleLineHeight;
        }

        public void Initialize() {
            if (isInitialized) return;
            isInitialized = true;
            OnInitialize();
        }

    }

}