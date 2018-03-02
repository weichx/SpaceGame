using UnityEngine;

namespace Freespace.POFModel {

    public class Header {

        public float maxRadius;
        public int objectFlags;
        public int subObjectCount;

        public Vector3 minBounding;
        public Vector3 maxBounding;
        public int[] detailLevelIndices;
        public int[] debrisCountIndices;

        public float mass;
        public Vector3 centerOfMass;
        public float[] momentOfInertia;

        public CrossSection[] crossSections;
        public MuzzleLight[] muzzleLights;

    }

}