using System;
using Weichx.Util;

namespace Weichx.EditorReflection {

    public class ReflectedArrayProperty : ReflectedListProperty {

        public ReflectedArrayProperty(ReflectedProperty parent, string name, Type declaredType, object value) : base(parent, name, declaredType, value) {
            if (actualValue == null) {
                actualType = declaredType;
                originalValue = actualValue;
            }
        }
        
        protected override void SetElementCount(int size) {
            if (size > children.Count) {
                Type elementType = declaredType.GetElementType();
                //because we know this will be null or a primitive,
                //it is safe to pass the same element to every new index
                object element = EditorReflector.GetDefaultForType(elementType);
                for (int i = children.Count; i < size; i++) {
                    children.Add(CreateChild(this, i.ToString(), elementType, element));
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
      
        public override void ApplyChanges() {
            if (actualValue == null && children.Count == 0) {
                return;
            }
            ResizeActualValue();
            Array array = (Array) actualValue;
            for (int i = 0; i < children.Count; i++) {
                children[i].ApplyChanges();
                array.SetValue(children[i].Value, i);
            }
       
            originalValue = actualValue;
            actualType = actualValue.GetType();
            SetChanged(false);
        }
     
        protected override void ResizeActualValue() {
            Array actual = (Array) actualValue;
            if (children.Count == 0 || actual != null && actual.Length == children.Count) {
                return;
            }
            actualValue = Array.CreateInstance(declaredType, children.Count);
        }
        
        protected override void CreateChildren() {
            if (actualValue == null) return;
            Array array = (Array) actualValue;
            Type elementType = actualType.GetElementType();
            for(int i = 0; i < array.Length; i++) {
                object value = array.GetValue(i);
                children.Add(CreateChild(this, i.ToString(), elementType, value));
            }
            
        }
    }

}