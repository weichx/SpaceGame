using System;

namespace Weichx.ReflectionAttributes {

    public class ConstructableSubclass { }

    public class UsePropertyDrawer : Attribute {

        public readonly Type type;

        public UsePropertyDrawer(Type type) {
            this.type = type;
        }

    }

}