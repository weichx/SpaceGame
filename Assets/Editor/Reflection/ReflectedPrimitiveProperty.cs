using System;

namespace Weichx.EditorReflection {

    public class ReflectedPrimitiveProperty : ReflectedProperty {

        public ReflectedPrimitiveProperty(ReflectedProperty parent, string name, Type declaredType, object value)
            : base(parent, name, declaredType, value) { }

    }

}