using System;
using System.Collections.Generic;
using System.Reflection;
using SpaceGame.Reflection;
using SpaceGame.Util;
using UnityEngine;

namespace SpaceGame.Editor.Reflection {

    public abstract class ReflectedProperty {

        public const BindingFlags BindFlags = BindingFlags.Public |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Instance;

        public GUIContent guiContent;
        public readonly ReflectedProperty parent;
        protected object actualValue;
        protected object originalValue;
        protected Type actualType;
        protected Type declaredType;
        protected Type originalType;
        protected string label;
        protected bool isExpanded;
        protected List<ReflectedProperty> children;

        protected ReflectedProperty(ReflectedProperty parent, string name, Type declaredType, object value) {
            this.parent = parent;
            this.originalValue = value;
            this.actualValue = value;
            this.declaredType = declaredType;
            this.originalType = value == null ? declaredType : value.GetType();
            this.actualType = originalType;
            this.label = StringUtil.SplitAndTitlize(name);
            this.children = new List<ReflectedProperty>(4);
            this.guiContent = new GUIContent(label);
        }

        public virtual bool IsExpanded {
            get { return isExpanded; }
            set { isExpanded = value; }
        }

        public virtual string Label => label;
        public virtual Type Type => actualType;
        public virtual int ChildCount => children.Count;
        public virtual bool DidChange => actualValue != originalValue;
        public virtual bool IsCircular => false;

        public bool IsArray => actualType.IsArray;
        public bool IsBuiltInType => Array.IndexOf(BuiltInTypes, actualType) != -1;
        public bool IsPrimitiveLike => actualType.IsPrimitive || actualType == typeof(string) || actualType.IsEnum;

        public ReflectedPropertyDrawer x;
        
        public void OnGUI() {
            
        }
        
        public virtual ReflectedProperty ChildAt(int idx) {
            if (children == null || idx < 0 || idx >= children.Count) return null;
            return children[idx];
        }

        public virtual void ApplyChanges() {
            if (!DidChange) return;

        }

        public abstract object Value { get; set; }

        public virtual int ArraySize {
            get { return 0; }
            set { }
        }

        public override string ToString() {
            return Label;
        }

        private static Type[] BuiltInTypes = {
            typeof(Rect),
            typeof(Color),
            typeof(Bounds),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Quaternion),
            typeof(AnimationCurve)
        };

        private static PropertyType GetPropertyType(Type type) {
            if (type.IsPrimitive || type == typeof(string) || type.IsEnum) {
                return PropertyType.Primitive;
            }
            if (type.IsArray) {
                return PropertyType.Array;
            }
            if (type.IsValueType) {
                return PropertyType.Struct;
            }
            if (type == typeof(Type)) {
                return PropertyType.Type;
            }
            if (type.IsSubclassOf(typeof(UnityEngine.Object))) {
                return PropertyType.Unity;
            }

            return PropertyType.Instance;
        }

        protected static ReflectedProperty CreateChild(ReflectedProperty parent, string name, Type type, object value) {
            switch (GetPropertyType(type)) {

                case PropertyType.Primitive:
                    return new ReflectedPrimitiveProperty(parent, name, type, value);
                case PropertyType.Array:
                    return new ReflectedListProperty(parent, name, type, value);
                case PropertyType.Instance:
                    return new ReflectedInstanceProperty(parent, name, type, value);
                case PropertyType.Struct:
                    return new ReflectedInstanceProperty(parent, name, type, value);
                case PropertyType.Unity:
                case PropertyType.Type:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected static ReflectedProperty CheckCircularReference(ReflectedProperty property) {
            if (property.actualValue == null || property.parent == null) {
                return null;
            }
            ReflectedProperty ptr = property.parent;
            while (ptr != null) {
                if (ptr.actualValue == property.actualValue) {
                    return ptr;
                }
                ptr = ptr.parent;
            }
            return null;
        }

    }

}