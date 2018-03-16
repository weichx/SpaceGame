//using System;
//using System.Collections.Generic;
//using System.Linq;
//using SpaceGame.Editor.GUIComponents;
//using SpaceGame.Editor.Reflection;
//using SpaceGame.Util;
//using UnityEditor;
//using UnityEngine;
//
//namespace SpaceGame.Editor.MissionWindow {
//
//    public class EntityPage : MissionWindowPage {
//
//        private const string NAME = nameof(MissionDefinition.name);
//        private const string GUID = nameof(MissionDefinition.guid);
//        private const string ENTITIES = nameof(MissionDefinition.entityDefinitions);
//
//        private Vector2 scrollPosition;
//        [SerializeField] private HorizontalPaneState splitterState;
//        private ReflectedProperty selectedEntityDefintion;
//        private List<ReflectedProperty> entityDefinitions;
//
//        public EntityPage(MissionWindowState state) : base(state) {
//            entityDefinitions = new List<ReflectedProperty>();
//            splitterState = new HorizontalPaneState();
//            splitterState.initialLeftPaneWidth = 100;
//            splitterState.minPaneWidthLeft = 25;
//            splitterState.minPaneWidthRight = 100;
//        }
//
//        public override void OnGUI() {
//            InfamyGUI.HorizontalSplitPane(splitterState, RenderEntityList, RenderEntityDetails);
//        }
//
//        private void RenderEntityList() {
//            state.currentMission[ENTITIES].GetChildren(entityDefinitions);
//            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
//            {
//
//                EditorGUILayout.BeginVertical();
//                {
//                    for (int i = 0; i < entityDefinitions.Count; i++) {
//                        ReflectedProperty entDef = entityDefinitions[i];
//                        bool isSelected = selectedEntityDefintion == entDef;
//                        InfamyGUI.SelectableLabel(entDef[NAME].stringValue, isSelected, entDef, OnEntitySelected);
//                    }
//                }
//                EditorGUILayout.EndVertical();
//            }
//            GUILayout.EndScrollView();
//        }
//
//        private void RenderEntityDetails() {
//            EditorGUILayout.BeginVertical();
//            {
//
//                if (selectedEntityDefintion != null) {
//
//                    EditorGUILayoutX.DrawProperties(selectedEntityDefintion);
//
//                    EditorGUILayout.BeginHorizontal();
//                    {
//
//                        InfamyGUI.Button("Delete", selectedEntityDefintion, DeleteEntity);
//
//                        GUILayout.FlexibleSpace();
//
//                        InfamyGUI.Button("Save", selectedEntityDefintion, SaveEntity);
//
//                    }
//                    EditorGUILayout.EndHorizontal();
//                }
//
//                GUILayout.FlexibleSpace();
//                EditorGUILayout.BeginHorizontal();
//                {
//                    GUILayout.FlexibleSpace();
//
//                    InfamyGUI.Button("Create Entity", CreateEntityDefinition);
//                    if (GUILayout.Button("Create Entity")) {
////                        state.CreateEntityDefinition();
//                    }
//                    EditorGUILayout.EndHorizontal();
//                }
//
//            }
//            EditorGUILayout.EndVertical();
//        }
//
//        private void CreateEntityDefinition() { }
//
//        private void SaveEntity(ReflectedProperty entityDefinition) { }
//
//        private void DeleteEntity(ReflectedProperty entityDefinition) {
////            state.currentMission.DeleteEntityDefinition(obj);
//            Debug.Log("Deleting");
//        }
//
//        private void OnEntitySelected(ReflectedProperty entityDefinition) {
//            selectedEntityDefintion = entityDefinition;
//        }
//
//        private void Reload() {
//            List<Entity> sceneEntities = Resources.FindObjectsOfTypeAll<Entity>().ToList().FindAll((entity) => {
//                return !entity.gameObject.IsPrefab();
//            });
//            ResolveEntityDefinitions(sceneEntities);
//        }
//
//        public override void OnEnable() {
//            EditorApplication.hierarchyWindowChanged += Reload;
//            Reload();
//        }
//
//        public override void OnDisable() {
//            EditorApplication.hierarchyWindowChanged -= Reload;
//        }
//
//        public void ResolveEntityDefinitions(List<Entity> sceneEntities) {
//
//            if (sceneEntities == null) return;
//
//            state.currentMission[ENTITIES].GetChildren(entityDefinitions);
//
//            List<EntityDefinition> missingDefinitions = new List<EntityDefinition>(32);
//
//            // this catches duplicate guids from prefab instantiation
//            IEnumerable<IGrouping<string, Entity>> duplicates = sceneEntities
//                .GroupBy(x => x.guid)
//                .Where(g => g.Count() > 1);
//
//            foreach (IGrouping<string, Entity> group in duplicates) {
//                List<Entity> dupGuidEntities = group.ToList();
//                for (int i = 1; i < dupGuidEntities.Count; i++) {
//                    Entity entity = dupGuidEntities[i];
//                    entity.guid = Guid.NewGuid().ToString();
//                    state.AddEntityDefinition(new EntityDefinition(entity));
//                }
//            }
//
//            for (int i = 0; i < sceneEntities.Count; i++) {
//                if (!entityDefinitions.Contains(sceneEntities[i].guid, (entDef, guid) => {
//                    return entDef[GUID].stringValue == guid;
//                })) {
//                    //no definition found for this entity, create one
//                    missingDefinitions.Add(new EntityDefinition(sceneEntities[i]));
//                }
//            }
//
//            for (int i = 0; i < entityDefinitions.Count; i++) {
//                string guid = entityDefinitions[i][GUID].stringValue;
//                if (!sceneEntities.Contains(guid, (entity, s) => entity.guid == s)) {
//                    //no scene entity found for this guid, create it
//                    entityDefinitions[i][NAME].Value += "[Missing!]";
//                }
//            }
//
//            state.currentMission[ENTITIES].AddArrayElements(missingDefinitions);
//
//        }
//
//    }
//
//}