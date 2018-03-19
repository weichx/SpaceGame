using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using SpaceGame.AI;
using UnityEditor;
using UnityEngine;
using Weichx.Persistence;

namespace SpaceGame.FileTypes {

    public class GameDataFile : ScriptableObject {

        [TextArea(20, 20)] [SerializeField] private string serializedEntityDefinitions = string.Empty;

        [TextArea(20, 20)] [SerializeField] private string serializedShipDefinitions = string.Empty;

        [TextArea(20, 20)] [SerializeField] private string serializedDecisionDefinitions = string.Empty;

        [TextArea(20, 20)] [SerializeField] private string serializedMissionDefinitions = string.Empty;

        private List<ShipDefinition> shipDefinitions;
        private List<EntityDefinition> entityDefinitions;
        private List<Decision> decisionDefinitions;
        private List<MissionDefinition> missionDefinitions;

        private bool isMissionDefDirty;
        private bool isShipDefDirty;
        private bool isEntityDefDirty;
        private bool isDecisionListDirty;

        //todo -- this needs help 
        public MissionDefinition GetMission(string missionGuid) {
            if (missionDefinitions == null || missionDefinitions.Count == 0) {
                missionDefinitions = Snapshot<List<MissionDefinition>>.Deserialize(serializedMissionDefinitions);
            }
            isMissionDefDirty = true;
            if (missionDefinitions.Count == 0) {
                missionDefinitions.Add(MissionDefinition.CreateMission());
            }
            
            return missionDefinitions[0];
        }

        [PublicAPI]
        public List<EntityDefinition> GetEntityDefinitions() {
            if (entityDefinitions == null || entityDefinitions.Count == 0) {
                entityDefinitions = Snapshot<List<EntityDefinition>>.Deserialize((serializedEntityDefinitions));
            }
            isEntityDefDirty = true;
            return entityDefinitions;
        }

        [PublicAPI]
        public List<ShipDefinition> GetShipDefintions() {
            if (shipDefinitions == null || shipDefinitions.Count == 0) {
                shipDefinitions = Snapshot<List<ShipDefinition>>.Deserialize(serializedShipDefinitions);
            }
            isShipDefDirty = true;
            return shipDefinitions;
        }

        [PublicAPI]
        public List<Decision> GetDecisions() {
            if (decisionDefinitions == null || decisionDefinitions.Count == 0) {
                decisionDefinitions = Snapshot<List<Decision>>.Deserialize(serializedDecisionDefinitions);
            }
            isDecisionListDirty = true;
            return decisionDefinitions;
        }

        [PublicAPI]
        public void Save() {
            Stopwatch watch = Stopwatch.StartNew();
            if (isShipDefDirty && shipDefinitions != null) {
                serializedShipDefinitions = Snapshot<List<ShipDefinition>>.Serialize(shipDefinitions);
            }
            if (isEntityDefDirty && entityDefinitions != null) {
                serializedEntityDefinitions = Snapshot<List<EntityDefinition>>.Serialize(entityDefinitions);
            }
            if (isDecisionListDirty && decisionDefinitions != null) {
                serializedDecisionDefinitions = Snapshot<List<Decision>>.Serialize(decisionDefinitions);
            }
            if (isMissionDefDirty && missionDefinitions != null) {
                serializedMissionDefinitions = Snapshot<List<MissionDefinition>>.Serialize(missionDefinitions);
            }
            isDecisionListDirty = false;
            isShipDefDirty = false;
            isMissionDefDirty = false;
            isEntityDefDirty = false;
            EditorUtility.SetDirty(this);
            watch.Stop();
            if (watch.ElapsedMilliseconds >= 10) {
                UnityEngine.Debug.Log($"Saving took {watch.ElapsedMilliseconds} milliseconds");
            }
        }

    }

}