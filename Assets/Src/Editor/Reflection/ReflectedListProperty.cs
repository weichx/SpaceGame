using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpaceGame.Editor.Reflection {

    public class ReflectedListProperty : ReflectedProperty {

        public ReflectedListProperty(ReflectedProperty parent, string name, Type declaredType, object value)
            : base(parent, name, declaredType, value) { }

        // need to fix this, use original value and 'deserialize into' style assignment
        public override void ApplyChanges() {
            if (Type.IsArray) {
                ResizeArray();
                Array array = (Array) actualValue;
                for (int i = 0; i < children.Count; i++) {
                    array.SetValue(children[i].Value, i);
                }
            }
            else {
                ResizeList();
                IList list = (IList) actualValue;
                for (int i = 0; i < children.Count; i++) {
                    list.Add(children[i].Value);
                }
            }
            originalValue = actualValue;
            actualType = actualValue.GetType();
        }

        public override object Value {
            get { return actualValue; }
            set {
                if (value == null) {
                    actualType = declaredType;
                    actualValue = null;
                }
                else if (declaredType.IsInstanceOfType(value)) {
                    actualType = value.GetType();
                    actualValue = value;
                }
            }
        }

        private void ResizeArray() {
            Array actual = actualValue as Array;
            if (actual != null && actual.Length == children.Count) {
                return;
            }
            actualValue = Array.CreateInstance(declaredType, children.Count);
        }

        private void ResizeList() {
            IList list = actualValue as IList;
            if (list != null && list.Count == children.Count) {
                return;
            }
            list = list ?? Activator.CreateInstance(declaredType) as IList;
            Debug.Assert(list != null, nameof(list) + " != null");
            actualValue = list;
        }

        public void AddArrayElements(IList elements) {
            int childCount = children.Count;
            Type elementType = declaredType.GetElementType() ?? declaredType.GetGenericArguments()[0];
            for (int i = 0; i < elements.Count; i++) {
                children.Add(CreateChild(this, (childCount + i).ToString(), elementType, elements[i]));
            }
        }

        public void AddArrayElement(object element) {
            Type elementType = declaredType.GetElementType() ?? declaredType.GetGenericArguments()[0];
            children.Add(CreateChild(this, children.Count.ToString(), elementType, element));
        }

        public ReflectedProperty Find(Predicate<ReflectedProperty> predicate) {
            return children.Find(predicate);
        }

        public int FindIndex(Predicate<ReflectedProperty> predicate) {
            return children.FindIndex(predicate);
        }

        public void SetChildIndex(int oldIndex, int insertIndex) {
            ReflectedProperty entity = children[oldIndex];
            children.RemoveAt(oldIndex);
            if (insertIndex > oldIndex) insertIndex--;
            children.Insert(insertIndex, entity);
        }

    }

}