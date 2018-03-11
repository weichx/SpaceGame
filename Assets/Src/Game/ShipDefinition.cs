using System;
using UnityEngine;

namespace SpaceGame {

    [Serializable]
    public class ShipDefinition {

        public string name;
//        public GameObject chassis;
//        public ShipClass shipClass;
        public float maxSpeed;
        public float turnRate;
        public float accelerationRate;

        public float hitpoints;
        public float shieldPoints;

        public Vector3 test;

    }

}