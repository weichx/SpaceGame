using System;
using System.Collections.Generic;
using System.Linq;
using SpaceGame.Editor.GUIComponents;
using SpaceGame.Editor.Reflection;
using SpaceGame.Util;
using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor.MissionWindow {

    public class EntityPage : MissionWindowPage {

        private HorizontalPaneState splitterState;
        private string selectedGUID;
        private EntityDefinition selectedEntityDefintion;
        private ReflectedObject reflectedEntDef;
        
        public EntityPage(MissionWindowState state) : base(state) {
            splitterState = new HorizontalPaneState();
            splitterState.initialLeftPaneWidth = 100;
            splitterState.minPaneWidthLeft = 25;
            splitterState.minPaneWidthRight = 100;
            selectedGUID = string.Empty;
        }

        public override void OnGUI() {
            InfamyGUI.HorizontalSplitPane(splitterState, RenderEntityList, RenderEntityDetails);
        }

        private void RenderEntityList() {
            List<EntityDefinition> entityDefinitions = state.GetEntityDefinitions();
            EditorGUILayout.BeginVertical();
            {
                for (int i = 0; i < entityDefinitions.Count; i++) {
                    EntityDefinition entityDefinition = entityDefinitions[i];
                    bool isSelected = selectedEntityDefintion == entityDefinition;
                    InfamyGUI.SelectableLabel(entityDefinition.name, isSelected, entityDefinition, OnEntitySelected);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void RenderEntityDetails() {
            EditorGUILayout.BeginVertical();
            {

                if (selectedEntityDefintion != null) {
                    EditorGUILayoutX.DrawProperties(reflectedEntDef);
                    if (GUILayout.Button("Save")) {
                        reflectedEntDef.ApplyModifiedProperties();
                        // apply to corresponding scene entity
                        state.SaveEntityDefinitions();
                    }
                }
                
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Create Entity")) {
                        state.CreateEntityDefinition();
                    }
                    EditorGUILayout.EndHorizontal();
                }

            }
            EditorGUILayout.EndVertical();
        }

        private void OnEntitySelected(EntityDefinition entityDefinition) {
            Debug.Assert(entityDefinition.guid.Length > 0);
            selectedEntityDefintion = entityDefinition;
            reflectedEntDef = new ReflectedObject(selectedEntityDefintion);
        }

        private void Reload() {
            List<Entity> sceneEntities = Resources.FindObjectsOfTypeAll<Entity>().ToList().FindAll((entity) => {
                return !entity.gameObject.IsPrefab();
            });
            ResolveEntityDefinitions(sceneEntities);
        }

        public override void OnEnable() {
            EditorApplication.hierarchyWindowChanged += Reload;
            Reload();
        }

        public override void OnDisable() {
            EditorApplication.hierarchyWindowChanged -= Reload;
        }

        public void ResolveEntityDefinitions(List<Entity> sceneEntities) {

            if (sceneEntities == null) return;
            
            List<EntityDefinition> entityDefinitions = state.GetEntityDefinitions();
            if (entityDefinitions == null) return;
            
            List<EntityDefinition> missingDefinitions = new List<EntityDefinition>(32);
            List<Entity> missingEntities = new List<Entity>(32);

            // this catches duplicate guids from prefab instantiation
            IEnumerable<IGrouping<string, Entity>> duplicates = sceneEntities
                .GroupBy(x => x.guid)
                .Where(g => g.Count() > 1);
            
            foreach (IGrouping<string, Entity> group in duplicates) {
                List<Entity> dupGuidEntities = group.ToList();
                for (int i = 1; i < dupGuidEntities.Count; i++) {
                    Entity entity = dupGuidEntities[i];
                    entity.guid = Guid.NewGuid().ToString();
                    EntityDefinition entityDefinition = new EntityDefinition(dupGuidEntities[0]);
                    entityDefinition.guid = entity.guid;
                    entityDefinitions.Add(entityDefinition);
                }
            }
            
            for (int i = 0; i < sceneEntities.Count; i++) {
                if (entityDefinitions.FindByIndex(sceneEntities[i].guid, (entDef, guid) => entDef.guid == guid) == -1) {
                    //no definition found for this entity, create one
                    missingDefinitions.Add(new EntityDefinition(sceneEntities[i]));
                }
            }

            for (int i = 0; i < entityDefinitions.Count; i++) {
                if (sceneEntities.FindByIndex(entityDefinitions[i].guid, (entity, s) => entity.guid == s) == -1) {
                    //no scene entity found for this guid, create it
//                    missingEntities.Add();
                }
            }

            state.SetEntityDefinitions(entityDefinitions.Concat(missingDefinitions).ToList());
        }

    }

}