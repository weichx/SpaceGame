using System;
using System.Collections.Generic;
using Weichx.Util;
using UnityEngine;
using Weichx.Persistence;
using Weichx.ReflectionAttributes;

namespace SpaceGame.FileTypes {

    public class MissionDataFile : ScriptableObject {

        [ReadOnly]
        public List<MissionContainer> serializedMissions;

        [Serializable]
        public class MissionContainer {

            [ReadOnly]
            public string guid;
            [ReadOnly]
            public string serializedMission;

            [ReadOnly]
            public int entityCount;
            
            public MissionContainer(string guid, string serializedMission) {
                this.guid = guid;
                this.serializedMission = serializedMission;
            }

        }

        public int MissionCount => serializedMissions.Count;

        public MissionDefinition GetMission(string missionGuid) {
            MissionContainer container = serializedMissions.Find(missionGuid, (m, g) => m.guid == g);
            if (container != null) {
                return Snapshot<MissionDefinition>.Deserialize(container.serializedMission);
            }
            return null;
        }

        public List<MissionDefinition> GetMissions() {
            if (serializedMissions.Count == 0) {
                List<MissionDefinition> missions = new List<MissionDefinition>(4);
                missions.Add(MissionDefinition.CreateMission());
                return missions;
            }
            return serializedMissions.Map((container) => {
                return Snapshot<MissionDefinition>.Deserialize(container.serializedMission);
            });
        }

//        public void SaveMission(MissionDefinition missionDefinition) {
//            if (missionDefinition == null) return;
//            string serialized = Snapshot<MissionDefinition>.Serialize(missionDefinition);
//            for (int i = 0; i < serializedMissions.Count; i++) {
//                if (serializedMissions[i].guid == missionDefinition.guid) {
//                    serializedMissions[i].serializedMission = serialized;
//                    serializedMissions[i].entityCount = missionDefinition.entityDefinitions.Count;
//                    return;
//                }
//            }
//            serializedMissions.Add(new MissionContainer(missionDefinition.guid, serialized));
//        }
        
        public static MissionDataFile Create(string name) {
            MissionDataFile asset = CreateInstance<MissionDataFile>();
            asset.name = name;
            return asset;
        }

    }

}