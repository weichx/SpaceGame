using System;
using UnityEngine;

namespace SpaceGame {



    //this allows type safety and a custom inspector
    public struct FactionReference {

        public readonly int factionId;

        public FactionReference(int factionId) {
            this.factionId = factionId;
        }

    }

    [Serializable]
    public class EntityDefinition : AssetDefinition {

        [HideInInspector] public int factionId;
        [HideInInspector] public int flightGroupId;
        
        public string callsign;
        public string shipType;
        
        public EntityDefinition() {
            this.name = $"Entity {id}";
            this.callsign = string.Empty;
            this.shipType = string.Empty;
        }

        public EntityDefinition(string name) : base(name) {
            this.callsign = "";
            this.shipType = "";
        }

    }

}