using System;

namespace SpaceGame.Editor.Reflection {

    public class ReflectedPrimitiveProperty : ReflectedProperty {

        public ReflectedPrimitiveProperty(ReflectedProperty parent, string name, Type declaredType, object value)
            : base(parent, name, declaredType, value) { }

        public override object Value {
            get { return actualValue; }
            set {
                if (value == null && actualValue != null) {
                    actualType = declaredType;
                    actualValue = EditorReflector.GetDefaultForType(declaredType);
                }
                //todo for primitives types must be convertable
                else if (value != null && declaredType.IsInstanceOfType(value)) {
                    actualType = value.GetType();
                    actualValue = value;
                }
            }
        }

    }

}