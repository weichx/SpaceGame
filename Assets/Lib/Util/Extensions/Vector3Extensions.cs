using UnityEngine;

namespace Weichx.Util {

    public static class Vector3Extensions {

        public static float AnglePreNormalized(this Vector3 self, Vector3 to) {
            return Mathf.Acos(Mathf.Clamp(Vector3.Dot(self, to), -1f, 1f)) * 57.29578f;
        }

        public static Vector3 From(this Vector3 self, Vector3 other) {
            return self - other;
        }

        public static Vector3 DirectionFrom(this Vector3 self, Vector3 other) {
            return (self - other).normalized;
        }

        public static Vector3 To(this Vector3 self, Vector3 other) {
            return other - self;
        }

        public static Vector3 DirectionTo(this Vector3 self, Vector3 other) {
            return (other - self).normalized;
        }

        public static float Dot(this Vector3 self, Vector3 other) {
            return Vector3.Dot(self, other);
        }

        public static float DistanceTo(this Vector3 self, Vector3 other) {
            return (other - self).magnitude;
        }

        public static float DistanceToSquared(this Vector3 self, Vector3 other) {
            return (other - self).sqrMagnitude;
        }

    }

}