using UnityEngine;

namespace Freespace.POFModel {

    public class HullLight {

        public int displayTime;
        public int onTime;
        public int offTime;
        public int parentIndex;
        public int lod;
        public int type;
        public string properties;
        public HullLightPoint[] lights;

    }

    public struct HullLightPoint {

        public Vector3 point;
        public Vector3 normal;
        public float radius;

    }
}