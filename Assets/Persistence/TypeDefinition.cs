namespace Weichx.Persistence {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using UnityEngine;

    [DebuggerDisplay("type = {" + nameof(typeName) + "}")]
    public class TypeDefinition {

        public Type type;
        public TypeValue typeValue;
        public readonly string typeName;
        public readonly Type genericBase;
        public readonly Type[] genericArguments;
        public readonly FieldInfo[] fields;

        public TypeDefinition(Type type) {
            this.type = type;
            this.typeValue = GetTypeValue(type);
            this.typeName = GetTypeName(typeValue, type.FullName);
            genericBase = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            genericArguments = type.IsGenericType ? type.GetGenericArguments() : null;
            fields = GetFields(this);
        }

        public bool HasFields => !IsPrimitiveLike && !IsType;

        public bool IsGeneric => genericBase != null;

        public int GenericArgumentCount => genericArguments?.Length ?? 0;

        public bool IsType => typeValue == TypeValue.Type;

        public bool IsPrimitive => (typeValue & TypeValue.Primitive) != 0;

        public bool IsPrimitiveLike => (typeValue & TypeValue.PrimitiveLike) != 0;

        public bool IsSimpleKnownType => (typeValue & TypeValue.SimpleKnownType) != 0;

        public bool IsEnumType => typeValue == TypeValue.Enum;

        public bool IsCollection => (typeValue & TypeValue.Collection) != 0;

        public bool IsValueType => type.IsValueType;

        public bool IsStructType => (typeValue & TypeValue.StructLike) != 0;

        public bool IsUnityType => (typeValue & TypeValue.UnityObject) != 0;

        public bool IsArray => (typeValue & TypeValue.Array) != 0;

        public bool IsArrayLike => (typeValue & TypeValue.ArrayLike) != 0;

        public bool IsReferenceType => type.IsClass && type != typeof(Type) && type != typeof(string);

        public bool IsKnownType {
            // not sure that this should include collections/generics
            get { return typeValue != TypeValue.Type && (typeValue & TypeValue.KnownType) != 0; }
        }

        public TypeDefinition GetArrayLikeElementType() {
            if (!IsArrayLike) return null;

            if (genericArguments != null && genericArguments.Length == 1) {
                return Get(genericArguments[0]);
            }
            else if (type.IsArray) {
                return Get(type.GetElementType());
            }
            else {
                return null;
            }

        }

        private static readonly Dictionary<Type, TypeDefinition> typeDefinitions = new Dictionary<Type, TypeDefinition>();

        public static TypeDefinition Get(Type type) {
            if (typeDefinitions.ContainsKey(type)) return typeDefinitions[type];
            TypeDefinition typeDefinition = new TypeDefinition(type);

            typeDefinitions.Add(type, typeDefinition);
            return typeDefinition;
        }

        private const BindingFlags FieldBindFlags = BindingFlags.Public |
                                                    BindingFlags.Default |
                                                    BindingFlags.Instance |
                                                    BindingFlags.NonPublic;

        private static List<FieldInfo> fieldInfoContainer = new List<FieldInfo>(16);

        private static FieldInfo[] GetFields(TypeDefinition typeDefinition) {
            if (typeDefinition.IsType || typeDefinition.IsPrimitiveLike || typeDefinition.IsArray) {
                return new FieldInfo[0];
            }
            fieldInfoContainer.Clear();
            FieldInfo[] fieldInfos = typeDefinition.type.GetFields(FieldBindFlags);

            for (int i = 0; i < fieldInfos.Length; i++) {
                FieldInfo fieldInfo = fieldInfos[i];
                if (!fieldInfo.IsNotSerialized) {
                    fieldInfoContainer.Add(fieldInfo);
                }
            }
            return fieldInfoContainer.ToArray();
        }

        public static TypeValue GetTypeValue(Type type) {

            if (type.IsPrimitive) {
                if (type == typeof(float)) {
                    return TypeValue.Float;
                }

                if (type == typeof(int)) {
                    return TypeValue.Integer;
                }

                if (type == typeof(uint)) {
                    return TypeValue.UnsignedInteger;
                }

                if (type == typeof(bool)) {
                    return TypeValue.Boolean;
                }

                if (type == typeof(short)) {
                    return TypeValue.Short;
                }

                if (type == typeof(ushort)) {
                    return TypeValue.UnsignedShort;
                }

                if (type == typeof(double)) {
                    return TypeValue.Double;
                }

                if (type == typeof(long)) {
                    return TypeValue.Long;
                }

                if (type == typeof(ulong)) {
                    return TypeValue.UnsignedLong;
                }

                if (type == typeof(char)) {
                    return TypeValue.Char;
                }

                if (type == typeof(byte)) {
                    return TypeValue.Byte;
                }

                if (type == typeof(sbyte)) {
                    return TypeValue.SignedByte;
                }

                if (type == typeof(IntPtr)) {
                    return TypeValue.Null; // unsupported
                }

                if (type == typeof(UIntPtr)) {
                    return TypeValue.Null; // unsupported
                }

                return TypeValue.Unknown;

            }

            if (typeof(Type).IsAssignableFrom(type)) {
                return TypeValue.Type;
            }

            if (type == typeof(Vector2)) {
                return TypeValue.Vector2;
            }

            if (type == typeof(Vector3)) {
                return TypeValue.Vector3;
            }

            if (type == typeof(Vector4)) {
                return TypeValue.Vector4;
            }

            if (type == typeof(Quaternion)) {
                return TypeValue.Quaternion;
            }

            if (type == typeof(Color) || type == typeof(Color32)) {
                return TypeValue.Color;
            }

            if (type == typeof(decimal)) {
                return TypeValue.Decimal;
            }

            if (type.IsEnum) {
                return TypeValue.Enum;
            }

            if (type == typeof(string)) {
                return TypeValue.String;
            }

            if (type.IsGenericType) {
                Type generic = type.GetGenericTypeDefinition();
                if (generic == typeof(List<>)) {
                    return TypeValue.List;
                }
                if (generic == typeof(Dictionary<,>)) {
                    return TypeValue.Dictionary;
                }
                if (generic == typeof(Stack<>)) {
                    return TypeValue.Stack;
                }
                if (generic == typeof(Queue<>)) {
                    return TypeValue.Queue;
                }
                if (generic == typeof(HashSet<>)) { }
                if (generic == typeof(LinkedList<>)) { }
            }

            if (type.IsArray) {
                return type.GetArrayRank() != 1 ? TypeValue.Unknown : TypeValue.Array;
            }

            if (type.IsValueType) {
                return TypeValue.Struct;
            }

            if (type.IsClass) {
                // check for list, dictionary, other common ones
                return type.IsSubclassOf(typeof(UnityEngine.Object)) ? TypeValue.UnityObject : TypeValue.Class;
            }

            return TypeValue.Unknown;
        }

        public static string GetTypeName(TypeValue typeValue, string typeName) {
            switch (typeValue) {
                case TypeValue.Type:
                    return "type";

                case TypeValue.String:
                    return "string";

                case TypeValue.Boolean:
                    return "bool";

                case TypeValue.Float:
                    return "float";

                case TypeValue.Double:
                    return "double";

                case TypeValue.Decimal:
                    return "decimal";

                case TypeValue.Integer:
                    return "int";

                case TypeValue.UnsignedInteger:
                    return "uint";

                case TypeValue.SignedByte:
                    return "sbyte";

                case TypeValue.Byte:
                    return "byte";

                case TypeValue.Short:
                    return "short";

                case TypeValue.UnsignedShort:
                    return "ushort";

                case TypeValue.Long:
                    return "long";

                case TypeValue.UnsignedLong:
                    return "ulong";

                case TypeValue.Char:
                    return "char";

                case TypeValue.Enum:
                    return "enum";

//                case TypeValue.Array:
//                    return "Array";

                case TypeValue.UnityObject:
                    return "UnityObject";

                case TypeValue.Vector2:
                    return "Vector2";

                case TypeValue.Vector3:
                    return "Vector3";

                case TypeValue.Vector4:
                    return "Vector4";

                case TypeValue.Quaternion:
                    return "Quaternion";

                case TypeValue.Color:
                    return "Color";

//                case TypeValue.List:
//                    return "List";

//                case TypeValue.Dictionary:
//                    return "Dictionary";

                default:
                    return typeName;
            }
        }

    }

}