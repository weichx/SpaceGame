using System;
using System.Collections.Generic;
using SpaceGame.AI;
using UnityEngine;
using Freespace;

namespace Src.Editor {

    public struct Section { }

    public class Reader {

        public byte[] data;

        public SectionHeader ReadSectionHeader() {
            return new SectionHeader();
        }

        public struct SectionHeader {

            public int typeId;
            public int fieldCount;
            public int byteSize;

        }

        public struct SectionField {

            public int typeId;
            public int fieldSize;
            public int fieldOffset;

        }

        public void ReadSection() {
            SectionHeader sectionHead = ReadSectionHeader();
            int sectionLength = sectionHead.fieldCount;
        }

        public float ReadFloat(string name) {
            return 0f; //data.ReadFloatField(name);
        }

    }

    public struct SerializedThing {

        public int typeId;
        public float someFloatValue;

        public void Deserialize() { }

    }

    public class MissionSaveFile { }

    public class MissionFile {


        private Evaluator<WaypointContext> CreateWaypointDSE() {
            return new Evaluator<WaypointContext>();
        }

    }

    public class WeaponAssetFile { }

    public class EntityDefinitionFile { }

    public class Inspector {

        public const int TypeId_Float = 0;
        public const int TypeId_String = 1;
        public const int TypeId_Array = 2;

    }

}