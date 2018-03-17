using UnityEngine;

namespace Freespace.POFModel {

    public class ShieldData {

        public Vector3[] vertices;
        public Face[] faces;
        public byte[] collisionBSP;

    }    

    public struct Face {

        public Vector3 normal;
        public int[] vertexIndices;
        public int[] neighborIndices;

    }

}