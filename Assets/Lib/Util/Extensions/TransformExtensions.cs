using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Weichx.Util {

    public static class TransformExtensions {

        public static void Reset(this Transform t) {
            t.rotation = Quaternion.identity;
            t.position = Vector3.zero;
            t.localScale = Vector3.one;
        }

        public static void ResetLocal(this Transform t) {
            t.localRotation = Quaternion.identity;
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
        }

        public static float DistanceTo(this Transform self, Transform other) {
            return (other.position - self.position).magnitude;
        }

        public static float DistanceTo(this Transform self, GameObject other) {
            return (other.transform.position - self.position).magnitude;
        }

        public static float DistanceTo(this Transform self, Vector3 position) {
            return (position - self.position).magnitude;
        }

        public static float DistanceToSquared(this Transform self, Transform other) {
            return (other.position - self.position).sqrMagnitude;
        }

        public static float DistanceToSquared(this Transform self, GameObject other) {
            return (other.transform.position - self.position).sqrMagnitude;
        }

        public static float DistanceToSquared(this Transform self, Vector3 other) {
            return (other - self.position).sqrMagnitude;
        }

        public static Transform[] GetChildren(this Transform self, Transform[] output = null) {
            if (output == null) {
                output = new Transform[self.childCount];
            }

            for (int i = 0; i < output.Length; i++) {
                output[i] = self.GetChild(i);
            }

            return output;
        }

        public static void Traverse(this Transform self, Action<Transform> callback) {
            int childCount = self.childCount;
            for (int i = 0; i < childCount; i++) {
                Transform child = self.GetChild(i);
                callback(child);
                child.Traverse(callback);
            }
        }

        public static void ForEachChild(this Transform transform, Action<Transform> action) {
            int count = transform.childCount;
            for (int i = 0; i < count; i++) {
                action(transform.GetChild(i));
            }
        }

        public static void ForEachChildIndexed(this Transform transform, Action<Transform, int> action) {
            int count = transform.childCount;
            for (int i = 0; i < count; i++) {
                action(transform.GetChild(i), i);
            }
        }

        public static T[] MapChildren<T>(this Transform transform, Func<Transform, T> action) {
            int count = transform.childCount;
            T[] retn = new T[count];
            for (int i = 0; i < count; i++) {
                retn[i] = action(transform.GetChild(i));
            }

            return retn;
        }

        public static T[] MapChildrenIndexed<T>(this Transform transform, Func<Transform, int, T> action) {
            int count = transform.childCount;
            T[] retn = new T[count];
            for (int i = 0; i < count; i++) {
                retn[i] = action(transform.GetChild(i), i);
            }

            return retn;
        }

        public static void ClearChildren(this Transform transform) {
            while (transform.childCount != 0) {
                Object.Destroy(transform.GetChild(0).gameObject);
            }
        }

        public static void ClearChildrenImmediate(this Transform transform) {
            while (transform.childCount != 0) {
                Object.DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

    }

}