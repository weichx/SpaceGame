using System.Collections.Generic;
using UnityEngine;
using Weichx.Persistence;

namespace SpaceGame.FileTypes {

    public class GameDataFile : ScriptableObject {

        public string serializedShipDefinitions;

        private Dictionary<string, ShipDefinition> shipDefs;

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

    }

}