using System;
using System.Collections.Generic;
using SpaceGame.EditorComponents;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public class MissionPage : MissionWindowPage {

        [Flags]
        private enum SelectionType {

            None = 0,
            Faction = 1 << 0,
            FlightGroup = 1 << 1,
            Entity = 1 << 2,
            Multiple = 1 << 8

        }

        private TreeViewState treeState;
        private MissionTreeView treeView;
        private MissionDefinition mission;
        private ReflectedObject reflectedSelection;
        private SelectionType selectionType;
        private AssetDefinition selectedAsset;
        private Action saveCallback;
        private Vector2 scroll = Vector2.zero;

        public MissionPage(Action saveCallback, MissionWindowState state, GameDatabase db) : base(state, db) {
            this.saveCallback = saveCallback;
            this.selectionType = SelectionType.None;
        }

        private bool IsMultiSelect => (selectionType & SelectionType.Multiple) != 0;

        public override void OnEnable() {
            mission = db.GetCurrentMission();
            treeView = new MissionTreeView(state.missionPageTreeViewState);
            treeView.SetDataAndRebuild(mission);
            treeView.createEntity += mission.CreateEntity;
            treeView.createFlightGroup += mission.CreateFlightGroup;
            treeView.createFaction += mission.CreateFaction;
            treeView.setEntityFlightGroup += mission.SetEntityFlightGroup;
            treeView.setFactionIndex += mission.SetFactionIndex;
            treeView.setEntityFaction += mission.SetEntityFaction;
            treeView.setFlightGroupFaction += mission.SetFlightGroupFaction;
            treeView.deleteAsset += mission.DeleteAsset;
            treeView.selectionChanged += OnSelectionChanged;
            
            mission.onChange += (changedId) => {
                reflectedSelection?.Update();
                treeView.SetDataAndRebuild(mission);
                treeView.SetSingleSelection(changedId);
            };
            treeView.PingSelection();
        }

        public override void OnDisable() {
            reflectedSelection?.ApplyModifiedProperties();
        }

        public override void OnGUI() {
            InfamyGUI.HorizontalSplitPane(state.missionPageSplitterState, RenderList, RenderDetails);
            if (treeView != null && selectedAsset != null) {
                treeView.UpdateDisplayName(selectedAsset);
            }
        }

        private void RenderList() {
            EditorGUILayoutX.BeginVertical();
            treeView.OnGUILayout();
            InfamyGUI.Button("Save", saveCallback);
            EditorGUILayoutX.EndVertical();
        }
       
        private void RenderDetails() {
            scroll = GUILayout.BeginScrollView(scroll);

            if (IsMultiSelect) {
                RenderMultiSelect();
            }
            else {
                switch (selectionType) {
                    case SelectionType.None:
                        EditorGUILayout.LabelField("Nothing selected");
                        break;
                    case SelectionType.Faction:
                        RenderFactionInspector();
                        break;
                    case SelectionType.FlightGroup:
                        RenderFlightGroupInspector();
                        break;
                    case SelectionType.Entity:
                        RenderEntityInspector();
                        break;
                    case SelectionType.Multiple:
                        throw new ArgumentOutOfRangeException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            GUILayout.EndScrollView();
            reflectedSelection?.ApplyModifiedProperties();
        }

        private void RenderMultiSelect() {
            switch (selectionType & ~(SelectionType.Multiple)) {
                case SelectionType.None:
                    EditorGUILayout.LabelField("Nothing selected");
                    return;
                case SelectionType.Faction:
                    EditorGUILayout.LabelField("Multiple Factions Selected");
                    break;
                case SelectionType.FlightGroup:
                    EditorGUILayout.LabelField("Multiple FlightGroups Selected");
                    break;
                case SelectionType.Entity:
                    EditorGUILayout.LabelField("Multiple Entities Selected");
                    break;
                case SelectionType.Multiple:
                    throw new ArgumentOutOfRangeException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RenderFactionInspector() {
            EditorGUILayoutX.BeginVertical();
            if (reflectedSelection != null) {
                EditorGUILayoutX.PropertyField(reflectedSelection);
            }
            EditorGUILayoutX.EndVertical();
        }

        private void RenderFlightGroupInspector() {
            EditorGUILayoutX.BeginVertical();
            if (reflectedSelection != null) {
                EditorGUILayoutX.PropertyField(reflectedSelection);
            }
            EditorGUILayoutX.EndVertical();
        }

        private void RenderEntityInspector() {
            EditorGUILayoutX.BeginVertical();
            if (reflectedSelection != null) {
                EditorGUILayoutX.PropertyField(reflectedSelection);
            }
            EditorGUILayoutX.EndVertical();
        }

        private void OnSelectionChanged(MissionTreeSelection treeSelection) {
            //todo currently ignoring multiselect
            selectionType = TranslateSelectionType(treeSelection);
            selectedAsset = treeSelection.properties?[0];
            reflectedSelection = new ReflectedObject(selectedAsset);
        }

        private static SelectionType TranslateSelectionType(MissionTreeSelection treeSelection) {
            List<AssetDefinition> items = treeSelection.properties;
            MissionTreeView.ItemType itemType = treeSelection.itemType;

            if (items == null || items.Count == 0) return SelectionType.None;

            SelectionType retn = items.Count > 1 ? SelectionType.Multiple : SelectionType.None;
            switch (itemType) {

                case MissionTreeView.ItemType.Faction:
                    return retn | SelectionType.Faction;

                case MissionTreeView.ItemType.FlightGroup:
                    return retn | SelectionType.FlightGroup;

                case MissionTreeView.ItemType.Entity:
                    return retn | SelectionType.Entity;

                case MissionTreeView.ItemType.Root:
                    return SelectionType.None;

                default:
                    throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
            }
        }

    }

}