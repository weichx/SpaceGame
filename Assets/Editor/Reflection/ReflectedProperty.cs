using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Weichx.Util;
using UnityEngine;
using Weichx.ReflectionAttributes;
using Debug = UnityEngine.Debug;

namespace Weichx.EditorReflection {

    [DebuggerTypeProxy(typeof(ReflectedPropertyDebugProxy))]
    [DebuggerDisplay("{" + nameof(Type) + "}")]
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
        protected List<Attribute> attributes;
        protected List<ReflectedProperty> children;
        protected List<ReflectedProperty> changedChildren;
        protected FieldInfo fieldInfo;
        protected bool isExpanded;
        protected bool isHidden;
        protected bool isDirty;
        private ReflectedPropertyDrawer drawer;

        protected ReflectedProperty(ReflectedProperty parent, string name, Type declaredType, object value) {
            this.parent = parent;
            this.name = name;
            this.originalValue = value;
            this.actualValue = value;
            this.declaredType = declaredType;
            this.actualType = value == null ? declaredType : value.GetType();

            this.label = StringUtil.SplitAndTitlize(name);
            this.children = new List<ReflectedProperty>(4);
            this.guiContent = new GUIContent(label);
            this.changedChildren = new List<ReflectedProperty>();
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
                isHidden = HasAttribute<HideInInspector>();
                isExpanded = HasAttribute<DefaultExpanded>();
            }
            this.drawer = EditorReflector.CreateReflectedPropertyDrawer(this);
        }

        public bool IsExpanded {
            get { return isExpanded; }
            set { isExpanded = value; }
        }

        public T GetValue<T>() {
            return (T) actualValue;
        }

        public bool IsHidden => isHidden;
        public int intValue => GetValue<int>();
        public bool boolValue => GetValue<bool>();
        public float floatValue => GetValue<float>();
        public string stringValue => GetValue<string>();
        public Vector2 vector2Value => GetValue<Vector2>();
        public Vector3 vector3Value => GetValue<Vector3>();
        public Vector4 vector4Value => GetValue<Vector4>();
        public ReflectedPropertyDrawer Drawer => drawer;

        public Type Type => actualType;
        public Type DeclaredType => declaredType;
        public virtual int ChildCount => children.Count;
        public bool DidChange => isDirty;
        public string Label => DidChange ? $"{label}*" : label;
        public ReflectedProperty Parent => parent;

        public bool IsOrphaned => parent == null && name != RootName;
        public bool HasModifiedProperties => isDirty || changedChildren.Count > 0;

        public T GetAttribute<T>() where T : Attribute {
            if (attributes == null) return null;
            for (int i = 0; i < attributes.Count; i++) {
                if (attributes[i].GetType() == typeof(T)) {
                    return (T) attributes[i];
                }
            }
            return null;
        }

        public bool HasAttribute<T>() where T : Attribute {
            return GetAttribute<T>() != null;
        }

        [DebuggerStepThrough]
        public ReflectedProperty ChildAt(int idx) {
            return children == null || (uint) idx >= children.Count ? null : children[idx];
        }

        [DebuggerStepThrough]
        public ReflectedProperty FindProperty(params string[] path) {
            ReflectedProperty ptr = this;
            if (path == null || path.Length == 0) return null;
            for (int i = 0; i < path.Length; i++) {
                ptr = ptr.FindProperty(path[i]);
                if (ptr == null) return null;
            }
            return ptr;
        }

        public ReflectedProperty FindParentOfDeclaredType(Type type) {
            if (parent == null) return null;
            ReflectedProperty ptr = parent;
            while (ptr != null) {
                if (ptr.declaredType == type) {
                    return ptr;
                }
                ptr = ptr.parent;
            }
            return null;
        }

        [DebuggerStepThrough]
        public ReflectedProperty FindProperty(string propertyName) {
            return children?.Find(propertyName, (property, name) => property.name == name);
        }

        public void ApplyChanges() {
            ApplyChangesInternal();
            SetChanged(false);
        }

        protected virtual void ApplyChangesInternal() {
            if (!HasModifiedProperties) return;

            for (int i = 0; i < children.Count; i++) {
                children[i].ApplyChanges();
            }

            if (parent != null && fieldInfo != null) {
                fieldInfo.SetValue(parent.Value, actualValue);
            }

            changedChildren.Clear();
            isDirty = false;
        }

        public void Update() {
            UpdateInternal(originalValue);
        }

        protected virtual void UpdateInternal(object value) {
            if (value == actualValue) return;
            UpdateInternalCommon(value);
            Debug.Assert(declaredType.IsAssignableFrom(actualType));
        }

        protected void UpdateInternalCommon(object value) {
            Type oldType = actualType;
            actualValue = value;
            actualType = actualValue == null ? declaredType : actualValue.GetType();
            originalValue = actualValue;
            isDirty = false;
            changedChildren.Clear();
            if (oldType != actualType) {
                UpdateDrawer();
            }
        }

        protected void UpdateDrawer() {
            drawer.OnDestroy();
            drawer = EditorReflector.CreateReflectedPropertyDrawer(this);
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
        }

        protected void SetChanged(bool didChange) {
            if (isDirty == didChange) return;
            isDirty = didChange;
            ReflectedProperty ptr = this;
            while (ptr?.parent != null) {
                if (didChange) {
                    ptr.parent.changedChildren.Add(ptr);
                }
                else {
                    ptr.parent.changedChildren.Remove(ptr);
                }
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
            this.drawer = null;
            this.actualValue = null;
            this.children = null;
            this.parent = null;
            this.attributes = null;
            this.guiContent = null;
            this.changedChildren = null;
            this.fieldInfo = null;
        }

        //can't access protected method elsewhere so this is a proxy
        protected void UpdateChildIntenal(ReflectedProperty child, object value) {
            child.UpdateInternal(value);
        }

        //can't access protected method elsewhere so this is a proxy
        protected void DestroyChild(ReflectedProperty child) {
            child.Destroy();
        }

        //can't access protected method elsewhere so this is a proxy
        protected void ApplyChildChanges(ReflectedProperty child) {
            child.ApplyChangesInternal();
        }

        protected void DestroyChildren() {
            for (int i = 0; i < children.Count; i++) {
                children[i].Destroy();
            }
            children.Clear();
        }

        public object Value {
            [DebuggerStepThrough]
            get {
                if (IsOrphaned) {
                    throw new Exception("Cannot retrieve values from an orphaned property");
                }
                return actualValue;
            }
            set {
                if (value == actualValue) return;

                if (value != null && !declaredType.IsInstanceOfType(value)) {
                    throw new UnassignableValueException(value.GetType(), declaredType);
                }
                SetValue(value);
                SetChanged(true);
            }
        }

        public GUIContent GUIContent {
            [DebuggerStepThrough]
            get {
                guiContent.text = Label;
                return guiContent;
            }
        }

        public void SetValueAndCopyCompatibleProperties(object newValue) {
            if (newValue == null) {
                Value = null;
                return;
            }

            if (children.Count == 0) {
                Value = newValue;
                return;
            }

            List<ReflectedProperty> oldChildren = new List<ReflectedProperty>(this.children);

            Value = newValue;

            for (int i = 0; i < oldChildren.Count; i++) {
                ReflectedProperty child = FindProperty(oldChildren[i].name);
                if (child != null && child.declaredType.IsAssignableFrom(oldChildren[i].declaredType)) {
                    child.Value = oldChildren[i].actualValue;
                    child.SetChanged(false);
                }
            }
        }

        protected object CreateValue(Type type) {
            if (type.IsArray) {
                return Array.CreateInstance(type.GetElementType(), 0);
            }
            return EditorReflector.MakeInstance(type);
        }

        public override string ToString() {
            return Label;
        }

        public ReflectedProperty this[string indexer] {
            [DebuggerStepThrough] get { return FindProperty(indexer); }
        }

        [DebuggerStepThrough]
        public ReflectedListProperty GetList(string fieldName) {
            return (ReflectedListProperty) FindProperty(fieldName);
        }

        public float GetPropertyHeight() {
            drawer.Initialize();
            return drawer.GetPropertyHeight(this);
        }

        protected static ReflectedProperty CreateChild(ReflectedProperty parent, string name, Type type, object value) {
            if (type.IsPrimitive || type == typeof(string) || type.IsEnum) {
                return new ReflectedPrimitiveProperty(parent, name, type, value);
            }

            if (type.IsArray) {
                return new ReflectedArrayProperty(parent, name, type, value);
            }

            if (typeof(IList).IsAssignableFrom(type)) {
                return new ReflectedListProperty(parent, name, type, value);
            }

            if (type == typeof(Type)) {
                return new ReflectedTypeProperty(parent, name, type, value);
            }

            return new ReflectedInstanceProperty(parent, name, type, value);
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

        private class ReflectedPropertyDebugProxy {

            public string name;
            public List<ReflectedProperty> children;
            public object value;
            public Type declaredType;
            public Type actualType;

            public ReflectedPropertyDebugProxy(ReflectedProperty property) {
                this.name = property.name;
                this.children = property.children;
                this.value = property.actualValue;
                this.declaredType = property.declaredType;
                this.actualType = property.actualType;
            }

        }

        private class UnassignableValueException : Exception {

            public UnassignableValueException(Type valueType, Type expectedType) : base
                ($"Unable to assign value of type type {valueType.Name} to field of type {expectedType.Name}") { }

        }

    }

}