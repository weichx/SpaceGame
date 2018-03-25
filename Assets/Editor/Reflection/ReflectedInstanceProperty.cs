using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using Weichx.ReflectionAttributes;
using Weichx.Util;

namespace Weichx.EditorReflection {

    [DebuggerDisplay("{" + nameof(Type) + "}")]
    public class ReflectedInstanceProperty : ReflectedProperty {

        private ReflectedProperty circularReference;
        
        private List<ReflectedProperty> childSet;

        public ReflectedInstanceProperty(ReflectedProperty parent, string name, Type declaredType, object value)
            : base(parent, name, declaredType, value) {
            if (actualValue == null) {
                actualValue = HasAttribute<CreateOnReflect>() ? CreateValue(declaredType) : null;
                actualType = declaredType;
            }
            CreateChildren();
        }

        protected override void SetValue(object value) {
            if (value == null) {
                actualType = declaredType;
                actualValue = null;
                UpdateDrawer();
                DestroyChildren();
            }
            else {
                Type newType = value.GetType();
                if (newType != actualType) {
                    UpdateDrawer();
                }
                actualType = newType;
                actualValue = value;
                CreateChildren();
            }
        }

        protected override void UpdateInternal(object value) {
            object oldValue = actualValue;
            UpdateInternalCommon(value);

            if (actualValue == null) {
                DestroyChildren();
            }
            else if (oldValue == null) {
                CreateChildren();
            }
            else {
                UpdateChildren();
            }
        }


        private void UpdateChildren() {

            FieldInfo[] fieldInfos = EditorReflector.GetFields(actualType);

            childSet = childSet ?? new List<ReflectedProperty>(8);
            
            for (int i = 0; i < children.Count; i++) {
                childSet.Add(children[i]);
            }

            for (int i = 0; i < fieldInfos.Length; i++) {
                FieldInfo info = fieldInfos[i];
                if (ShouldReflectProperty(info)) {
                    ReflectedProperty child = children.Find(info, (c, fInfo) => c.name == fInfo.Name);
                    if (child != null) {
                        UpdateChildIntenal(child, info.GetValue(actualValue));
                        childSet.Remove(child);
                    }
                    else {
                        children.Add(CreateChild(this, info.Name, info.FieldType, info.GetValue(actualValue)));
                    }
                }
            }

            for (int i = 0; i < childSet.Count; i++) {
                children.Remove(childSet[i]);
            }

            childSet.Clear();
            UnityEngine.Debug.Assert(isDirty == false && changedChildren.Count == 0);
        }

        private void CreateChildren() {
            if (actualValue == null) return;

            DestroyChildren();

            circularReference = CheckCircularReference(this);

            UnityEngine.Debug.Assert(circularReference == null, "circularReference == null");

            FieldInfo[] fieldInfos = EditorReflector.GetFields(actualType);
            for (int i = 0; i < fieldInfos.Length; i++) {
                FieldInfo fi = fieldInfos[i];
                if (ShouldReflectProperty(fi)) {
                    children.Add(CreateChild(this, fi.Name, fi.FieldType, fi.GetValue(actualValue)));
                }
            }
        }

        private static bool ShouldReflectProperty(FieldInfo fi) {
            return !fi.IsNotSerialized && (fi.IsPublic || EditorReflector.HasAttribute(fi, typeof(SerializeField)));
        }

    }

}