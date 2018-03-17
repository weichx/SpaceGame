using System;
using System.Collections;
using JetBrains.Annotations;

namespace Weichx.EditorReflection {

    public class ReflectedObject {

        private ReflectedProperty[] reflectedProperties;
        private ReflectedProperty root;

        public ReflectedObject([NotNull] object target) {

            Type type = target.GetType();
            if (type.IsArray) {
                root = new ReflectedArrayProperty(null, "--Root--", type, target);
            }
            else if (typeof(IList).IsAssignableFrom(type)) {
                root = new ReflectedListProperty(null, "--Root--", type, target);
            }
            else if (type.IsPrimitive || type == typeof(string) || type.IsEnum) {
                root = new ReflectedPrimitiveProperty(null, "--Root--", type, target);
            }
            else {
                root = new ReflectedInstanceProperty(null, "--Root--", target.GetType(), target);
            }
        }

        public ReflectedProperty Root => root;
        
        public void ApplyModifiedProperties() {
            root.ApplyChanges();
        }

        public ReflectedProperty GetChildAt(int idx) {
            return root.ChildAt(idx);
        }

        public int ChildCount {
            get { return root.ChildCount; }
        }

        public bool HasModifiedProperties => root.HasModifiedProperties;
        public object Value => root.Value;

        public ReflectedProperty FindChild(string childName) {
            return root.FindProperty(childName);
        }

        public ReflectedProperty this[string childName] {
            get { return root.FindProperty(childName); }
        }

        public ReflectedProperty FindProperty(params string[] path) {
            return root.FindProperty(path);
        }

    }

}