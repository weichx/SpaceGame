using System;

namespace SpaceGame.Editor.Reflection {

    public class ReflectedListProperty : ReflectedProperty {

        public ReflectedListProperty(ReflectedProperty parent, string name, Type declaredType, object value)
            : base(parent, name, declaredType, value) { }

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

    }

}