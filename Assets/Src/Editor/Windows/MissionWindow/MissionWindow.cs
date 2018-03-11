using System;
using System.Collections.Generic;
using System.Linq;
using SpaceGame.Editor.GUIComponents;
using SpaceGame.FileTypes;
using SpaceGame.Util;
using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor.Windows {

    public class MissionWindow : EditorWindow {

        private List<Entity> entitites;
        private GameDataFile gameDataFile;
        private ShipDefinition shipDefinition;
        private GUISkin skin;

        private readonly string[] tabs = {"Entities", "Ships"};

        private List<ClickableLabel<Entity>> entityLabels;
        private ClickableLabel<Entity> selected;
        private Entity selectedEntity;

        [MenuItem("Window/Mission Editor")]
        private static void Init() {
            GetWindow<MissionWindow>("Mission Editor");
        }

        private readonly HorizontalPaneState paneConfiguration = new HorizontalPaneState() {
            initialLeftPaneWidth = 120,
            minPaneWidthLeft = 65,
            minPaneWidthRight = 100
        };

        private void OnEnable() {
            EditorApplication.hierarchyWindowChanged += CollectEntities;
            CollectEntities();
            gameDataFile = Resources.Load<GameDataFile>("Game Data");
            gameDataFile.GetShipDefinitions();
            shipDefinition = new ShipDefinition();
            skin = EditorGUIUtility.Load("MissionWindowSkin.asset") as GUISkin;
        }

        private void OnDisable() {
            EditorApplication.hierarchyWindowChanged -= CollectEntities;
        }

        private void CollectEntities() {
            entitites = Resources.FindObjectsOfTypeAll<Entity>().ToList().FindAll((entity) => {
                return !entity.gameObject.IsPrefab();
            });

            entityLabels = new List<ClickableLabel<Entity>>();
            for (int i = 0; i < entitites.Count; i++) {
                entityLabels.Add(new ClickableLabel<Entity>(entitites[i].name, entitites[i], OnSelected));
            }
        }

        public void Vertical(Action action) {
            EditorGUILayout.BeginVertical();
            action();
            EditorGUILayout.EndVertical();
        }

        public void Horizontal(Action action) {
            EditorGUILayout.BeginHorizontal();
            action();
            EditorGUILayout.EndHorizontal();
        }

        private void OnInspectorUpdate() {
            Repaint();
        }

        public void OnGUI() {
            GUI.skin = skin;
            GUILayout.Toolbar(0, tabs);
            EditorGUILayoutHorizontalPanes.Begin(paneConfiguration);
            Vertical(() => {
                Repeat(entityLabels, (item) => {
                    item.OnGUILayout(selectedEntity == item.data);
                });
            });
            EditorGUILayoutHorizontalPanes.Splitter();
            RenderDetails();
            EditorGUILayoutHorizontalPanes.End();
        }

        private void OnSelected(Entity entity) {
            selectedEntity = entity;
        }

        private void RenderDetails() {
            Vertical(() => {

                if (GUILayout.Button("Save")) {
                    gameDataFile.CreateOrReplaceShipDefinition(shipDefinition.name, shipDefinition);
                }

            });
        }

        enum MissionWindowPage {

            Enity,
            Ship

        }

        private void Repeat<T>(List<T> list, Action<T> render) {
            list.ForEach(render);
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