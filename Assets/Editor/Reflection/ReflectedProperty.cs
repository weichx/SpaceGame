using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Weichx.Util;
using UnityEngine;

namespace Weichx.EditorReflection {

    public abstract class ReflectedProperty {

        protected const BindingFlags BindFlags = BindingFlags.Public |
                                                 BindingFlags.NonPublic |
                                                 BindingFlags.Instance;

        internal const string RootName = "--Root--";

        public readonly string name;

        protected GUIContent guiContent;
        protected ReflectedProperty parent;
        protected object actualValue;
        protected object originalValue;
        protected Type actualType;
        protected Type declaredType;
        protected string label;
        protected List<ReflectedProperty> children;
        protected HashSet<string> changedChildren;
        protected FieldInfo fieldInfo;
        protected bool isExpanded;
        protected bool isHidden;
        protected bool isDirty;
        protected List<Attribute>attributes;
        protected ReflectedPropertyDrawer drawer;
        
        protected ReflectedProperty(ReflectedProperty parent, string name, Type declaredType, object value) {
            this.parent = parent;
            this.name = name;
            this.originalValue = value;
            this.actualValue = value;
            this.declaredType = declaredType;
            this.actualType = value == null ? declaredType : value.GetType();
            ;
            this.label = StringUtil.SplitAndTitlize(name);
            this.children = new List<ReflectedProperty>(4);
            this.guiContent = new GUIContent(label);
            this.changedChildren = new HashSet<string>();
            this.isHidden = false;
            this.isDirty = false;
            if (parent != null) {
                fieldInfo = parent.Type.GetField(name, BindFlags);
                object[] attrs = fieldInfo?.GetCustomAttributes(false);
                attributes = new List<Attribute>();
                if (attrs != null) {
                    for (int i = 0; i < attrs.Length; i++) {
                        attributes.Add((Attribute) attrs[i]);
                    }
                }
                isHidden = fieldInfo?.GetCustomAttributes(typeof(HideInInspector), false).Length != 0;
            }
            this.drawer = EditorReflector.CreateReflectedPropertyDrawer(this);
        }

        public bool IsHidden => isHidden;

        public virtual bool IsExpanded {
            get { return isExpanded; }
            set { isExpanded = value; }
        }


        public T GetValue<T>() {
            return (T) actualValue;
        }

        public int intValue => GetValue<int>();
        public bool boolValue => GetValue<bool>();
        public float floatValue => GetValue<float>();
        public string stringValue => GetValue<string>();
        public Vector2 vector2Value => GetValue<Vector2>();
        public Vector3 vector3Value => GetValue<Vector3>();
        public Vector4 vector4Value => GetValue<Vector4>();

        public List<T> GetListValue<T>() {
            return actualValue as List<T>;
        }

        public virtual bool IsCircular => false;
        public Type Type => actualType;
        public Type DeclaredType => declaredType;
        public ReflectedPropertyDrawer Drawer => drawer;
        public virtual int ChildCount => children.Count;
        public virtual bool DidChange => actualValue != originalValue;
        public virtual string Label => DidChange ? label + "*" : label;

        public bool IsOrphaned => parent == null && name != RootName;
        public bool HasModifiedProperties => changedChildren.Count > 0;
        public bool IsBuiltInType => Array.IndexOf(BuiltInTypes, actualType) != -1;
        public bool IsPrimitiveLike => actualType.IsPrimitive || actualType == typeof(string) || actualType.IsEnum;

    
        public T GetAttribute<T>() where T : Attribute {
            if (attributes == null) return null;
            for (int i = 0; i < attributes.Count; i++) {
                if (attributes[i].GetType() == typeof(T)) {
                    return (T) attributes[i];
                }
            }
            return null;
        }

        public virtual ReflectedProperty ChildAt(int idx) {
            if (children == null || idx < 0 || idx >= children.Count) return null;
            return children[idx];
        }

