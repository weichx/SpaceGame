using System;
using System.Collections.Generic;
using SpaceGame.EditorComponents;
using SpaceGame.FileTypes;
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

        [SerializeField] private HorizontalPaneState splitterState;
        private TreeViewState treeState;
        private new MissionTreeView treeView;
        private MissionDefinition mission;
        private ReflectedObject reflectedMission;
        private ReflectedObject reflectedSelection;
        private SelectionType selectionType;
        private AssetDefinition selectedAsset;

        public MissionPage(MissionWindowState state, GameDataFile gameData) : base(state, gameData) {
            this.selectionType = SelectionType.None;
        }

        private bool IsMultiSelect => (selectionType & SelectionType.Multiple) != 0;

        public override void OnEnable() {
            mission = gameData.GetMission(state.activeMissionGuid);
            reflectedMission = new ReflectedObject(mission);
            treeView = new MissionTreeView(mission, state.missionPageTreeViewState, OnSelectionChanged);
        }

        public override void OnDisable() {
            reflectedSelection?.ApplyModifiedProperties();
            reflectedMission.Update();
            gameData.Save();
        }

        public override void OnGUI() {
            InfamyGUI.HorizontalSplitPane(splitterState, RenderList, RenderDetails);
            if (treeView != null && selectedAsset != null) {
                treeView.UpdateDisplayName(selectedAsset);
            }
        }

        private void RenderList() {
            EditorGUILayoutX.BeginVertical();
            reflectedSelection?.ApplyModifiedProperties();
            treeView.OnGUILayout();
            reflectedSelection?.Update();
            GUILayout.FlexibleSpace();
            InfamyGUI.Button("Create Faction", OnCreateFaction);
            EditorGUILayoutX.EndVertical();
        }

        private void OnCreateFaction() {
            mission.AddFaction();
            treeView.Reload();
        }

        private void RenderDetails() {
            if (IsMultiSelect) {
                RenderMultiSelect();
            }
            else {
                switch (selectionType) {
                    case SelectionType.None:
                        EditorGUILayout.LabelField("Nothing selected");
                        return;
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
                EditorGUILayoutX.DrawProperties(reflectedSelection);
            }
            EditorGUILayoutX.EndVertical();
        }

        private void RenderFlightGroupInspector() {
            EditorGUILayoutX.BeginVertical();
            if (reflectedSelection != null) {
                EditorGUILayoutX.DrawProperties(reflectedSelection);
            }
            EditorGUILayoutX.EndVertical();
        }
        
        private void RenderEntityInspector() {
            EditorGUILayoutX.BeginVertical();
            if (reflectedSelection != null) {
                EditorGUILayoutX.DrawProperties(reflectedSelection);
            }
            EditorGUILayoutX.EndVertical();
        }

        private void OnSelectionChanged(MissionTreeSelection treeSelection) {
            //todo currently ignoring multiselect
            reflectedSelection?.ApplyModifiedProperties();
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

                case MissionTreeView.ItemType.None:
                    return SelectionType.None;
                default:
                    throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
            }
        }

    }

}