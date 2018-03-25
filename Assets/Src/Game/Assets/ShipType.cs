using JetBrains.Annotations;
using SpaceGame.Assets;
using SpaceGame.FileTypes;
using UnityEngine;

namespace SpaceGame {

    public enum ShipCategory {

        SmallFighter,
        Starfighter,
        Bomber,
        Frigate,
        Transport

    }

    public class ShipType : GameAsset {

        public AssetPointer<Chassis> chassis;

        public float maxSpeed;
        public float turnRate;
        public float accelerationRate;

        public float hitpoints;
        public float shieldPoints;

        [HideInInspector] public int shipGroupId;

        [UsedImplicitly] private ShipType() { }
        
        private ShipType(int id, string name) : base(id, name) { }

    }

}