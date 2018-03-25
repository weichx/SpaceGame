using System;
using System.Collections;
using System.Diagnostics;
using JetBrains.Annotations;
using Weichx.ReflectionAttributes;
using Weichx.Util;

namespace Weichx.EditorReflection {

    public class ReflectedListProperty : ReflectedProperty {

        public ReflectedListProperty(ReflectedProperty parent, string name, Type declaredType, object value)
            : base(parent, name, declaredType, value) {
            if (actualValue == null && HasAttribute<CreateOnReflect>()) {
                actualValue = CreateValue(declaredType);
                actualType = actualValue.GetType();
            }
            CreateChildren();
        }

        [PublicAPI]
        public Type ElementType => GetElementType();

        public int ElementCount {
            [DebuggerStepThrough] get { return children.Count; }
            set { SetElementCount(value >= 0 ? value : 0); }
        }

        protected override void ApplyChangesInternal() {
            if (!HasModifiedProperties) {
                return;
            }
            ResizeActualValue();
            IList list = (IList) actualValue;
            for (int i = 0; i < children.Count; i++) {
                ApplyChildChanges(children[i]);
                list[i] = children[i].Value;
            }
            if (parent != null && fieldInfo != null) {
                fieldInfo.SetValue(parent.Value, actualValue);
            }
            changedChildren.Clear();
            isDirty = false;
        }

        protected override void UpdateInternal(object value) {
            object oldValue = actualValue;
            UpdateInternalCommon(value);

            if (actualValue == null) {
                DestroyChildren();
            }
            else if (oldValue == null) {
                CreateChildren();
            }
            else {
                UpdateChildren();
            }
        }

        protected virtual void SetElementCount(int size, bool shouldChange = true) {
            if (size > children.Count) {
                Type elementType = GetElementType();
                //because we know this will be null or a primitive,
                //it is safe to pass the same element to every new index
                //todo maybe make a real instance when we can
                object element = EditorReflector.GetDefaultForType(elementType);
                for (int i = children.Count; i < size; i++) {
                    children.Add(CreateChild(this, string.Empty, elementType, element));
                }
                if (shouldChange) {
                    SetChanged(true);
                }
            }
            else if (size < children.Count) {
                while (children.Count > size) {
                    DestroyChild(children.RemoveAndReturnAtIndex(children.Count - 1));
                }
                if (shouldChange) {
                    SetChanged(true);
                }
            }

        }

        protected override void SetValue(object value) {
            if (value == null) {
                DestroyChildren();
                actualValue = null;
                actualType = declaredType;
            }
            else {
                actualValue = value;
                actualType = value.GetType();
                CreateChildren();
            }
        }

        [DebuggerStepThrough]
        protected virtual Type GetElementType() {
            return declaredType.GetGenericArguments()[0];
        }

        protected virtual void CreateChildren() {
            IList list = (IList) actualValue;
            if (list == null) return;
            Type elementType = GetElementType();
            for (int i = 0; i < list.Count; i++) {
                children.Add(CreateChild(this, string.Empty, elementType, list[i]));
            }
        }

        protected void UpdateChildren() {
            IList list = actualValue as IList;
            Debug.Assert(list != null, nameof(list) + " != null");
            SetElementCount(list.Count, false);
            for (int i = 0; i < list.Count; i++) {
                UpdateChildIntenal(children[i], list[i]);
            }
        }

        protected virtual void ResizeActualValue() {
            IList list = actualValue as IList;
            if (children.Count == 0 || (list != null && list.Count == children.Count)) {
                return;
            }
            list = list ?? EditorReflector.MakeInstance<IList>(declaredType);
            Debug.Assert(list != null, nameof(list) + " != null");
            if (list.Count > children.Count) {
                while (list.Count > children.Count) {
                    list.RemoveAt(list.Count - 1);
                }
            }
            else if (list.Count < children.Count) {
                while (list.Count < children.Count) {
                    list.Add(EditorReflector.GetDefaultForType(declaredType.GetGenericArguments()[0]));
                }
            }
            actualValue = list;
        }

        public ReflectedProperty Find(Predicate<ReflectedProperty> predicate) {
            return children.Find(predicate);
        }

        public int FindIndex(Predicate<ReflectedProperty> predicate) {
            return children.FindIndex(predicate);
        }

        public void AddElements(IList elements) {
            Type elementType = GetElementType();
            for (int i = 0; i < elements.Count; i++) {
                children.Add(CreateChild(this, string.Empty, elementType, elements[i]));
            }
            if (elements.Count > 0) SetChanged(true);
        }

        public void AddElement(object element) {
            Type elementType = GetElementType();
            children.Add(CreateChild(this, string.Empty, elementType, element));
            SetChanged(true);
        }

        public void InsertElement(int index) {
            if (index >= 0 && index <= ChildCount) {
                Type elementType = GetElementType();
                object defaultValue = EditorReflector.GetDefaultForType(declaredType);
                ReflectedProperty child = CreateChild(this, string.Empty, elementType, defaultValue);
                if (index == ChildCount) {
                    AddElement(child);
                    return;
                }
                children.Insert(index, child);
                SetChanged(true);
            }
        }

        public void InsertElement(ReflectedProperty property, int insertIndex) {
            InsertElement(insertIndex);
            children[insertIndex].Value = property?.Value;
        }

        public void Duplicate(int index) {
            UnityEngine.Debug.Log("Duplicate not yet implemented");
        }

        public void RemoveElementAt(int index) {
            ReflectedProperty child = children.RemoveAndReturnAtIndex(index);
            if (child != null) {
                DestroyChild(child);
            }
            SetChanged(true);
        }

        public void Clear() {
            DestroyChildren();
            SetChanged(true);
        }

        public bool MoveElement(int oldIndex, int insertIndex) {
            children.MoveToIndex(oldIndex, insertIndex);
            SetChanged(true);
            return true;
        }

        public bool MoveElement(ReflectedProperty property, int insertIndex) {
            if (property.Parent != this) return false;
            return MoveElement(children.IndexOf(property), insertIndex);
        }

        public void RemoveElement(ReflectedProperty selectedDecision) {
            ReflectedProperty child = children?.Find(selectedDecision);
            if (child != null) {
                children.Remove(child);
                DestroyChild(child);
                SetChanged(true);
            }
        }

        public ReflectedProperty this[int indexer] {
            [DebuggerStepThrough] get { return children?[indexer]; }
        }

    }

}