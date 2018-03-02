using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpaceGame.Util {

    public static class GameObjectExtensions {

    #if UNITY_EDITOR

        public static bool IsPrefab(this GameObject gameObject) {
            return PrefabUtility.GetPrefabType(gameObject) == PrefabType.Prefab;
        }

    #endif

    }

}