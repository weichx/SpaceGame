using System.Collections.Generic;
using UnityEngine;
using Weichx.Persistence;

namespace SpaceGame.FileTypes {

    public class GameDataFile : ScriptableObject {

        public string serializedShipDefinitions;
        [SerializeField] private string serializedEntityDefinitions = "";

        private Dictionary<string, ShipDefinition> shipDefs;
        private List<EntityDefinition> entityDefinitions;

        private void OnEnable() {
            if (entityDefinitions == null) entityDefinitions = new List<EntityDefinition>(32);
        }

        public void CreateOrReplaceShipDefinition(string name, ShipDefinition shipDefinition) {
            if (shipDefs == null) {
                if (!string.IsNullOrEmpty(serializedShipDefinitions)) {
                    Snapshot<Dictionary<string, ShipDefinition>> snapshot = Snapshot<Dictionary<string, ShipDefinition>>.FromString(serializedShipDefinitions);
                    shipDefs = snapshot.Deserialize();
                }
                else {
                    shipDefs = new Dictionary<string, ShipDefinition>();
                }
            }
            shipDefs[name] = shipDefinition;
            serializedShipDefinitions = new Snapshot<Dictionary<string, ShipDefinition>>(shipDefs).Serialize();
        }

        public void GetShipDefinitions() {
            if (shipDefs == null) {
                if (!string.IsNullOrEmpty(serializedShipDefinitions)) {
                    Snapshot<Dictionary<string, ShipDefinition>> snapshot = Snapshot<Dictionary<string, ShipDefinition>>.FromString(serializedShipDefinitions);
                    shipDefs = snapshot.Deserialize();
                }
                else {
                    shipDefs = new Dictionary<string, ShipDefinition>();
                }
            }
        }

        public void AddEntityDefinition(EntityDefinition entityDefinition) {
            entityDefinitions.Add(entityDefinition);

        }

        public List<EntityDefinition> LoadEntityDefinitions() {
            if (serializedEntityDefinitions != string.Empty) {
                return Snapshot<List<EntityDefinition>>.FromString(serializedEntityDefinitions).Deserialize();
            }
            else {
                return new List<EntityDefinition>(entityDefinitions);
            }
        }

        public void SaveEntityDefinitions() {
            serializedEntityDefinitions = new Snapshot<List<EntityDefinition>>(entityDefinitions).Serialize();
        }

    }

}