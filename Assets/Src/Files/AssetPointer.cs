using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpaceGame.FileTypes {

    public struct AssetPointer<T> where T : Object {

        public readonly string assetPath;

        private bool attemptedFetch;
        private T asset;

        public AssetPointer(T asset) {
            this.asset = asset;
            this.assetPath = string.Empty;
            this.attemptedFetch = true;
        }

        public AssetPointer(string assetPath) {
            this.asset = null;
            this.assetPath = CleanPath(assetPath);
            this.attemptedFetch = false;
        }

        public static string CleanPath(string s) {
            if (s.StartsWith("Assets/Resources/")) {
                s = s.Substring("Assets/Resources/".Length);
            }
            return Path.ChangeExtension(s, null);
        }

        public T GetAsset() {
            if (!attemptedFetch && !string.IsNullOrEmpty(assetPath)) {
                attemptedFetch = true;
                asset = Resources.Load<T>(assetPath);
            #if UNITY_EDITOR
                if (asset != null) return asset;
                string guid = AssetDatabase.AssetPathToGUID(assetPath);
                asset = AssetDatabase.LoadAssetAtPath<T>(guid);
                if (asset != null) {
                    Debug.Log($"POINTING TO ASSET NOT FOUND IN RESOURCE DIRECTORY: {assetPath}");
                }

            #endif
            }
            return asset;
        }

        public static T GetAsset(string assetPath) {
            return new AssetPointer<T>(assetPath).GetAsset();
        }

    }

    public struct TextureAssetPointer {

        public readonly string assetPath;

        private bool attemptedFetch;
        private Texture2D asset;

        public TextureAssetPointer(string assetPath) {
            this.asset = null;
            this.assetPath = assetPath;
            this.attemptedFetch = false;
        }

        public Texture2D GetAsset() {
            if (!attemptedFetch && !string.IsNullOrEmpty(assetPath)) {
                attemptedFetch = true;
                asset = Resources.Load<Texture2D>(assetPath);
            }
            return asset;
        }

        public static Texture2D GetAsset(string assetGuid) {
            return Resources.Load<Texture2D>(assetGuid);
        }

    }

}