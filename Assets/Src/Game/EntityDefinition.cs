﻿using System;
using UnityEngine;

namespace SpaceGame {

    public struct AssetPointer<T> {

        public string assetGuid;

        public AssetPointer(string assetGuid) {
            this.assetGuid = assetGuid;
        }

    }

    //this allows type safety and a custom inspector
    public struct FactionReference {

        public readonly int factionId;

        public FactionReference(int factionId) {
            this.factionId = factionId;
        }

    }
    
    [Serializable]
    public class EntityDefinition {

        public string name;
        public string callsign;
        public string shipType;
        public string flightGroup;
        public FactionReference faction;

        public AssetPointer<GameObject> chassis;
        
        [HideInInspector] public string guid;

        public EntityDefinition() {
            this.name = string.Empty;
            this.callsign = string.Empty;
            this.shipType = string.Empty;
            this.flightGroup = string.Empty;
            this.faction = new FactionReference(0);
            this.guid = string.Empty;
        }
        
        public EntityDefinition(Entity entity) {
            this.name = entity.name;
            this.callsign = "";
            this.shipType = "";
            this.flightGroup = "";
            this.faction = new FactionReference(entity.factionId);
            this.guid = entity.guid;
        }

    }

}