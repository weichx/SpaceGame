using System.Collections.Generic;
using System.Linq;
using SpaceGame.Editor.GUIComponents;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Weichx.EditorReflection;
using Weichx.Util;

namespace SpaceGame.Editor.MissionWindow {

    public class MissionPage : MissionWindowPage {

        private const string NAME = nameof(MissionDefinition.name);
        private const string GUID = nameof(MissionDefinition.guid);
        private const string ENTITIES = nameof(MissionDefinition.entityDefinitions);

        [SerializeField] private HorizontalPaneState splitterState;
        private ReflectedProperty selectedEntity;
//        private ReflectedListProperty entityDefinitions;
        private Vector2 scrollPosition;
        private EntityTreeView treeView;
        private TreeViewState treeState;
        private ReflectedListProperty entities;
        
        public MissionPage(MissionWindowState state) : base(state) {
            splitterState = new HorizontalPaneState();
            splitterState.initialLeftPaneWidth = 100;
            splitterState.minPaneWidthLeft = 25;
            splitterState.minPaneWidthRight = 100;
        }

        public override void OnGUI() {
            InfamyGUI.HorizontalSplitPane(splitterState, RenderEntityList, RenderEntityDetails);
        }

        private void RenderEntityList() {
            treeView.OnGUI(GUILayoutUtility.GetRect(0, 10000, 0, 10000));
        }

        private void RenderEntityDetails() {
            EditorGUILayout.BeginVertical();
            {

                if (selectedEntity != null) {

                    EditorGUILayoutX.DrawProperties(selectedEntity);

                    EditorGUILayout.BeginHorizontal();
                    {

                        InfamyGUI.Button("Delete", selectedEntity, DeleteEntity);

                        GUILayout.FlexibleSpace();

                        InfamyGUI.Button("Save", selectedEntity, SaveEntity);

                    }
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    InfamyGUI.Button("Create Entity", CreateEntityDefinition);

                    EditorGUILayout.EndHorizontal();
                }

            }
            EditorGUILayout.EndVertical();
        }
        
        private void Reload() {
            List<Entity> sceneEntities = Resources.FindObjectsOfTypeAll<Entity>().ToList().FindAll((entity) => {
                return !entity.gameObject.IsPrefab();
            });
            entities = new ReflectedListProperty(null, "--Root--", sceneEntities.GetType(), sceneEntities);
            
        }

        public override void OnEnable() {
            EditorApplication.hierarchyWindowChanged += Reload;
            Reload();
            treeState = new TreeViewState();
            treeView = new EntityTreeView(entities, treeState, OnEntitySelected);
        }

        public override void OnDisable() {
            EditorApplication.hierarchyWindowChanged -= Reload;
        }

        private void CreateEntityDefinition() {
//            entityDefinitions.AddElement(new EntityDefinition());
            treeView.Reload();
        }

        private void SaveEntity(ReflectedProperty entityDefinition) {
            state.SaveMission();
            state.Save();
        }

        private void DeleteEntity(ReflectedProperty entityDefinition) {
//            state.currentMission.DeleteEntityDefinition(obj);
            Debug.Log("Deleting");
        }

        private void OnEntitySelected(ReflectedProperty entityDefinition) {
            selectedEntity = entityDefinition;
        }

        class EntityPointer {

            public int entityInstanceId;
            
        }
        
        public void ResolveEntityDefinitions(List<Entity> sceneEntities) {

            
//            for (int i = 0; i < sceneEntities.Count) {
//                if()
//            }
//            List<
            
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
//
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
//            ReflectedListProperty entityDefs = state.currentMission[ENTITIES] as ReflectedListProperty;
//            System.Diagnostics.Debug.Assert(entityDefs != null, nameof(entityDefs) + " != null");
//            entityDefs.AddArrayElements(missingDefinitions);

        }

    }

}