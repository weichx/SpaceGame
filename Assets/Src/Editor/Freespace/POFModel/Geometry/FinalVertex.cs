using UnityEngine;

namespace Freespace.POFModel.Geometry {


    internal class FinalVertex {

        public Vector3 position;
        public Vector3 normal;
        public Vector2 uvCoord;
        public int triangleIndex;

        public FinalVertex(Vertex vertex, int triangleIndex) {
            position = vertex.point;
            normal = vertex.normal;
            uvCoord = vertex.uvCoords;
            this.triangleIndex = triangleIndex;
        }

    }

}