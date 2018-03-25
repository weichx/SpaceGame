using System;

namespace Weichx.EditorReflection {

    public class ReflectedArrayProperty : ReflectedListProperty {

        public ReflectedArrayProperty(ReflectedProperty parent, string name, Type declaredType, object value)
            : base(parent, name, declaredType, value) {}

        protected override void ResizeActualValue() {
            Array actual = (Array) actualValue;
            if (children.Count == 0 || actual != null && actual.Length == children.Count) {
                return;
            }
            actualValue = Array.CreateInstance(declaredType.GetElementType(), children.Count);
        }

        protected override Type GetElementType() {
            return declaredType.GetElementType();
        }
        
    }

}