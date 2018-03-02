using UnityEngine;

namespace Freespace.POFModel {

    public struct PositionNormal {

        public Vector3 point;
        public Vector3 normal;

        public PositionNormal(Vector3 point, Vector3 normal) {
            this.point = point;
            this.normal = normal;
        }

    }

}