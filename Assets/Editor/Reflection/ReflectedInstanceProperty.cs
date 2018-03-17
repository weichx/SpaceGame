using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Weichx.Util;

namespace Weichx.EditorReflection {

    public class ReflectedInstanceProperty : ReflectedProperty {

        private ReflectedProperty circularReference;

        public ReflectedInstanceProperty(ReflectedProperty parent, string name, Type declaredType, object value)
            : base(parent, name, declaredType, value) {
            if (actualValue == null) {
                actualValue = CreateValue(declaredType);
                actualType = declaredType;
                originalValue = actualValue;
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
        
        protected override void SetValue(object value) {
            if (value == null) {
                actualType = declaredType;
                actualValue = null;
                this.drawer = EditorReflector.CreateReflectedPropertyDrawer(this);
                DestroyChildren();
            }
            else {
                Type newType = value.GetType();
                if (newType != actualType) {
                    this.drawer = EditorReflector.CreateReflectedPropertyDrawer(this);
                }
                actualType = newType;
                actualValue = value;
                CreateChildren();
            }
            SetChanged(true);
        }
        
        public override void Update() {

            if (parent != null) {
                object parentValue = fieldInfo.GetValue(parent.Value);
                if (parentValue != actualValue) {
                    actualValue = parentValue;
                    actualType = actualValue == null ? declaredType : actualValue.GetType();
                    UpdateChildren();
                }
            }
            
            SetChanged(false);
            
        }

        private void UpdateChildren() {

            FieldInfo[] fieldInfos = EditorReflector.GetFields(actualType);
//            actualType.GetFields(BindFlags);                
            HashSet<ReflectedProperty> childSet = new HashSet<ReflectedProperty>(children);

            for (int i = 0; i < fieldInfos.Length; i++) {
                FieldInfo info = fieldInfos[i];
                if (ShouldReflectProperty(info)) {
                    ReflectedProperty child = children.Find(info, (c, fInfo) => c.name == fInfo.Name);
                    if (child != null) {
                        child.Update();
                        childSet.Remove(child);
                    }
                    else {
                        children.Add(CreateChild(this, info.Name, info.FieldType, info.GetValue(actualValue)));
                    }
                }
            }

            foreach (ReflectedProperty toRemove in childSet) {
                children.Remove(toRemove);
            }
           
            
        }
        
        private void CreateChildren() {
            if (actualValue == null) return;//todo -- bad
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
            if (fi.IsNotSerialized) {
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