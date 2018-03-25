using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using SpaceGame.AI;
using UnityEditor;
using UnityEngine;
using Weichx.Persistence;

namespace SpaceGame.FileTypes {

    public class GameDataFile : ScriptableObject {

//        public int idGenerator;
//
//        [TextArea(20, 20)] [SerializeField] private string serializedEntityDefinitions = string.Empty;
//
//        [TextArea(20, 20)] [SerializeField] private string serializedShipDefinitions = string.Empty;
//
//        [TextArea(20, 20)] [SerializeField] private string serializedDecisionDefinitions = string.Empty;
//
//        [TextArea(20, 20)] [SerializeField] private string serializedMissionDefinitions = string.Empty;
//        
//        [TextArea(20, 20)] [SerializeField] private string serializedFactionDefinitions = string.Empty;
//        
//        [TextArea(20, 20)] [SerializeField] private string serializedFlightGroupDefinitions = string.Empty;
//
//        private List<EntityDefinition> entityDefinitions;
//        private List<Decision> decisionDefinitions;
//        private List<MissionDefinition> missionDefinitions;
//
//        private bool isMissionDefDirty;
//        private bool isEntityDefDirty;
//        private bool isDecisionListDirty;
//        
        [TextArea(20, 40)]
        public string database;
//
//        //todo -- this needs help 
//        public MissionDefinition GetMission(string missionGuid) {
//            if (missionDefinitions == null || missionDefinitions.Count == 0) {
//                missionDefinitions = Snapshot<List<MissionDefinition>>.Deserialize(serializedMissionDefinitions);
//            }
//            isMissionDefDirty = true;
//            if (missionDefinitions.Count == 0) {
//                missionDefinitions.Add(MissionDefinition.CreateMission());
//            }
//
//            return missionDefinitions[0];
//        }
//
//        public List<T> Get<T>() where T : AssetDefinition {
//            if (typeof(T) == typeof(ShipDefinition)) {
//                return Snapshot<List<T>>.Deserialize(serializedShipDefinitions);
//            }
//            else if (typeof(T) == typeof(EntityDefinition)) {
//                return Snapshot<List<T>>.Deserialize(serializedEntityDefinitions);
//            }
//            else if (typeof(T) == typeof(FactionDefinition)) {
//                return Snapshot<List<T>>.Deserialize(serializedFactionDefinitions);
//            }
//            else if (typeof(T) == typeof(FlightGroupDefinition)) {
//                return Snapshot<List<T>>.Deserialize(serializedFlightGroupDefinitions);
//            }
//            else if (typeof(T) == typeof(Decision)) {
//                return Snapshot<List<T>>.Deserialize(serializedDecisionDefinitions);
//            }
//            return null;
//        }
//
//        [PublicAPI]
//        public List<EntityDefinition> GetEntityDefinitions() {
//            if (entityDefinitions == null || entityDefinitions.Count == 0) {
//                entityDefinitions = Snapshot<List<EntityDefinition>>.Deserialize((serializedEntityDefinitions));
//            }
//            isEntityDefDirty = true;
//            return entityDefinitions;
//        }
//
//        [PublicAPI]
//        public List<ShipDefinition> GetShipDefintions() {
//            return Snapshot<List<ShipDefinition>>.Deserialize(serializedShipDefinitions);
//        }
//
//        public void SaveShipDefinitions(List<ShipDefinition> shipDefinitions) {
//            serializedShipDefinitions = Snapshot<List<ShipDefinition>>.Serialize(shipDefinitions);
//        }
//
//        [PublicAPI]
//        public List<Decision> GetDecisions() {
//            if (decisionDefinitions == null || decisionDefinitions.Count == 0) {
//                decisionDefinitions = Snapshot<List<Decision>>.Deserialize(serializedDecisionDefinitions);
//            }
//            isDecisionListDirty = true;
//            return decisionDefinitions;
//        }
//
//        [PublicAPI]
//        public void Save() {
//            Stopwatch watch = Stopwatch.StartNew();
//            if (isEntityDefDirty && entityDefinitions != null) {
//                serializedEntityDefinitions = Snapshot<List<EntityDefinition>>.Serialize(entityDefinitions);
//            }
//            if (isDecisionListDirty && decisionDefinitions != null) {
//                serializedDecisionDefinitions = Snapshot<List<Decision>>.Serialize(decisionDefinitions);
//            }
//            if (isMissionDefDirty && missionDefinitions != null) {
//                serializedMissionDefinitions = Snapshot<List<MissionDefinition>>.Serialize(missionDefinitions);
//            }
//            isDecisionListDirty = false;
//            isMissionDefDirty = false;
//            isEntityDefDirty = false;
//            EditorUtility.SetDirty(this);
//            watch.Stop();
//            if (watch.ElapsedMilliseconds >= 10) {
//                UnityEngine.Debug.Log($"Saving took {watch.ElapsedMilliseconds} milliseconds");
//            }
//        }

    }

}