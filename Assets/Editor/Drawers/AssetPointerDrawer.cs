using System;
using SpaceGame;
using SpaceGame.EditorComponents;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(AssetPointer<Chassis>), PropertyDrawerForOption.IncludeSubclasses)]
    public class AssetPointerChassisDrawer : ReflectedPropertyDrawer {

        private GameObject asset;
        private Type assetType;

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {
            string assetGUID = ((AssetPointer<Chassis>) property.Value).assetGuid;
            if (!string.IsNullOrEmpty(assetGUID) && asset == null) {
                string path = AssetDatabase.GUIDToAssetPath(assetGUID);
                asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            GameObject lastAsset = asset;
            asset = EditorGUI.ObjectField(position, property.GUIContent, lastAsset, typeof(GameObject), false) as GameObject;
            if (lastAsset != asset) {
                if (asset != null) {
                    if (asset.GetComponent<Chassis>() == null) {
                        EditorUtility.DisplayDialog("Invalid asset type",
                            $"You must provide a prefab with a root level {typeof(Chassis).Name} component", "Got it");
                        asset = lastAsset;
                        return;
                    }
                    string path = AssetDatabase.GetAssetPath(asset.GetInstanceID());
                    property.Value = new AssetPointer<Chassis>(AssetDatabase.AssetPathToGUID(path));
                }
                else {
                    property.Value = default(AssetPointer<Chassis>);
                }
            }
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            return EditorGUIX.singleLineHeight;
        }

    }

}