using System.Runtime.CompilerServices;
using UnityEngine;

namespace Weichx.Util {

    public static class QuaternionExtensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetForward(this Quaternion rotation) {
            return rotation * Vector3.forward;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetRight(this Quaternion rotation) {
            return rotation * Vector3.right;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetUp(this Quaternion rotation) {
            return rotation * Vector3.up;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 InverseTransformDirection(this Quaternion rotation, Vector3 direction) {
            return Quaternion.Inverse(rotation) * direction;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 TransformDirection(this Quaternion rotation, Vector3 direction) {
            return Quaternion.Inverse(rotation) * direction;
        }
        

    }

}