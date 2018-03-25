using System;
using System.Diagnostics;

namespace Weichx.EditorReflection {

    [DebuggerDisplay("{" + nameof(name) + "}")]
    public class ReflectedPrimitiveProperty : ReflectedProperty {

        public ReflectedPrimitiveProperty(ReflectedProperty parent, string name, Type declaredType, object value)
            : base(parent, name, declaredType, value) { }

    }

}