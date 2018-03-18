using System.Collections.Generic;
using SpaceGame.AI;
using UnityEditor;
using UnityEngine;
using Weichx.Persistence;

namespace SpaceGame.FileTypes {

    public class GameDataFile : ScriptableObject {

        [TextArea(20, 20)]
        [SerializeField] private string serializedEntityDefinitions = string.Empty;
        
        [TextArea(20, 20)]
        [SerializeField] private string serializedShipDefinitions = string.Empty;
        
        [TextArea(20, 20)]
        [SerializeField] private string serializedDecisionDefinitions = string.Empty;

        private List<ShipDefinition> shipDefinitions;
        private List<EntityDefinition> entityDefinitions;
        private List<Decision> decisionDefinitions;

        private void OnEnable() {
            if (entityDefinitions == null || entityDefinitions.Count == 0) {
                entityDefinitions = Snapshot<List<EntityDefinition>>.Deserialize((serializedEntityDefinitions));
            }
            if (shipDefinitions == null || shipDefinitions.Count == 0) {
                shipDefinitions = Snapshot<List<ShipDefinition>>.Deserialize(serializedShipDefinitions);
            }
            if (decisionDefinitions == null || decisionDefinitions.Count == 0) {
                decisionDefinitions = Snapshot<List<Decision>>.Deserialize(serializedDecisionDefinitions);
            }
        }

        public List<ShipDefinition> GetShipDefintions() {
            OnEnable();
            return shipDefinitions;
        }

        public List<Decision> GetDecisions() {
            OnEnable();
            return decisionDefinitions;
        }

        public void Save(List<Decision> decisionList) {
            this.decisionDefinitions = new List<Decision>(decisionList);
            serializedDecisionDefinitions = Snapshot<List<Decision>>.Serialize(decisionList);
            EditorUtility.SetDirty(this);
        }

        public void Save(List<ShipDefinition> shipListValue) {
            this.shipDefinitions = new List<ShipDefinition>(shipListValue);
            serializedShipDefinitions = Snapshot<List<ShipDefinition>>.Serialize(shipListValue);
            EditorUtility.SetDirty(this);
        }

    }

}