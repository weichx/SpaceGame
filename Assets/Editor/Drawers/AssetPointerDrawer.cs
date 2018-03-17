using SpaceGame;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace Src.Editor.Drawers {

    [PropertyDrawerFor(typeof(AssetPointer<GameObject>))]
    public class GameObjectAssetPointerDrawer : ReflectedPropertyDrawer {

        private GameObject asset;
        
        public override void OnGUI(ReflectedProperty property, GUIContent label = null) {
            string assetGUID = ((AssetPointer<GameObject>)property.Value).assetGuid;
            if (!string.IsNullOrEmpty(assetGUID) && asset == null) {
                string path = AssetDatabase.GUIDToAssetPath(assetGUID);
                asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            GameObject lastAsset = asset;
            asset = EditorGUILayout.ObjectField(property.GUIContent, lastAsset, typeof(GameObject), false) as GameObject;
            if (lastAsset != asset) {
                if (asset != null) {
                    string path = AssetDatabase.GetAssetPath(asset.GetInstanceID());
                    property.Value = new AssetPointer<GameObject>(AssetDatabase.AssetPathToGUID(path));
                }
                else {
                    property.Value = default(AssetPointer<GameObject>);
                }
            }
        }

    }

}