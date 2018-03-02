using System.Collections.Generic;
using UnityEngine;

namespace Freespace.POFModel {

    public class SubObject {

        public int subModelNumber;
        public float radius;
        public int submodelParent;
        public Vector3 offset;
        public Vector3 geometricCenter;
        public Vector3 boundingBoxMin;
        public Vector3 boundingBoxMax;
        public string submodelName;
        public string properties;
        public int movementType;
        public int movementAxis;
        public int reserved;
        public byte[] bspData;

        public int[] textureIndices;
        public Mesh mesh;
       
    }

}