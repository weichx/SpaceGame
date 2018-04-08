using SpaceGame.FileTypes;
using UnityEditor;
using UnityEngine;
using Weichx.Persistence;

namespace SpaceGame.Editor.Windows {

    [InitializeOnLoad]
    public static class DatabaseEditorContainer {

        private static GameDatabase db;

        static DatabaseEditorContainer() {
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            EditorApplication.hierarchyWindowChanged += OnHierarchyWindowChanged;
            EditorApplication.projectWindowChanged += OnProjectWindowChanged;
        }

        private static void OnProjectWindowChanged() {
            db.UpdateProjectAssets();
        }
        
        private static void OnHierarchyWindowChanged() {
            db.UpdateSceneEntities();
        }

        public static void EnsureDatabase() {
            if (db != null) return;

            GameDataFile gameData = Resources.Load<GameDataFile>("Data/GameDatabase");
            if (gameData == null) {
                gameData = ScriptableObject.CreateInstance<GameDataFile>();
                AssetDatabase.CreateAsset(gameData, "Assets/Resources/Data/GameDatabase.asset");
                EditorUtility.SetDirty(gameData);
                AssetDatabase.SaveAssets();
            }

            db = Snapshot<GameDatabase>.Deserialize(gameData.database);

            GameDatabase.ActiveInstance = db;

            if (gameData.debugReset) {
                db = Snapshot<GameDatabase>.DeserializeDefault();
            }
            else {
                db = Snapshot<GameDatabase>.Deserialize(gameData.database);
            }
            db.UpdateSceneEntities();
            db.UpdateProjectAssets();
        }

        private static void OnAfterAssemblyReload() {
            EnsureDatabase();
        }

        private static void OnBeforeAssemblyReload() {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            EditorApplication.hierarchyWindowChanged -= OnHierarchyWindowChanged;
            EditorApplication.projectWindowChanged -= OnProjectWindowChanged;

            db.ClearSceneEntities();
            db.ClearProjectAssets();
            db = null;
        }

    }

}