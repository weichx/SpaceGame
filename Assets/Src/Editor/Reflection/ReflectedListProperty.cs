using System;
using System.Collections;

namespace SpaceGame.Editor.Reflection {

    public class ReflectedListProperty : ReflectedProperty {

        public ReflectedListProperty(ReflectedProperty parent, string name, Type declaredType, object value)
            : base(parent, name, declaredType, value) { }

        public override void ApplyChanges() {
            IList parentArray = parent?.Value as IList;
            if (parentArray != null) {
                parentArray[int.Parse(name)] = Value;
            }
            if (children != null) {
                for (int i = 0; i < children.Count; i++) {
                    children[i].ApplyChanges();
                }
            }
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

    }

}