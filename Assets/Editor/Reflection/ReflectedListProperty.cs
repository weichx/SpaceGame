using System;
using System.Collections;
using System.Diagnostics;
using Weichx.Util;

namespace Weichx.EditorReflection {

    public class ReflectedListProperty : ReflectedProperty {

        public ReflectedListProperty(ReflectedProperty parent, string name, Type declaredType, object value)
            : base(parent, name, declaredType, value) {
            if (actualValue == null) {
                actualType = declaredType;
                originalValue = actualValue;
            }
            CreateChildren();
        }

        public override void ApplyChanges() {
            ResizeActualValue();
            IList list = (IList) actualValue;
            for (int i = 0; i < children.Count; i++) {
                children[i].ApplyChanges();
                list[i] = children[i].Value;
            }
            originalValue = actualValue;
            SetChanged(false);
        }

        public virtual int ElementCount {
            get { return children.Count; }
            set {
                if (value < 0) return;
                SetElementCount(value);
            }
        }

        protected virtual void SetElementCount(int size) {
            if (size > children.Count) {
                Type elementType = declaredType.GetGenericArguments()[0];
                //because we know this will be null or a primitive,
                //it is safe to pass the same element to every new index
                object element = EditorReflector.GetDefaultForType(elementType);
                for (int i = children.Count; i < size; i++) {
                    children.Add(CreateChild(this, string.Empty, elementType, element));
                }
                SetChanged(true);
            }
            else if(size < children.Count) {
                while (children.Count > size) {
                   DestroyChild(children.RemoveAndReturnAtIndex(children.Count - 1));
                } 
                SetChanged(true);
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
            SetChanged(true);
        }

        protected virtual void CreateChildren() {
            IList list = (IList) actualValue;
            Type elementType = declaredType.GetGenericArguments()[0];
            for (int i = 0; i < list.Count; i++) {
                object value = list[i];
                children.Add(CreateChild(this, i.ToString(), elementType, value));
            }
        }

        protected virtual void ResizeActualValue() {
            IList list = actualValue as IList;
            if (children.Count == 0 || (list != null && list.Count == children.Count)) {
                return;
            }
            list = list ?? Activator.CreateInstance(declaredType) as IList;
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
            Type elementType = declaredType.GetElementType() ?? declaredType.GetGenericArguments()[0];
            for (int i = 0; i < elements.Count; i++) {
                children.Add(CreateChild(this, string.Empty, elementType, elements[i]));
            }
        }

        public void AddElement(object element) {
            Type elementType = declaredType.GetElementType() ?? declaredType.GetGenericArguments()[0];
            children.Add(CreateChild(this, string.Empty, elementType, element));
        }

        public void InsertElement(int index) {
            if (index >= 0 && index <= ChildCount) {
                Type elementType = declaredType.GetGenericArguments()[0];
                object defaultValue = EditorReflector.GetDefaultForType(declaredType);
                ReflectedProperty child = CreateChild(this, string.Empty, elementType, defaultValue);
                if (index == ChildCount) {
                    AddElement(child);
                }
                children.Insert(index, child);
            }
        }

        public void Duplicate(int index) {
            UnityEngine.Debug.Log("Duplicate not yet implemented");
//            ReflectedProperty child = children[index];
//            if (child.Value == null) {
//            }
//            else {
//                object value = child.Value;
//                object clonedValue;
//                if (child.Type.IsArray) {
//                    clonedValue = Array.CreateInstance(child.Type, ((Array)child.Value).Length);
//                }
//                else {
//                    clonedValue = Activator.CreateInstance(child.Type);
//                }
//                ReflectedProperty clone = CreateChild(this, )
//            }
        }
        
        public virtual void RemoveElementAt(int index) {
            ReflectedProperty child = children.RemoveAndReturnAtIndex(index);
            if (child != null) {
                DestroyChild(child);
            }
        }
        
        public virtual void Clear() {
            DestroyChildren();
        }
        
        public bool MoveElement(int oldIndex, int insertIndex) {
            if (oldIndex < 0 || oldIndex >= children.Count) return false;
            if (insertIndex < 0 || insertIndex >= children.Count) return false;
            ReflectedProperty entity = children[oldIndex];
            children.RemoveAt(oldIndex);
            if (insertIndex > oldIndex) insertIndex--;
            children.Insert(insertIndex, entity);
            return true;
        }
        
        public ReflectedProperty this[int indexer] {
            get { return children?[indexer]; }
        }

        public void RemoveElement(ReflectedProperty selectedDecision) {
            ReflectedProperty child = children?.Find(selectedDecision);
            if (child != null) {
                children.Remove(child);
                DestroyChild(child);
            }
        }

    }

}