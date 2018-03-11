using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace SpaceGame.Util {

    public static class GameObjectExtensions {

        public static bool IsPrefab(this GameObject gameObject) {
        #if UNITY_EDITOR
            return PrefabUtility.GetPrefabType(gameObject) == PrefabType.Prefab;
        #else
            return false;
        #endif

        }

    }

}