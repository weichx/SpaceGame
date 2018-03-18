using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Weichx.Persistence {

    public partial class Snapshot<T> {

        public class Deserializer {

            private object[] referenceMap;
            private ReferenceDefinition[] referenceDefinitions;

            internal Deserializer(Dictionary<object, ReferenceDefinition> inputRefMap) {
                int i = 0;
                Dictionary<object, ReferenceDefinition>.ValueCollection refs = inputRefMap.Values;
                referenceDefinitions = new ReferenceDefinition[refs.Count];
                referenceMap = new object[refs.Count];
                foreach (ReferenceDefinition referenceDefinition in refs) {
                    referenceDefinitions[i++] = referenceDefinition;
                }
            }

            public T Deserialize() {
                Array.Clear(referenceMap, 0, referenceMap.Length);
                for (int i = 0; i < referenceDefinitions.Length; i++) {
                    referenceMap[i] = CreateReference(referenceDefinitions[i]);
                }
                for (int i = 0; i < referenceDefinitions.Length; i++) {
                    if (referenceDefinitions[i].typeDefinition.IsArray) {
                        CreateArrayMembers(referenceMap[i] as Array, referenceDefinitions[i]);
                    }
                    else if (referenceDefinitions[i].typeDefinition.IsList) {
                        CreateListMembers(referenceMap[i] as IList, referenceDefinitions[i]);
                    }
                    else {
                        CreateFields(referenceMap[i], referenceDefinitions[i].fields);
                    }
                }
                return (T) referenceMap[0];
            }

            private object CreateReference(ReferenceDefinition referenceDefinition) {
                TypeDefinition typeDefinition = referenceDefinition.typeDefinition;
                if (typeDefinition.IsArray) {
                    Type typeElement = typeDefinition.type.GetElementType();
                    return Array.CreateInstance(typeElement, referenceDefinition.fields.Length);
                }
                else if (typeDefinition.IsList) {
                    Type elementType = typeDefinition.GetArrayLikeElementType().type;
                    IList list = Activator.CreateInstance(typeDefinition.type) as IList;
                    Debug.Assert(list != null, nameof(list) + " != null");
                    object defaultValue = GetDefaultForType(elementType);
                    for (int i = 0; i < referenceDefinition.fields.Length; i++) {
                        list.Add(defaultValue);
                    }
                    return list;
                }
                else {
                    return Activator.CreateInstance(typeDefinition.type);
                }
            }

            private static object GetDefaultForType(Type type) {
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }

            private void CreateFields(object target, FieldDefinition[] fields) {
                for (int i = 0; i < fields.Length; i++) {
                    FieldDefinition field = fields[i];
                    switch (field.fieldType) {
                        case FieldType.Array:
                        case FieldType.Reference:
                            field.fieldInfo.SetValue(target, field.refId == -1 ? null : referenceMap[field.refId]);
                            break;
                        case FieldType.Struct:
                            field.fieldInfo.SetValue(target, CreateStruct(field));
                            break;
                        case FieldType.Primitive:
                            field.fieldInfo.SetValue(target, field.value);
                            break;
                        case FieldType.Type:
                            field.fieldInfo.SetValue(target, field.value);
                            break;
                        case FieldType.Unity:
                            field.fieldInfo.SetValue(target, null);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            private void CreateListMembers(IList target, ReferenceDefinition def) {
                FieldDefinition[] fields = def.fields;
                for (int i = 0; i < fields.Length; i++) {
                    FieldDefinition field = fields[i];
                    switch (field.fieldType) {
                        case FieldType.Array:
                        case FieldType.Reference:
                            target[i] = field.refId == -1 ? null : referenceMap[field.refId];
                            break;
                        case FieldType.Struct:
                            target[i] = CreateStruct(field);
                            break;
                        case FieldType.Primitive:
                            target[i] = field.value;
                            break;
                        case FieldType.Type:
                            target[i] = field.value;
                            break;
                        case FieldType.Unity:
                            target[i] = null;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            private void CreateArrayMembers(Array target, ReferenceDefinition def) {
                FieldDefinition[] fields = def.fields;
                for (int i = 0; i < fields.Length; i++) {
                    FieldDefinition field = fields[i];
                    switch (field.fieldType) {
                        case FieldType.Array:
                        case FieldType.Reference:
                            target.SetValue(field.refId == -1 ? null : referenceMap[field.refId], i);
                            break;
                        case FieldType.Struct:
                            target.SetValue(CreateStruct(field), i);
                            break;
                        case FieldType.Primitive:
                            target.SetValue(field.value, i);
                            break;
                        case FieldType.Type:
                            target.SetValue(field.value, i);
                            break;
                        case FieldType.Unity:
                            target.SetValue(null, i);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            private object CreateStruct(FieldDefinition structField) {
                object structVal = Activator.CreateInstance(structField.typeDefinition.type);
                CreateFields(structVal, structField.members);
                return structVal;
            }

        }

    }

}