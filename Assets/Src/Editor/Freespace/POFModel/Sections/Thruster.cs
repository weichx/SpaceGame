using UnityEngine;

namespace Freespace.POFModel {

    public struct ThrusterGlow {

        public Vector3 position;
        public Vector3 normal;
        public float radius;

    }
    
    public class Thruster {

        public ThrusterGlow[] glows;
        public string properties;

    }

}