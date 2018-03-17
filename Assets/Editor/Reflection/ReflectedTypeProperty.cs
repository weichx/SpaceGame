using System;

namespace Weichx.EditorReflection {

    public class ReflectedTypeProperty : ReflectedProperty {

        public ReflectedTypeProperty(ReflectedProperty parent, string name, Type declaredType, object value)
            : base(parent, name, declaredType, value) {
            
        }

    }

}