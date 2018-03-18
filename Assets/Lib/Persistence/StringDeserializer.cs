using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Weichx.Persistence {

    public class StringDeserializer {

        private const char StructChar = '&';
        private const char ReferenceChar = '%';
        private const char PrimitiveChar = '-';
        private const char TypeChar = '*';
        private const char UnityChar = '!';
        private const char UnknownChar = '?';

        private List<Type> typeMap;
        private List<TypeDefinition> typeDefinitionMap;
        private List<string> assemblyMap;
        private List<object> referenceMap;
        private Stack<TargetInfo> targetStack;

        private int pointer;
        private string[] lines;

        public StringDeserializer() {
            typeMap = new List<Type>(32);
            typeDefinitionMap = new List<TypeDefinition>(32);
            assemblyMap = new List<string>(4);
            referenceMap = new List<object>(16);
            targetStack = new Stack<TargetInfo>(4);
        }

        private struct TargetInfo {

            public readonly object target;
            public readonly TypeDefinition typeDefinition;
            public readonly Array targetAsArray;
            public int assignedFields;
            public IList targetAsList;

            public TargetInfo(object target, TypeDefinition typeDefinition) {
                this.target = target;
                this.targetAsArray = target as Array;
                this.targetAsList = target as IList;
                this.typeDefinition = typeDefinition;
                this.assignedFields = 0;
            }

        }

        public T ObjectFromString<T>(string serializedSnapshot) {
            lines = serializedSnapshot.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

            //File Format:
            //Assemblies: @assemblyId assemblyName
            //Types: @TtypeId typeName
            //References: @RrefId @TtypeId @CfieldCountIfArray
            //Fields starting with ^ are reference definitions. @RrefId followed by n fields or array members
            //Fields starting with - are primitives or strings
            //Fields starting with & are structs
            //Fields starting with % are references

            ReadAssemblies();
            pointer++;
            ReadTypes();
            pointer++;
            ReadReferences();
            pointer++;
            ReadFields();

            return referenceMap[0] is T ? (T) referenceMap[0] : default(T);
        }

        private void ReadAssemblies() {
            while (pointer < lines.Length && lines[pointer] != string.Empty) {
                StringReader reader = new StringReader(lines[pointer]);
                reader.ReadTaggedInt('A');
                string assemblyName = reader.ReadLine();
                assemblyMap.Add(assemblyName);
                pointer++;
            }
        }

        private void ReadTypes() {
            while (pointer < lines.Length && lines[pointer] != string.Empty) {
                StringReader reader = new StringReader(lines[pointer]);
                reader.ReadTaggedInt('T');
                int assemblyId = reader.ReadTaggedInt('A');
                string typeName = reader.ReadLine();
                Type type = Type.GetType(typeName + ", " + assemblyMap[assemblyId]);
                typeMap.Add(type);
                typeDefinitionMap.Add(TypeDefinition.Get(type));
                pointer++;
            }
        }

        private void ReadReferences() {
            while (pointer < lines.Length && lines[pointer] != string.Empty) {
                StringReader reader = new StringReader(lines[pointer]);
                reader.ReadTaggedInt('R');
                int typeId = reader.ReadTaggedInt('T');
                Type type = typeMap[typeId];
                if (type.IsArray) {
                    referenceMap.Add(Array.CreateInstance(type.GetElementType(), reader.ReadTaggedInt('C')));
                }
                else if (typeof(IList).IsAssignableFrom(type)) {
                    IList list = Activator.CreateInstance(type) as IList;
                    Debug.Assert(list != null, nameof(list) + " != null");
                    int fieldCount = reader.ReadTaggedInt('C');
                    Type elementType = type.GetGenericArguments()[0];
                    object tempVal = GetDefaultForType(elementType);
                    for (int i = 0; i < fieldCount; i++) {
                        list.Add(tempVal);
                    }
                    referenceMap.Add(list);
                }
                else {
                    referenceMap.Add(Activator.CreateInstance(typeMap[typeId]));
                }
                pointer++;
            }
        }

        private void AssignValue(string name, object value) {
            TargetInfo ti = targetStack.Pop();
            if (ti.typeDefinition.IsArray) {
                ti.targetAsArray.SetValue(value, ti.assignedFields);
            }
            else if (ti.typeDefinition.IsArrayLike) {
                ti.targetAsList[ti.assignedFields] = value;
            }
            else {
                FieldInfo fi = FindFieldInfo(ti.typeDefinition.fields, value, name);
                if (fi != null) {
                    fi.SetValue(ti.target, value);
                }
            }
            ti.assignedFields++;
            targetStack.Push(ti);
        }

        private void ReadFields() {
            for (int i = 0; i < referenceMap.Count; i++) {
                string line = lines[pointer];
                StringReader reader = new StringReader(line);
                int refId = reader.ReadTaggedInt('R');
                int typeId = reader.ReadTaggedInt('T');
                int fieldCount = reader.ReadTaggedInt('C');
                object reference = referenceMap[refId];
                TargetInfo targetInfo = new TargetInfo(reference, typeDefinitionMap[typeId]);
                Debug.Assert(targetStack.Count == 0);
                targetStack.Push(targetInfo);
                ReadFieldLines(fieldCount);
                targetStack.Pop();
                Debug.Assert(targetStack.Count == 0);
                pointer++;
            }

        }

        private void ReadFieldLines(int count) {
            int read = 0;

            while (read != count) {
                pointer++;
                string line = lines[pointer];
                StringReader reader = new StringReader(line);
                char lineChar = line[0];
                if (lineChar == StructChar) {
                    //line = "& @TtypeId name @CfieldCount"
                    Debug.Assert(targetStack.Count > 0);
                    int typeId = reader.ReadTaggedInt('T');
                    string name = reader.ReadString();
                    int fieldCount = reader.ReadTaggedInt('C');
                    Type type = typeMap[typeId];
                    object structValue = Activator.CreateInstance(type);
                    TargetInfo targetInfo = new TargetInfo(structValue, typeDefinitionMap[typeId]);
                    targetStack.Push(targetInfo);
                    ReadFieldLines(fieldCount);
                    targetStack.Pop();
                    AssignValue(name, structValue);
                }
                else if (lineChar == ReferenceChar) {
                    //line = "% @RrefId name"
                    int fieldRefId = reader.ReadTaggedInt('R');
                    string name = reader.ReadString();
                    AssignValue(name, fieldRefId == -1 ? null : referenceMap[fieldRefId]);
                }
                else if (lineChar == PrimitiveChar) {
                    // line = "- @TtypeId name value"
                    int typeId = reader.ReadTaggedInt('T');
                    string name = reader.ReadString();
                    string value = reader.ReadLine();
                    TypeDefinition td = typeDefinitionMap[typeId];
                    AssignValue(name, DeserializePrimitive(td, value));
                }
                else if (lineChar == TypeChar) {
                    int typeId = reader.ReadTaggedInt('T');
                    string name = reader.ReadString();
                    AssignValue(name, typeId != -1 ? typeMap[typeId] : null);
                }
                else if (lineChar == UnityChar) {
                    //todo -- not handling unity right now
                    int typeId = reader.ReadTaggedInt('T');
                    string name = reader.ReadString();
                    AssignValue(name, null);
                }
                else if (lineChar == UnknownChar) { }
                read++;
            }

        }

        private static FieldInfo FindFieldInfo(FieldInfo[] fieldInfos, object value, string name) {
            if (value != null) {
                Type type = value.GetType();
                for (int j = 0; j < fieldInfos.Length; j++) {
                    FieldInfo fi = fieldInfos[j];
                    if (fi.Name == name && fi.FieldType.IsAssignableFrom(type)) {
                        return fi;
                    }
                }
            }
            else {
                for (int j = 0; j < fieldInfos.Length; j++) {
                    FieldInfo fi = fieldInfos[j];
                    if (fi.Name == name) {
                        return fi;
                    }
                }
            }

            return null;
        }

        private static object DeserializePrimitive(TypeDefinition typeDefinition, string value) {
            switch (typeDefinition.typeValue) {
                case TypeValue.Double:
                    return double.Parse(value);
                case TypeValue.Decimal:
                    return decimal.Parse(value);
                case TypeValue.Float:
                    return float.Parse(value);
                case TypeValue.Enum:
                    return Enum.Parse(typeDefinition.type, value);
                case TypeValue.String:
                    return Regex.Unescape(value);
                case TypeValue.Boolean:
                    return bool.Parse(value);
                case TypeValue.Integer:
                    return int.Parse(value);
                case TypeValue.UnsignedInteger:
                    return uint.Parse(value);
                case TypeValue.SignedByte:
                    return sbyte.Parse(value);
                case TypeValue.Byte:
                    return byte.Parse(value);
                case TypeValue.Short:
                    return short.Parse(value);
                case TypeValue.UnsignedShort:
                    return ushort.Parse(value);
                case TypeValue.Long:
                    return long.Parse(value);
                case TypeValue.UnsignedLong:
                    return ulong.Parse(value);
                case TypeValue.Char:
                    return char.Parse(value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static object GetDefaultForType(Type type) {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

    }

}