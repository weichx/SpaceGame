using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Weichx.Persistence {

    public partial class Snapshot<T> {

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
            private List<Action> listCleaners;
            private Dictionary<object, TargetInfo> listTargetInfo;

            private int pointer;
            private string[] lines;

            public StringDeserializer() {
                typeMap = new List<Type>(32);
                typeDefinitionMap = new List<TypeDefinition>(32);
                assemblyMap = new List<string>(4);
                referenceMap = new List<object>(16);
                targetStack = new Stack<TargetInfo>(4);
                listCleaners = new List<Action>(4);
                listTargetInfo = new Dictionary<object, TargetInfo>();
            }

            private class TargetInfo {

                public object target;
                public readonly TypeDefinition typeDefinition;
                public int assignedFields;
                public IList targetAsList;
                public List<int> arrayFieldsToClean;

                public TargetInfo(object target, TypeDefinition typeDefinition) {
                    this.target = target;
                    this.targetAsList = target as IList;
                    this.typeDefinition = typeDefinition;
                    this.arrayFieldsToClean = new List<int>();
                    this.assignedFields = 0;
                }

                //when values are null because their type are missing, we can get in trouble
                //with having unexpected nulls in arrays. This resizes lists and arrays so
                //that that doesn't happen.
                public void CleanMissingTypeListValues() {
                    
                    if (arrayFieldsToClean.Count == 0) return;
                    
                    int notToCleanCount = targetAsList.Count - arrayFieldsToClean.Count;
                    IList newTarget;
                    Type targetType = typeDefinition.type;
                    if (target.GetType().IsArray) {
                        newTarget = Array.CreateInstance(targetType.GetElementType(), notToCleanCount);
                    }
                    else {
                        newTarget = MakeInstance<IList>(targetType);
                        object tempVal = GetDefaultForType(typeDefinition.GetArrayLikeElementType().type);
                        for (int i = 0; i < notToCleanCount; i++) {
                            newTarget.Add(tempVal);
                        }
                    }
                    int currentIndex = 0;
                    for (int i = 0; i < targetAsList.Count; i++) {
                        if (!arrayFieldsToClean.Contains(i)) {
                            newTarget[currentIndex] = targetAsList[i];
                            currentIndex++;
                        }
                    }
                    arrayFieldsToClean.Clear();
                    target = newTarget;
                }

            }

            public T ObjectFromString(string serializedSnapshot) {
                lines = serializedSnapshot.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

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

                for (int i = 0; i < listCleaners.Count; i++) {
                    listCleaners[i]();
                }

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
                    // note: type might be null here! that's ok, handled in assignment code
                    Type type = Type.GetType($"{typeName}, {assemblyMap[assemblyId]}");
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
                    if (type == null) {
                        referenceMap.Add(null);
                    }
                    else {
                        if (type.IsArray) {
                            referenceMap.Add(Array.CreateInstance(type.GetElementType(), reader.ReadTaggedInt('C')));
                        }
                        else if (typeof(IList).IsAssignableFrom(type)) {
                            IList list = MakeInstance<IList>(type);
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
                            referenceMap.Add(MakeInstance(typeMap[typeId]));
                        }
                    }
                    pointer++;
                }
            }

            private object GetCleanedList(object listToClean) {
                TargetInfo ti = listTargetInfo[listToClean];
                if (ti == null) return listToClean;
                ti.CleanMissingTypeListValues();
                return ti.target;
            }

            private void AssignValue(string name, object value, bool nullDueToMissingType) {
                TargetInfo ti = targetStack.Pop();
                // target will be null if type cannot resolved
                if (ti.target != null) {

                    // if the value is a list we need to possibly clean it (remove nulls) from it which 
                    // in the array case means re-assigning.

                    if (ti.typeDefinition.IsArrayLike) {
                        ti.targetAsList[ti.assignedFields] = value;
                        if (value == null && nullDueToMissingType) {
                            ti.arrayFieldsToClean.Add(ti.assignedFields);
                        }
                        if (value is IList) {
                            //need to close over this index value
                            int index = ti.assignedFields;
                            // todo -- im a little concerned about this breaking if the index changes due to cleaning
                            listCleaners.Add(() => ti.targetAsList[index] = GetCleanedList(ti.targetAsList[index]));
                        }
                    }
                    else {
                        FieldInfo fi = FindFieldInfo(ti.typeDefinition.fields, value, name);
                        if (fi != null) {
                            fi.SetValue(ti.target, value);
                            if (value is IList) {
                                listCleaners.Add(() => fi.SetValue(ti.target, GetCleanedList(fi.GetValue(ti.target))));
                            }
                        }
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
                    if (reference is IList) {
                        listTargetInfo.Add(reference, targetInfo);
                    }
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
                        object structValue = MakeInstance(type);
                        TargetInfo targetInfo = new TargetInfo(structValue, typeDefinitionMap[typeId]);
                        targetStack.Push(targetInfo);
                        ReadFieldLines(fieldCount);
                        targetStack.Pop();
                        AssignValue(name, structValue, typeDefinitionMap[typeId] == null);
                    }
                    else if (lineChar == ReferenceChar) {
                        //line = "% @RrefId name"
                        int fieldRefId = reader.ReadTaggedInt('R');
                        string name = reader.ReadString();
                        object refVal = fieldRefId == -1 ? null : referenceMap[fieldRefId];
                        bool nullDueToTypeMissing = fieldRefId != -1 && refVal == null;
                        AssignValue(name, refVal, nullDueToTypeMissing);
                    }
                    else if (lineChar == PrimitiveChar) {
                        // line = "- @TtypeId name value"
                        int typeId = reader.ReadTaggedInt('T');
                        string name = reader.ReadString();
                        string value = reader.ReadLine();
                        TypeDefinition td = typeDefinitionMap[typeId];
                        AssignValue(name, DeserializePrimitive(td, value), false);
                    }
                    else if (lineChar == TypeChar) {
                        int typeId = reader.ReadTaggedInt('T');
                        string name = reader.ReadString();
                        object typeVal = typeId == -1 ? null : typeMap[typeId];
                        bool nullDueToTypeMissing = typeId != -1 && typeVal == null;
                        AssignValue(name, typeVal, nullDueToTypeMissing);
                    }
                    else if (lineChar == UnityChar) {
                        //todo -- not handling unity right now
                        int typeId = reader.ReadTaggedInt('T');
                        string name = reader.ReadString();
                        AssignValue(name, null, true);
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
                return type.IsValueType ? MakeInstance(type) : null;
            }

        }

    }

}