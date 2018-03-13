using System;

namespace SpaceGame.Reflection {

    public class UsePropertyDrawer : Attribute {

        public readonly Type type;

        public UsePropertyDrawer(Type type) {
            this.type = type;
        }

    }

}