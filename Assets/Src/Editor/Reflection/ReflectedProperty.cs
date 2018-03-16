using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SpaceGame.Util;
using UnityEngine;

namespace SpaceGame.Editor.Reflection {

    public abstract class ReflectedProperty {

        public const BindingFlags BindFlags = BindingFlags.Public |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Instance;

        protected GUIContent guiContent;
        protected ReflectedProperty parent;
        protected object actualValue;
        protected object originalValue;
        protected Type actualType;
        protected Type declaredType;
        protected Type originalType;
        protected string label;
        public readonly string name;
        protected bool isExpanded;
        protected List<ReflectedProperty> children;
        protected HashSet<string> changedChildren;
        protected FieldInfo fieldInfo;
        protected bool didChange;
        protected bool isHidden;

        protected ReflectedProperty(ReflectedProperty parent, string name, Type declaredType, object value) {
            this.parent = parent;
            this.name = name;
            this.originalValue = value;
            this.actualValue = value;
            this.declaredType = declaredType;
            this.originalType = value == null ? declaredType : value.GetType();
            this.actualType = originalType;
            this.label = StringUtil.SplitAndTitlize(name);
            this.children = new List<ReflectedProperty>(4);
            this.guiContent = new GUIContent(label);
            this.Drawer = EditorReflector.CreateReflectedPropertyDrawer(actualType);
            this.changedChildren = new HashSet<string>();
            this.isHidden = false;
            if (parent != null) {
                fieldInfo = parent.Type.GetField(name, BindFlags);
                isHidden = fieldInfo?.GetCustomAttributes(typeof(HideInInspector), false).Length != 0;
            }

        }

        public bool IsHidden => isHidden;

        public virtual bool IsExpanded {
            get { return isExpanded; }
            set { isExpanded = value; }
        }

        public ReflectedPropertyDrawer Drawer { get; set; }

        public virtual string Label => DidChange ? label + "*" : label;
        public virtual Type Type => actualType;
        public virtual int ChildCount => children.Count;
        public virtual bool DidChange => actualValue != originalValue;
        public virtual bool IsCircular => false;

        public bool IsArray => actualType.IsArray;
        public virtual FieldInfo FieldInfo => fieldInfo;
        public bool HasModifiedProperties => changedChildren.Count > 0;
        public bool IsBuiltInType => Array.IndexOf(BuiltInTypes, actualType) != -1;
        public bool IsPrimitiveLike => actualType.IsPrimitive || actualType == typeof(string) || actualType.IsEnum;

        public virtual ReflectedProperty ChildAt(int idx) {
            if (children == null || idx < 0 || idx >= children.Count) return null;
            return children[idx];
        }

        public ReflectedProperty this[int indexer] {
            get { return children?[indexer]; }
        }

        public ReflectedProperty this[string indexer] {
            get { return FindProperty(indexer); }
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

//        public void SetSiblingIndex(int index) {
//            if (parent == null || index < 0 || index >= parent.children.Count) {
//                return;
//            }
//            int oldIndex = GetSiblingIndex();
//            Debug.Log("Was: " + oldIndex);
//            parent.children.RemoveAt(oldIndex);
//            if (index > oldIndex) index--;
//            parent.children.Insert(index, this);
//            Debug.Log("Now is: " + GetSiblingIndex());
//        }

        public virtual void OnGUILayout() { }

        public virtual void OnGUI(Rect rect) { }

        public virtual float GetPropertyHeight() {
            return Drawer.GetPropertyHeight(this);
        }

        protected void SetChanged(bool didChange) {
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

        public int GetSiblingIndex() {
            return parent == null ? -1 : parent.children.IndexOf(this);
        }

        public List<ReflectedProperty> GetChildren(List<ReflectedProperty> input = null) {
            input = input ?? new List<ReflectedProperty>(children);
            input.Resize(children.Count);
            for (int i = 0; i < children.Count; i++) {
                input[i] = children[i];
            }
            return input;
        }

        public T GetValue<T>() {
            return (T) actualValue;
        }

        public string stringValue => GetValue<string>();
        public float floatValue => GetValue<float>();
        public int intValue => GetValue<int>();
        public bool boolValue => GetValue<bool>();

        public List<T> GetListValue<T>() {
            return actualValue as List<T>;
        }

        protected void Destroy() {
            DestroyChildren();
            parent?.children.Remove(this);
            this.Drawer = null;
            this.actualValue = null;
            this.children = null;
            this.parent = null;
        }

        protected void DestroyChildren() {
            for (int i = 0; i < children.Count; i++) {
                children[i].Destroy();
            }
        }

        public abstract object Value { get; set; }

        public virtual int ArraySize {
            get { return 0; }
            set { }
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