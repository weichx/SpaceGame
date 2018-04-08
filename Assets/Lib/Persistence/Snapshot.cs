using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace Weichx.Persistence {

    public partial class Snapshot<T> {

        private Dictionary<object, ReferenceDefinition> referenceMap;
        private Deserializer deserializer;
        private StringSerializer serialzer;
        private int referenceIdGenerator;

        internal class ReferenceDefinition {

            public int refId;
            public TypeDefinition typeDefinition;
            public FieldDefinition[] fields;

        }

        internal class FieldDefinition {

            public int refId;
            public string name;
            public object value;
            public FieldType fieldType;
            public FieldInfo fieldInfo;
            public TypeDefinition typeDefinition;
            public FieldDefinition[] members;

        }

        public Snapshot(T target) {
            referenceMap = new Dictionary<object, ReferenceDefinition>();
            CreateReferenceDefinition(target);
        }

        private ReferenceDefinition CreateReferenceDefinition(object target) {
            ReferenceDefinition refNode = new ReferenceDefinition();
            refNode.refId = referenceIdGenerator++;
            refNode.typeDefinition = TypeDefinition.Get(target.GetType());
            referenceMap[target] = refNode;
            if (refNode.typeDefinition.IsArray) {
                Array array = target as Array;
                Debug.Assert(array != null, nameof(array) + " != null");
                int length = array.Length;
                refNode.fields = new FieldDefinition[length];
                TypeDefinition elementType = refNode.typeDefinition.GetArrayLikeElementType();
                for (int i = 0; i < length; i++) {
                    refNode.fields[i] = CreateArrayMemberDefinition(elementType, array.GetValue(i), i);
                }
            }
            else if (refNode.typeDefinition.IsArrayLike) {
                IList list = target as IList;
                Debug.Assert(list != null, nameof(list) + " != null");

                int length = list.Count;

                refNode.fields = new FieldDefinition[length];
                TypeDefinition elementType = refNode.typeDefinition.GetArrayLikeElementType();
                for (int i = 0; i < length; i++) {
                    refNode.fields[i] = CreateArrayMemberDefinition(elementType, list[i], i);
                }
            }
            else {
                refNode.fields = new FieldDefinition[refNode.typeDefinition.fields.Length];
                FieldInfo[] fields = refNode.typeDefinition.fields;
                for (int i = 0; i < fields.Length; i++) {
                    refNode.fields[i] = CreateFieldDefinition(target, fields[i]);
                }
            }
            return refNode;
        }

        internal enum FieldType {

            Reference,
            Struct,
            Primitive,
            Type,
            Unity,
            Array

        }

        private FieldType GetFieldType(TypeDefinition typeDefinition) {
            if (typeDefinition.IsPrimitiveLike) return FieldType.Primitive;
            if (typeDefinition.IsArray) return FieldType.Array;
            if (typeDefinition.IsStructType) return FieldType.Struct;
            if (typeDefinition.IsUnityType) return FieldType.Unity;
            if (typeDefinition.IsType) return FieldType.Type;
            return FieldType.Reference;
        }

        private FieldDefinition CreateArrayMemberDefinition(TypeDefinition elementType, object value, int index) {
            FieldDefinition fieldDefinition = new FieldDefinition();
            TypeDefinition typeDefinition = GetTypeDefinition(elementType, value);
            fieldDefinition.name = index.ToString();
            fieldDefinition.refId = GetRefId(typeDefinition, value);
            fieldDefinition.value = typeDefinition.IsReferenceType ? null : value;
            fieldDefinition.fieldType = GetFieldType(typeDefinition);
            fieldDefinition.typeDefinition = typeDefinition;

            switch (fieldDefinition.fieldType) {
                case FieldType.Array:
                case FieldType.Reference:
                case FieldType.Primitive:
                case FieldType.Type:
                case FieldType.Unity:
                    break;
                case FieldType.Struct:
                    fieldDefinition.members = CreateFieldMembers(typeDefinition, value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return fieldDefinition;
        }

        private FieldDefinition CreateFieldDefinition(object target, FieldInfo fi) {
            object value = fi.GetValue(target);

            FieldDefinition fieldDefinition = new FieldDefinition();
            TypeDefinition declaredTypeDefinintion = TypeDefinition.Get(fi.FieldType);
            TypeDefinition typeDefinition = GetTypeDefinition(declaredTypeDefinintion, value);

            fieldDefinition.name = fi.Name;
            fieldDefinition.fieldInfo = fi;
            fieldDefinition.refId = GetRefId(typeDefinition, value);
            fieldDefinition.value = typeDefinition.IsReferenceType && !(value is Type) ? null : value;
            fieldDefinition.fieldType = GetFieldType(typeDefinition);
            fieldDefinition.typeDefinition = typeDefinition;

            switch (fieldDefinition.fieldType) {
                case FieldType.Array:
                case FieldType.Reference:
                case FieldType.Primitive:
                case FieldType.Type:
                case FieldType.Unity:
                    break;
                case FieldType.Struct:
                    fieldDefinition.members = CreateFieldMembers(typeDefinition, value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return fieldDefinition;
        }

        private FieldDefinition[] CreateFieldMembers(TypeDefinition typeDefinition, object target) {
            FieldInfo[] fields = typeDefinition.fields;
            FieldDefinition[] fieldDefinitions = new FieldDefinition[fields.Length];
            for (int i = 0; i < fields.Length; i++) {
                fieldDefinitions[i] = CreateFieldDefinition(target, fields[i]);
            }
            return fieldDefinitions;
        }

        private TypeDefinition GetTypeDefinition(TypeDefinition declaredTypeDefinition, object value) {
            if (value == null || declaredTypeDefinition.IsPrimitiveLike) return declaredTypeDefinition;
            return TypeDefinition.Get(value.GetType());
        }

        private int GetRefId(TypeDefinition typeDefinition, object value) {
            if (value == null || value is Type || !typeDefinition.IsReferenceType) return -1;
            ReferenceDefinition referenceDefinition;
            if (referenceMap.TryGetValue(value, out referenceDefinition)) {
                return referenceDefinition.refId;
            }
            return CreateReferenceDefinition(value).refId;
        }

        public string Serialize() {
            return new StringSerializer(referenceMap).ToString();
        }

        public static T Deserialize(string serializedSnapshot) {
            if (string.IsNullOrEmpty(serializedSnapshot)) {
                return DeserializeDefault();
            }
            return FromString(serializedSnapshot).Deserialize();
        }

        public static string Serialize(T target) {
            return new Snapshot<T>(target).Serialize();
        }

        public T Deserialize() {
            if (deserializer == null) {
                deserializer = new Deserializer(referenceMap);
            }
            return deserializer.Deserialize();
        }

        public static Snapshot<T> FromString(string serializedSnapshot) {
            if (string.IsNullOrEmpty(serializedSnapshot)) {
                return DefaultSnapshot();
            }
            return new Snapshot<T>(new StringDeserializer().ObjectFromString(serializedSnapshot));
        }

        public static Snapshot<T> DefaultSnapshot() {
            Type type = typeof(T);
            object target;
            if (type.IsArray) {
                target = Array.CreateInstance(type.GetElementType(), 0);
            }
            else {
                target = MakeInstance(type);
            }
            return new Snapshot<T>((T) target);
        }

        public static string SerializeDefault() {
            Type type = typeof(T);
            object target;
            if (type.IsArray) {
                target = Array.CreateInstance(type.GetElementType(), 0);
            }
            else {
                target = MakeInstance(type);
            }
            return new Snapshot<T>((T) target).Serialize();
        }

        public static T DeserializeDefault() {
            Type type = typeof(T);
            object target;
            if (type.IsArray) {
                target = Array.CreateInstance(type.GetElementType(), 0);
            }
            else {
                target = MakeInstance(type);
            }
            return new Snapshot<T>((T) target).Deserialize();
        }

        private const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        private static object MakeInstance(Type type) {
            if (type == null) return null;
            if (type.GetConstructor(bindFlags, null, Type.EmptyTypes, null) != null) {
                return Activator.CreateInstance(type, true);
            }
            if (type.IsValueType) {
                return Activator.CreateInstance(type, true);
            }
            UnityEngine.Debug.Log($"Unitialized {type.Name}");
            return FormatterServices.GetUninitializedObject(type);
        }

        private static TType MakeInstance<TType>(Type type) {
            if (type == null) return default(TType);
            if (type.GetConstructor(bindFlags, null, Type.EmptyTypes, null) != null) {
                return (TType) Activator.CreateInstance(type, true);
            }
            if (type.IsValueType) {
                return (TType) Activator.CreateInstance(type, true);
            }
            UnityEngine.Debug.Log($"Unitialized {type.Name}");
            return (TType) FormatterServices.GetUninitializedObject(type);
        }

        public static T Clone(T toClone) {
            return new Snapshot<T>(toClone).Deserialize();
        }

    }

}