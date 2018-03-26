using System;
using SpaceGame;
using SpaceGame.EditorComponents;
using SpaceGame.FileTypes;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(AssetPointer<Chassis>), PropertyDrawerForOption.IncludeSubclasses)]
    public class AssetPointerGameObjectDrawer : ReflectedPropertyDrawer {

        private Type assetType;

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            AssetPointer<Chassis> pointer = (AssetPointer<Chassis>) property.Value;
            GameObject asset = pointer.GetAsset()?.gameObject;
            GameObject lastAsset = asset;

            asset = EditorGUI.ObjectField(position, property.GUIContent, lastAsset, typeof(GameObject), false) as GameObject;

            if (lastAsset == asset) return;

            if (asset == null || !InResourcePath(asset) || !HasComponent(asset)) {
                property.Value = new AssetPointer<Chassis>();
                return;
            }
            string path = AssetDatabase.GetAssetPath(asset.GetInstanceID());
            property.Value = new AssetPointer<Chassis>(path);
        }

        private bool HasComponent(GameObject asset) {
            if (asset.GetComponent<Chassis>() == null) {
                EditorUtility.DisplayDialog("Invalid asset type",
                    $"You must provide a prefab with a root level {typeof(Chassis).Name} component", "Got it");
                return false;
            }
            return true;
        }

        private static bool InResourcePath(GameObject asset) {
            string path = AssetDatabase.GetAssetPath(asset.GetInstanceID());
            string resourcePath = AssetPointer<GameObject>.CleanPath(path);
            if (Resources.Load<GameObject>(resourcePath) == null) {
                EditorUtility.DisplayDialog(
                    "Invalid asset location",
                    "The asset must be located in a Resources directory",
                    "Got it"
                );
                return false;
            }
            return true;
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            return EditorGUIX.singleLineHeight;
        }

    }
    
    [PropertyDrawerFor(typeof(AssetPointer<Texture2D>), PropertyDrawerForOption.IncludeSubclasses)]
    public class AssetPointerTextureDrawer : ReflectedPropertyDrawer {

        private Type assetType;

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {
            AssetPointer<Texture2D> pointer = (AssetPointer<Texture2D>) property.Value;
            Texture2D asset = pointer.GetAsset();
            Texture2D lastAsset = asset;

            asset = EditorGUI.ObjectField(position, property.GUIContent, lastAsset, typeof(Texture2D), false) as Texture2D;

            if (lastAsset == asset) return;

            if (asset == null || !InResourcePath(asset)) {
                property.Value = new AssetPointer<Texture2D>();
                return;
            }
            string path = AssetDatabase.GetAssetPath(asset.GetInstanceID());
            property.Value = new AssetPointer<Texture2D>(path);
        }
       
        private static bool InResourcePath(Texture2D asset) {
            string path = AssetDatabase.GetAssetPath(asset.GetInstanceID());
            string resourcePath = AssetPointer<Texture2D>.CleanPath(path);
            if (Resources.Load<Texture2D>(resourcePath) == null) {
                EditorUtility.DisplayDialog(
                    "Invalid asset location",
                    "The asset must be located in a Resources directory",
                    "Got it"
                );
                return false;
            }
            return true;
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            return EditorGUIX.singleLineHeight;
        }

    }

}