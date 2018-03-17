using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Weichx.Persistence {

    public partial class Snapshot<T> {

        internal class StringSerializer {

            private const char StructChar = '&';
            private const char ReferenceChar = '%';
            private const char PrimitiveChar = '-';
            private const char TypeChar = '*';
            private const char UnityChar = '!';
            private const char UnknownChar = '?';

            private Dictionary<Type, int> typeMap;
            private Dictionary<Assembly, int> assemblyMap;

            private int typeIdGenerator;
            private int assemblyIdGenerator;
            private int referenceIdGenerator;

            private StringBuilder typeSection;
            private StringBuilder fieldSection;
            private StringBuilder assemblySection;
            private StringBuilder referenceSection;

            public StringSerializer(Dictionary<object, ReferenceDefinition> inputRefMap) {
                this.typeMap = new Dictionary<Type, int>();
                this.assemblyMap = new Dictionary<Assembly, int>();
                this.typeSection = new StringBuilder(500);
                this.fieldSection = new StringBuilder(500);
                this.assemblySection = new StringBuilder(500);
                this.referenceSection = new StringBuilder(500);
                foreach (ReferenceDefinition item in inputRefMap.Values) {
                    int refId = item.refId;
                    int typeId = GetTypeId(item.typeDefinition);
                    int fieldCount = item.fields.Length;
                    referenceSection.AppendLine(WriteReferenceSectionLine(refId, typeId, fieldCount));
                    fieldSection.AppendLine(WriteReferenceFieldDefinitionLine(refId, typeId, fieldCount));
                    WriteFields(item.fields);
                }
            }

            private string WriteAssemblyLine(int assemblyId, string assemblyName) {
                return $"@A{assemblyId} {assemblyName}";
            }

            private string WriteTypeLine(int typeId, int assemblyId, string typeName) {
                return $"@T{typeId} @A{assemblyId} {typeName}";
            }

            private string WriteReferenceSectionLine(int refId, int typeId, int fieldCount) {
                return $"@R{refId} @T{typeId} @C{fieldCount}";
            }

            private string WriteReferenceFieldDefinitionLine(int refId, int typeId, int fieldCount) {
                return $"^ @R{refId} @T{typeId} @C{fieldCount}";
            }

            private string WriteReferenceFieldLine(int refId, string name) {
                return $"{ReferenceChar} @R{refId} {name}";
            }

            private string WriteUnityFieldLine(int typeId, string name) {
                return $"{UnityChar} @T{typeId} {name}"; // todo -- handle this case
            }

            private string WritePrimitiveFieldLine(FieldDefinition field) {
                return StringifyPrimitive(field);
            }

            private string WriteStructLine(int typeId, string name, int fieldCount) {
                return $"{StructChar} @T{typeId} {name} @C{fieldCount}";
            }

            private string WriteTypeFieldLine(int typeId, string name) {
                return $"{TypeChar} @T{typeId} {name}";
            }

            private string WriteUnknownFieldLine(int typeId, string name) {
                return $"{UnknownChar} @T{typeId} {name}";
            }

            public override string ToString() {
                assemblySection.AppendLine("");
                assemblySection.AppendLine(typeSection.ToString());
                assemblySection.AppendLine(referenceSection.ToString());
                assemblySection.Append(fieldSection);
                return assemblySection.ToString();
            }

            private void WriteFields(FieldDefinition[] fields) {
                for (int i = 0; i < fields.Length; i++) {
                    FieldDefinition field = fields[i];
                    TypeDefinition childTypeDefinition = fields[i].typeDefinition;
                    int typeId = GetTypeId(childTypeDefinition);
                    int fieldCount = field.members?.Length ?? 0;
                    string fieldName = field.name;

                    if (childTypeDefinition.IsPrimitiveLike) {
                        fieldSection.AppendLine(WritePrimitiveFieldLine(field));
                    }
                    else if (childTypeDefinition.IsStructType) {
                        fieldSection.AppendLine(WriteStructLine(typeId, fieldName, fieldCount));
                        WriteFields(field.members);
                    }
                    else if (childTypeDefinition.IsArray) {
                        fieldSection.AppendLine(WriteReferenceFieldLine(field.refId, fieldName));
                    }
                    else if (childTypeDefinition.IsType) {
                        Type typeValue = field.value as Type;
                        if (typeValue != null) {
                            int typeValueId = GetTypeId(TypeDefinition.Get(typeValue));
                            fieldSection.AppendLine(WriteTypeFieldLine(typeValueId, fieldName));
                        }
                        else {
                            fieldSection.AppendLine(WriteTypeFieldLine(-1, fieldName));
                        }
                    }
                    else if (childTypeDefinition.IsReferenceType) {
                        fieldSection.AppendLine(WriteReferenceFieldLine(field.refId, fieldName));
                    }
                    else if (childTypeDefinition.IsUnityType) {
                        fieldSection.AppendLine(WriteUnityFieldLine(typeId, fieldName));
                    }
                    else {
                        fieldSection.AppendLine(WriteUnknownFieldLine(typeId, fieldName));
                    }
                }
            }

            protected int GetTypeId(TypeDefinition typeDefinition) {
                int typeId;
                if (!typeMap.TryGetValue(typeDefinition.type, out typeId)) {

                    if (!assemblyMap.ContainsKey(typeDefinition.type.Assembly)) {
                        int assemblyId = assemblyIdGenerator++;
                        assemblyMap.Add(typeDefinition.type.Assembly, assemblyId);
                        assemblySection.AppendLine(WriteAssemblyLine(assemblyId, typeDefinition.type.Assembly.FullName));
                    }

                    typeId = typeIdGenerator++;
                    typeMap.Add(typeDefinition.type, typeId);
                    typeSection.AppendLine(WriteTypeLine(typeId, assemblyMap[typeDefinition.type.Assembly], typeDefinition.type.FullName));
                }
                return typeId;

            }

            public string StringifyPrimitive(FieldDefinition field) {
                object value = field.value;
                string fieldName = field.name;
                TypeDefinition td = field.typeDefinition;
                int typeId = GetTypeId(td);
                switch (td.typeValue) {
                    case TypeValue.Double:
                        return $"{PrimitiveChar} @T{typeId} {fieldName} {(double) value:R}";
                    case TypeValue.Decimal:
                        return $"{PrimitiveChar} @T{typeId} {fieldName} {Convert.ToDouble(value):R}";
                    case TypeValue.Float:
                        return $"{PrimitiveChar} @T{typeId} {fieldName} {(float) value:R}";
                    case TypeValue.Enum:
                        return $"{PrimitiveChar} @T{typeId} {fieldName} {(int) value}";
                    case TypeValue.String:
                        string strVal = value == null ? null : Regex.Escape(value.ToString());
                        return $"{PrimitiveChar} @T{typeId} {fieldName} {strVal}";
                    case TypeValue.Boolean:
                        return $"{PrimitiveChar} @T{typeId} {fieldName} {value.ToString().ToLower()}";
                    case TypeValue.Integer:
                        return $"{PrimitiveChar} @T{typeId} {fieldName} {value}";
                    case TypeValue.UnsignedInteger:
                        return $"{PrimitiveChar} @T{typeId} {fieldName} {value}";
                    case TypeValue.SignedByte:
                        return $"{PrimitiveChar} @T{typeId} {fieldName} {value}";
                    case TypeValue.Byte:
                        return $"{PrimitiveChar} @T{typeId} {fieldName} {value}";
                    case TypeValue.Short:
                        return $"{PrimitiveChar} @T{typeId} {fieldName} {value}";
                    case TypeValue.UnsignedShort:
                        return $"{PrimitiveChar} @T{typeId} {fieldName} {value}";
                    case TypeValue.Long:
                        return $"{PrimitiveChar} @T{typeId} {fieldName} {value}";
                    case TypeValue.UnsignedLong:
                        return $"{PrimitiveChar} @T{typeId} {fieldName} {value}";
                    case TypeValue.Char:
                        return $"{PrimitiveChar} @T{typeId} {fieldName} {value}";
//                case TypeValue.Vector2:
//                    Vector2 v2 = (Vector2) value;
//                    return $"{StructChar} @T{typeId} {fieldName} {v2.x:R} {v2.y:R}";
//                case TypeValue.Vector3:
//                    Vector3 v3 = (Vector3) value;
//                    return $"{StructChar} @T{typeId} {fieldName} {v3.x:R} {v3.y:R} {v3.z:R}";
//                case TypeValue.Vector4:
//                    Vector4 v4 = (Vector4) value;
//                    return $"{StructChar} @T{typeId} {fieldName} {v4.x:R} {v4.y:R} {v4.z:R} {v4.w:R}";
//                case TypeValue.Quaternion:
//                    Quaternion q = (Quaternion) value;
//                    return $"{StructChar} @T{typeId} {fieldName} {q.x:R} {q.y:R} {q.z:R} {q.w:R}";
//                case TypeValue.Color:
//                    Color c = (Color) value;
//                    return $"{StructChar} @T{typeId} {fieldName} {c.r:R} {c.g:R} {c.b:R} {c.a:R}";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }

    }

}