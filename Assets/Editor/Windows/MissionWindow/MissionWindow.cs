using System.Collections.Generic;
using Lib.Util;
using SpaceGame.FileTypes;
using UnityEditor;
using UnityEngine;
using Weichx.Persistence;

namespace SpaceGame.Editor.MissionWindow {

    public class MissionWindow : EditorWindow {

        private MissionWindowState state;
        private GameDataFile gameData;
        private MissionWindowPage[] pages;
        private ListX<Entity> sceneEntities;
        private Dictionary<Entity, int> entityToId;
        private GameDatabase db;

        private readonly string[] tabs = {
            "Missions",
            "Ships",
            "AI",
        };

        private void OnEnable() {
            gameData = Resources.Load<GameDataFile>("Data/GameDatabase");
            if (gameData == null) {
                gameData = CreateInstance<GameDataFile>();
                AssetDatabase.CreateAsset(gameData, "Assets/Resources/Data/GameDatabase.asset");
                EditorUtility.SetDirty(gameData);
                AssetDatabase.SaveAssets();
            }          

            state = MissionWindowState.Restore();

            if (gameData.debugReset) {
                db = Snapshot<GameDatabase>.DeserializeDefault();
            }
            else {
                db = Snapshot<GameDatabase>.Deserialize(gameData.database);
            }
            
            GameDatabase.ActiveInstance = db;
            db.UpdateSceneEntities();

            pages = new MissionWindowPage[] {
                new MissionPage(Save, state, db),
                new ShipPage(state, db),
                new AIPage(state, db)
            };
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;

            state.currentPageIndex = Mathf.Clamp(state.currentPageIndex, 0, pages.Length - 1);
            pages[state.currentPageIndex].OnEnable();

        }

        private void Save() {
            gameData.database = Snapshot<GameDatabase>.Serialize(db);
            EditorUtility.SetDirty(gameData);
            state.Save();
        }

        private void OnHierarchyChange() {
            db.UpdateSceneEntities();
        }

        private void OnBeforeAssemblyReload() {
            db.ClearSceneEntities();
        }
       
        private void OnAfterAssemblyReload() { }

        private void OnDisable() {
            pages[state.currentPageIndex].OnDisable();
            Save();
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }

        private void OnInspectorUpdate() {
            Repaint();
        }

        public void OnGUI() {
            if (state == null) return;

            if (pages == null) return;

            int lastPage = state.currentPageIndex;

            state.currentPageIndex = GUILayout.Toolbar(lastPage, tabs, (GUILayoutOption[]) null);

            state.currentPageIndex = Mathf.Clamp(state.currentPageIndex, 0, pages.Length - 1);

            if (lastPage != state.currentPageIndex) {
                pages[lastPage].OnDisable();
                pages[state.currentPageIndex].OnEnable();
            }
            pages[state.currentPageIndex].OnGUI();
        }

    }

}

/*

Ship Details Page
    - Define Ship Stats
    - Define Load out options
    - Define Chassis
Entity Page
    - Faction
    - Ship Type
    - Call Sign
    - Active in Mission
    - Cargo
    - Starting Stats / Overrides
    - Flight Group
    - AI Packages
    - Goals
AI Page
    - Build AI Behaviors
    - Apply Considerations
    
Weapons Page
    -  

*/