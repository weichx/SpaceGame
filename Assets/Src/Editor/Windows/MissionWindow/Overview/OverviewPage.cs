using System;
using SpaceGame.Editor.GUIComponents;
using SpaceGame.FileTypes;
using Src.Editor;
using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor.MissionWindow {

    public class OverviewPage : MissionWindowPage {

        private MissionDefinition[] missions;
        private HorizontalPaneState splitterState;

        public OverviewPage(MissionWindowState state) : base(state) {
            splitterState = new HorizontalPaneState();
            splitterState.initialLeftPaneWidth = 100;
            splitterState.minPaneWidthLeft = 25;
            splitterState.minPaneWidthRight = 100;
            Reload();
        }

        private bool HasValidMission => state.CurrentMission != null;

        public override void OnGUI() {
            InfamyGUI.HorizontalSplitPane(splitterState, RenderMissionList, RenderMissionDetails);
        }

        private void Reload() {
            missions = Resources.LoadAll<MissionDefinition>("Missions");
        }

        private void RenderMissionList() {
            EditorGUILayout.BeginVertical();
            {
                for (int i = 0; i < missions.Length; i++) {
                    MissionDefinition mission = missions[i];
                    bool isSelected = mission == state.CurrentMission;
                    InfamyGUI.SelectableLabel(mission.name, isSelected, i, OnMissionSelected);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void RenderMissionDetails() {
            EditorGUILayout.BeginVertical();
            {

                if (HasValidMission) {
                    MissionDefinition mission = state.CurrentMission;
                    string missionName = mission.name;
                    mission.name = EditorGUILayout.DelayedTextField(InfamyGUI.GetLabel("Mission Name"), missionName);
                    if (mission.name != missionName) {
                        string assetPath = AssetDatabase.GetAssetPath(mission.GetInstanceID());
                        AssetDatabase.RenameAsset(assetPath, mission.name);
                    }
                    
                    EditorGUILayout.LabelField("Created At", mission.createdAt);
                }
                
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("New Mission")) {
                        MissionDefinition.Create("New Mission");
                        Reload();
                    }
                    EditorGUILayout.EndHorizontal();
                }

            }
            EditorGUILayout.EndVertical();
        }

        private void OnMissionSelected(int index) {
            state.CurrentMission = missions[index];
        }

    }

}