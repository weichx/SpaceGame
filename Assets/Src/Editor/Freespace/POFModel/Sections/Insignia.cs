using UnityEngine;

namespace Freespace.POFModel {

    public struct InsigniaFace {

        public int vertexIndex0;
        public int vertexIndex1;
        public int vertexIndex2;
        public float u0, u1, u2;
        public float v0, v1, v2;

    }
    
    public class Insignia {

        public int detailLevel;
        public int faceCount;
        public int vertexCount;
        public Vector3[] vertices;
        public Vector3 offset;
        public InsigniaFace[] faces;

    }

}