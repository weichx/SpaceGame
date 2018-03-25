using UnityEngine;

namespace SpaceGame {

    public struct TransformInfo {

        public readonly int entityId;
        public Vector3 position;
        public Quaternion rotation;

        public TransformInfo(Entity entity) {
            this.entityId = entity.index;
            this.position = entity.transform.position;
            this.rotation = entity.transform.rotation;
        }

        public TransformInfo(int entityId, Vector3 position, Quaternion rotation) {
            this.entityId = entityId;
            this.position = position;
            this.rotation = rotation;
        }

        public Vector3 forward => rotation * Vector3.forward;
        public Vector3 right => rotation * Vector3.right;
        public Vector3 up => rotation * Vector3.up;

        public float DistanceTo(TransformInfo other) {
            return (other.position - position).magnitude;
        }

        public float DistanceTo(Vector3 position) {
            return (position - position).magnitude;
        }

        public float DistanceToSquared(TransformInfo other) {
            return (other.position - position).sqrMagnitude;
        }

        public float DistanceToSquared(Vector3 other) {
            return (other - position).sqrMagnitude;
        }

        public Vector3 DirectionTo(TransformInfo other) {
            return (other.position - position).normalized;
        }

        public Vector3 DirectionTo(Vector3 other) {
            return (other - position).normalized;
        }

    }

}