using System.Collections.Generic;
using Weichx.Util;
using SpaceGame.Editor.Windows;
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

        private readonly string[] tabs = {
            "Missions",
            "Ships",
            "AI",
        };

        private void OnEnable() {

            state = MissionWindowState.Restore();
            
            if (GameDatabase.ActiveInstance != null) {
                GameDatabase.ActiveInstance.ClearSceneEntities();
            }
            
            DatabaseEditorContainer.EnsureDatabase();
            GameDatabase.ActiveInstance?.UpdateSceneEntities();
            GameDatabase.ActiveInstance?.UpdateProjectAssets();
            
            pages = new MissionWindowPage[] {
                new MissionPage(Save, state, GameDatabase.ActiveInstance),
                new ShipPage(state, GameDatabase.ActiveInstance),
                new AIPage(state, GameDatabase.ActiveInstance)
            };

            state.currentPageIndex = Mathf.Clamp(state.currentPageIndex, 0, pages.Length - 1);
            pages[state.currentPageIndex].OnEnable();

        }

        private void Save() {
            gameData = Resources.Load<GameDataFile>("Data/GameDatabase");
            gameData.database = Snapshot<GameDatabase>.Serialize(GameDatabase.ActiveInstance);
            EditorUtility.SetDirty(gameData);
            state.Save();
        }

        private void OnDisable() {
            pages[state.currentPageIndex].OnDisable();
            Save();
            GameDatabase.ActiveInstance.ClearSceneEntities();
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