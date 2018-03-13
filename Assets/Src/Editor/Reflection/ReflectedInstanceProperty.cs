using System;
using System.Reflection;
using UnityEngine;

namespace SpaceGame.Editor.Reflection {

    public class ReflectedInstanceProperty : ReflectedProperty {

        private ReflectedProperty circularReference;

        public ReflectedInstanceProperty(ReflectedProperty parent, string name, Type declaredType, object value)
            : base(parent, name, declaredType, value) {
            if (actualValue == null) {
                actualValue = CreateValue(declaredType);
                actualType = declaredType;
                originalValue = actualValue;
                originalType = declaredType;
            }
            CreateChildren();
        }

        public override bool IsCircular => circularReference != null;
        public override int ChildCount => circularReference != null ? circularReference.ChildCount : children.Count;

        public override ReflectedProperty ChildAt(int index) {
            if (circularReference != null) {
                return circularReference.ChildAt(index);
            }
            return base.ChildAt(index);
        }
        
        public override object Value {
            get {
                if (circularReference != null) {
                    return circularReference.Value;
                }
                return actualValue;
            }
            set {
                if (value == null && actualValue != null) {
                    actualType = declaredType;
                    actualValue = null;
                    DestroyChildren();
                }
                else if (value != null && declaredType.IsInstanceOfType(value)) {
                    actualType = value.GetType();
                    actualValue = value;
                    CreateChildren();
                }
            }
        }

        private void CreateChildren() {
            children.Clear(); // destroy?
            circularReference = CheckCircularReference(this);
            if (circularReference != null) return;
            FieldInfo[] fieldInfos = actualType.GetFields(BindFlags);
            for (int i = 0; i < fieldInfos.Length; i++) {
                FieldInfo fi = fieldInfos[i];
                if (ShouldReflectProperty(fi)) {
                    children.Add(CreateChild(this, fi.Name, fi.FieldType, fi.GetValue(actualValue)));
                }
            }
        }

        private static bool ShouldReflectProperty(FieldInfo fi) {
            if (fi.IsNotSerialized || EditorReflector.HasAttribute(fi, typeof(HideInInspector))) {
                return false;
            }
            return fi.IsPublic || EditorReflector.HasAttribute(fi, typeof(SerializeField));
        }

        private static object CreateValue(Type type) {
            ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor != null && !type.IsAbstract) {
                return Activator.CreateInstance(type);
            }
            return null;
        }

    }

}