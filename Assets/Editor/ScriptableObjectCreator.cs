﻿
namespace SpaceGame.FileTypes {

    using UnityEngine;
    using UnityEditor;
    using System.IO;

    public static class ScriptableObjectUtility {

        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static void CreateAsset<T>() where T : ScriptableObject {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "") {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "") {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T) + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        
        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static T CreateAsset<T>(string name, string path) where T : ScriptableObject {
            T asset = ScriptableObject.CreateInstance<T>();

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            return asset;
        }

        [MenuItem("Assets/Custom/Create Game Data File")]
        public static void CreateShipDefinitionAsset() {
            CreateAsset<GameDataFile>();
        }
        
     
        [MenuItem("Assets/Custom/Clone Editor Skin")]
        public static void CloneEditorSkin() {
            GUISkin skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
            GUISkin skin2 = Object.Instantiate(skin);
            AssetDatabase.CreateAsset(skin2, "Assets/EditorSkin.asset");
        }

    }

}