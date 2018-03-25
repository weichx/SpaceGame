using System;
using System.Reflection;

namespace Weichx.ReflectionAttributes {

    public class ConstructableSubclass { }

    public class UsePropertyDrawer : Attribute {

        public readonly Type type;
        public readonly string data;

        public UsePropertyDrawer(Type type, string data = "") {
            this.type = type;
            this.data = data;
        }

        public object GetData(object target) {
            if (data.StartsWith("method:")) {
                MethodInfo methodInfo = target.GetType().GetMethod(data, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (methodInfo != null) {
                    return methodInfo.Invoke(target, new object[0]);
                }
            }
            return null;
        }

    }

}