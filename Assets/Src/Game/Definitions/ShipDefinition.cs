using JetBrains.Annotations;
using SpaceGame.FileTypes;

namespace SpaceGame {

    public enum ShipType {

        Starfighter,
        Bomber,
        Frigate,
        Transport

    }

    public class ShipDefinition : AssetDefinition {

        public AssetPointer<Chassis> chassis;

        public float maxSpeed;
        public float turnRate;
        public float accelerationRate;

        public float hitpoints;
        public float shieldPoints;

        public ShipType shipType;

        [UsedImplicitly]
        public ShipDefinition() {}

        public ShipDefinition(int id) : base(id) {
            shipType = ShipType.Starfighter;
            name = $"Star Ship {id}";
        }

    }

}