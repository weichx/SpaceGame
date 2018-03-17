using System;
using UnityEngine;

namespace SpaceGame {

    [Serializable]
    public class ShipDefinition {

        [NonSerialized][HideInInspector]
        public readonly int id;
        
        public string name;
        public AssetPointer<Chassis> chassis;
        public float maxSpeed;
        public float turnRate;
        public float accelerationRate;

        public float hitpoints;
        public float shieldPoints;

        private static int idGenerator;
        
        public ShipDefinition() {
            this.id = idGenerator++;
        }

    }

}