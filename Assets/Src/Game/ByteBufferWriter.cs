using System;
using System.Collections.Generic;
using System.Text;
using SpaceGame.Persistence;
using UnityEngine;

namespace SpaceGame {

    public class ByteBufferWriter {
        
        public const int TypeId_Int = 1;
        public const int TypeId_Float = 2;
        public const int TypeId_Bool = 3;
        public const int TypeId_String = 4;
        public const int TypeId_IntArray = 5;
        public const int TypeId_FloatArray = 6;
        public const int TypeId_StringArray = 7;
        public const int TypeId_BoolArray = 8;
        public const int TypeId_Vector2 = 9;
        public const int TypeId_Vector3 = 10;
        public const int TypeId_Vector4 = 11;
        public const int TypeId_Quaternion = 12;
        public const int TypeId_Color = 13;
        public const int TypeId_Vector2Array = 14;
        public const int TypeId_Vector3Array = 15;
        public const int TypeId_Vector4Array = 16;
        public const int TypeId_QuaternionArray = 17;
        public const int TypeId_ColorArray = 18;

        public const int TypeId_Struct = 19;
        public const int TypeId_StructArray = 20;
        public const int TypeId_AssetPointer = 21;
        public const int TypeId_AssetArray = 21;

        public const int TypeId_C2 = 100;
        
        public class C2 { }

        public struct SerializedField {

            public string name;
            public int typeId;
            public byte[] bytes;

            public SerializedField(string name, int typeId, byte[] bytes) {
                this.name = name;
                this.typeId = typeId;
                this.bytes = bytes;
            }

        }

        // class Consideration<DecisionContext>
        // field type field Name field value
        private List<SerializedField> fields;

        public ByteBufferWriter() {
            string s = nameof(List<SerializedField>);
            // TagName: RefArray: SomeType[count] {
            // [0] NameSpaced.Name.Without.Assembly.ExactTypeName
            //  - fieldName ExactTypeName
            //    - fieldValue nested data
            //    - 
            //  - fieldName fieldValue
            // StructArray: 
            // { MakeDeserializer<List<SerializedField>>(() => new Type())
        this.fields = new List<SerializedField>(64);
        }

        public void WriteIntField(string name, int value) {
            fields.Add(new SerializedField(name, TypeId_Int, BitConverter.GetBytes(value)));
        }

        public void WriteFloatField(string name, float value) {
            fields.Add(new SerializedField(name, TypeId_Float, BitConverter.GetBytes(value)));
        }

        public void WriteBooleanField(string name, bool value) {
            fields.Add(new SerializedField(name, TypeId_Bool, BitConverter.GetBytes(value)));
        }

        public void WriteStringField(string name, string value) {
            fields.Add(new SerializedField(name, TypeId_String, Encoding.UTF8.GetBytes(value)));
        }

        public void WriteIntArrayField(string name, int[] value) {
            byte[] result = new byte[value.Length * sizeof(int)];
            Buffer.BlockCopy(value, 0, result, 0, result.Length);
            fields.Add(new SerializedField(name, TypeId_Bool, result));
        }
        
        public void WriteBoolArrayField(string name, bool[] value) { }
        public void WriteFloatArrayField(string name, float[] value) { }
        public void WriteStringArrayField(string name, string[] value) { }

        public void WriteColorField(string name, Color value) { }
        public void WriteVector2Field(string name, Vector2 value) { }
        public void WriteVector3Field(string name, Vector3 value) { }
        public void WriteVector4Field(string name, Vector4 value) { }
        public void WriteQuaternionFieldField(string name, Quaternion value) { }

        public void WriteColorArrayField(string name, Color[] value) { }
        public void WriteVector2ArrayField(string name, Vector2[] value) { }
        public void WriteVector3ArrayField(string name, Vector3[] value) { }
        public void WriteVector4ArrayField(string name, Vector4[] value) { }
        public void WriteQuaternionArrayField(string name, Quaternion[] value) { }

    }

}