        public ReflectedProperty FindProperty(params string[] path) {
            ReflectedProperty ptr = this;
            if (path == null || path.Length == 0) return null;
            for (int i = 0; i < path.Length; i++) {
                ptr = ptr.FindProperty(path[i]);
                if (ptr == null) return null;
            }
            return ptr;
        }

        public ReflectedProperty FindProperty(string propertyName) {
            return children?.Find(propertyName, (property, name) => property.name == name);
        }

        // todo this isn't cached at all right now
        public virtual void ApplyChanges() {
            if (children != null) {
                for (int i = 0; i < children.Count; i++) {
                    children[i].ApplyChanges();
                }
            }
            if (parent != null && fieldInfo != null) {
                fieldInfo.SetValue(parent.Value, Value);
            }
            originalValue = actualValue;
            SetChanged(false);
        }

        public virtual void Update() {
            if (fieldInfo != null) {
                Value = fieldInfo.GetValue(parent.Value);
            }
            SetChanged(false);
        }

        protected virtual void SetValue(object value) {
            Type previousType = actualType;
            if (value == null) {
                actualType = declaredType;
                actualValue = EditorReflector.GetDefaultForType(declaredType);
            }
            else {
                actualType = value.GetType();
                actualValue = value;
            }
            if (actualType != previousType) {
                this.drawer = EditorReflector.CreateReflectedPropertyDrawer(this);
            }
            SetChanged(true);
        }

        protected void SetChanged(bool didChange) {
            if (didChange && isDirty) return;
            isDirty = didChange;
            ReflectedProperty ptr = parent;
            string childName = name;
            while (ptr != null && parent != null) {
                if (didChange) {
                    parent.changedChildren.Add(childName);
                }
                else {
                    parent.changedChildren.Remove(childName);
                }
                childName = parent.name;
                ptr = ptr.parent;
            }

        }

        public List<ReflectedProperty> GetChildren(List<ReflectedProperty> input = null) {
            input = input ?? new List<ReflectedProperty>(children);
            input.Resize(children.Count);
            for (int i = 0; i < children.Count; i++) {
                input[i] = children[i];
            }
            return input;
        }

        protected void Destroy() {
            DestroyChildren();
            parent?.children.Remove(this);
            this.drawer = null;
            this.actualValue = null;
            this.children = null;
            this.parent = null;
        }

        protected void DestroyChildren() {
            for (int i = 0; i < children.Count; i++) {
                children[i].Destroy();
            }
        }

        public object Value {
            get { return actualValue; }
            set {
                if (value == actualValue) return;

                if (value != null && !declaredType.IsInstanceOfType(value)) {
                    throw new UnassignableValueException(value.GetType(), declaredType);
                }
                SetValue(value);
            }
        }

        public GUIContent GUIContent {
            get {
                guiContent.text = Label;
                return guiContent;
            }
        }

        public override string ToString() {
            return Label;
        }

        public ReflectedProperty this[string indexer] {
            get { return FindProperty(indexer); }
        }

        private static Type[] BuiltInTypes = {
            typeof(Rect),
            typeof(Color),
            typeof(Bounds),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Quaternion),
            typeof(AnimationCurve),
            typeof(RectInt),
            typeof(BoundsInt),
            typeof(Vector2Int),
            typeof(Vector3Int)
        };

        private static PropertyType GetPropertyType(Type type) {
            if (type.IsPrimitive || type == typeof(string) || type.IsEnum) {
                return PropertyType.Primitive;
            }
            if (type.IsArray || typeof(IList).IsAssignableFrom(type)) {
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
                    return new ReflectedInstanceProperty(parent, name, type, value);
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

        protected void DestroyChild(ReflectedProperty child) {
            Debug.Assert(child.parent == this, "child.parent == this");
            child.parent = null;

        }

        internal class UnassignableValueException : Exception {

            public UnassignableValueException(Type valueType, Type expectedType) : base
                ($"Unable to assign value of type type {valueType.Name} to field of type {expectedType.Name}") { }

        }

    }

}