using System;
using System.Collections.Generic;
using SpaceGame.Persistence;
using UnityEditor;
using UnityEngine;

namespace SpaceGame {

    public struct SerializedEntity {

        public int id;

    }

    public struct SerializedClass {

        public string typeName;

    }

    class SomeAttr : Attribute {

        public SomeAttr(string value) { }

    }

    public struct SerializedField {

        public readonly byte[] data;

    }

    public enum SerializedFieldType {

        Float,
        Int,
        Bool,
        Char,
        String,
        Object,
        GenericObject,
        Array,
        AssetPointer

    }

    public class Inspector {

        public bool Render(Rect rect, SerializedField field) {
            ByteBufferReader reader = new ByteBufferReader(field.data);
            SerializedFieldType fieldType = (SerializedFieldType) reader.ReadInt();
            string fieldName = reader.ReadString();

            switch (fieldType) {
                case SerializedFieldType.Float:
                    return RenderFloatInspector(rect, fieldName, reader);
                case SerializedFieldType.Int:
                    return RenderIntInspector(rect, fieldName, reader);
            }
            return false;
        }

        private bool RenderFloatInspector(Rect rect, string fieldName, ByteBufferReader reader) {
            float value = reader.ReadFloat();
            float retn = EditorGUI.FloatField(rect, new GUIContent(fieldName), value);
            return (retn != value);
        }

        private bool RenderIntInspector(Rect rect, string fieldName, ByteBufferReader reader) {
            int value = reader.ReadInt();
            float retn = EditorGUI.IntField(rect, new GUIContent(fieldName), value);
            return (retn != value);
        }

    }

    [SomeAttr(("str"))]
    public class DataFile : ScriptableObject {

        public List<AudioClip> sounds;
        public List<GameObject> gameObjects;

        public List<int> entityPointers;
        public List<int> shipPointers;

        /*
         * General
         *     Ship Definitions
         *     AI Behaviors
         *     Weapon Definitions
         * Mission
         *     Entitites
         *         Id
         *         Name
         *         Ship Pointer
         *         Stat Pointer
         *         Behavior Pointer
         *     Waypoints
         *     Areas
         *     Other
         * Per Mission
         *     Event List
         *     Data List
         *     Entity Positions
         *
         *
         * void Deserialize(Reader reader) {
         *    float x = Reader.TryRead<float>(nameof(x));
         *      GameObject prefab = Reader.TryReadPointer();
         * }
         *
         * void Serialize(Writer writer) {
         *    writer.WriteFloat(nameof(x), x);
         *    writer.WritePointer(nameof(y), gameObject);
         * }
         *
         * Section {
         *     sectionId
         *     sectionSize
         *     data Pairs[] {
         *        definedTypeName/Id => bytes
         *     }
         *     
         * }
         *
         *
         * InspectorX (type)
         *     Data Pairs = Serialize(thing);
         *     DataTable
         *
         *     Section {
         *         TypeId
         *         InstanceId
         *         Data...
         *     }
         *
         *
         *    ArrayMustBeOfPureType
         *     
         *
         *     TypeConstructor {
         *         
         *        Dictionary<int, Func<T>> { typeId => StructDeserializer }
         * 
         *     }
         *
         * Mission.Deserialize()
         * Template.Deserialize()
         *
         * inspector = GetGameType(typeof(Thing)).GetInspector();
         * inspector.Render();
         * 
         * if(inspector.changed) {
         *    Data.Update();
         * }
         *
         * Inspectors.Add(typeof(type), () => {
         *
         *   float newValue = Editor.FloatField(value, label);
         *   if(newValue != value) {
         *        changed = true;
         * }
         * 
         * }
         *
         * 
         */

        interface IGameSerializable { }

        interface IGameDeserializer {

            float ReadFloat(string name);
            float ReadInt(string name);
            GameObject ReadGameObject(string fieldName);
            T[] ReadArray<T>(string name) where T : struct;

        }

        class Thing<T> : IGameSerializable {

            public float x;
            public float[] list;

            public void Deserialize(IGameDeserializer reader) {
//                x = reader.ReadFloat(nameof(x));
//                list = reader.ReadArray<>(nameof(list));
//                string str = reader.ReadString(nameof(list));
//                writer.WriteString(nameof(list), "");
            }

            public static void DeserializeInstance() { }

        }

    }

}