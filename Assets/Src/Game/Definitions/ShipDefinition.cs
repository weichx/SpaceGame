using System;
using SpaceGame.FileTypes;

namespace SpaceGame {
    
    [Serializable]
    public class ShipDefinition : AssetDefinition {

        public AssetPointer<Chassis> chassis;
        
        public float maxSpeed;
        public float turnRate;
        public float accelerationRate;

        public float hitpoints;
        public float shieldPoints;

        private static int idGenerator;
        
        public ShipDefinition() {
            this.name = "Unnamed Ship Def";
        }

    }

}