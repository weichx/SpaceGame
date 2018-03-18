using System;
using UnityEngine;
using Weichx.ReflectionAttributes;

namespace SpaceGame {
    
    [Serializable]
    public class ShipDefinition : IIdentitifiable {

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
            this.name = "Unnamed Ship Def";
        }

        public int Id => id;
        public string Name => name;

    }

}