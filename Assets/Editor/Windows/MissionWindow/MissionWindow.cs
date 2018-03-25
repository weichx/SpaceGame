using System;
using System.Collections.Generic;
using SpaceGame.FileTypes;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Weichx.Persistence;

namespace SpaceGame.Editor.MissionWindow {

    public class MissionWindow : EditorWindow {

        private MissionWindowState state;
        private GameDataFile gameData;
        private MissionWindowPage[] pages;
        private List<Entity> sceneEntities;
        private Dictionary<Entity, string> entityToId;
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

            LoadSceneEntities();
            entityToId = new Dictionary<Entity, string>();
            foreach (Entity entity in sceneEntities) {
                if (string.IsNullOrEmpty(entity.guid)) {
                    entity.guid = Guid.NewGuid().ToString();
                }
                entityToId.Add(entity, entity.guid);
                EditorUtility.SetDirty(entity);
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

            state = MissionWindowState.Restore();

            if (gameData.debugReset) {
                db = Snapshot<GameDatabase>.DeserializeDefault();
            }
            else {
                db = Snapshot<GameDatabase>.Deserialize(gameData.database);
            }
            GameDatabase.ActiveInstance = db;

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
            LoadSceneEntities();
            bool didChange = false;
            for (int i = 0; i < sceneEntities.Count; i++) {
                Entity entity = sceneEntities[i];
                string id = entity.guid;
                string storedId;
                if (entityToId.TryGetValue(entity, out storedId)) {
                    if (id != storedId) {
                        Debug.Assert(false, $"Should never hit this. Expected {id} to be {storedId}");
                    }
                }
                else {
                    string newGuid = Guid.NewGuid().ToString();
                    if (entity.guid != "--default--") {
                        Debug.Log("Duplicated");
                        // gameData.GetMission(state.activeMissionGuid).CloneEntityDefinition(entity.guid, newGuid);
                    }
                    else {
                        Debug.Log("Created");
                        //gameData.GetMission(state.activeMissionGuid).CreateEntityDefinition(newGuid);
                    }
                    entity.guid = newGuid;
                    entityToId.Add(entity, entity.guid);
                    EditorUtility.SetDirty(entity);
                    didChange = true;
                }
            }
            if (didChange) {
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }

        private void OnBeforeAssemblyReload() { }

        private void LoadSceneEntities() {
            if (sceneEntities == null) sceneEntities = new List<Entity>();
            sceneEntities.Clear();

            Entity[] entities = Resources.FindObjectsOfTypeAll<Entity>();

            for (int i = 0; i < entities.Length; i++) {
                if (!EditorUtility.IsPersistent(entities[i])) {
                    sceneEntities.Add(entities[i]);
                }
            }

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