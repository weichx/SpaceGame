using SpaceGame.Editor.GUIComponents;
using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor.MissionWindow {

    public class OverviewPage : MissionWindowPage {

        private HorizontalPaneState splitterState;

        public OverviewPage(MissionWindowState state) : base(state) {
            splitterState = new HorizontalPaneState();
            splitterState.initialLeftPaneWidth = 100;
            splitterState.minPaneWidthLeft = 25;
            splitterState.minPaneWidthRight = 100;
        }

        private bool HasValidMission => state.currentMission != null;

        public override void OnEnable() {
               
        }
        
        public override void OnGUI() {
            InfamyGUI.HorizontalSplitPane(splitterState, RenderMissionList, RenderMissionDetails);
        }

        private void RenderMissionList() {
            EditorGUILayout.BeginVertical();
            {
//                for (int i = 0; i < state.missions.Count; i++) {
//                    MissionDefinition mission = state.missions[i];
//                    bool isSelected = mission == state.currentMission;
//                    ReflectedObject reflected = state.reflectedMissions[i];
//                    ReflectedProperty nameProp = reflected.FindChild(nameof(mission.name));
//                    string name = (string)nameProp.Value;
//                    if (reflected.HasModifiedProperties) name += "*";
//                    InfamyGUI.SelectableLabel(name, isSelected, mission, OnMissionSelected);
//                }
            }
            EditorGUILayout.EndVertical();
        }

        private void RenderMissionDetails() {
            EditorGUILayout.BeginVertical();
            {

                if (HasValidMission) {
//                    ReflectedObject reflected = state.currentMissionReflected;
//                    if (reflected == null) {
//                        Debug.Log("Ref null");
//                        return;
//                    }
//                    ReflectedProperty nameProp = reflected.FindChild("name");
//                    if (nameProp == null) {
//                        Debug.Log("Null");
//                        return;
//                    }
//                    nameProp.Value = EditorGUILayout.TextField(InfamyGUI.GetLabel("Mission Name"), (string)nameProp.Value);
//                    EditorGUILayout.LabelField("Created At", mission.createdAt);
                }
                
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("New Mission")) {
                        state.AddMission();
                    }
                    if (GUILayout.Button("Save")) {
                        state.SaveMission();
                        state.Save();
                    }
                    EditorGUILayout.EndHorizontal();
                }

            }
            EditorGUILayout.EndVertical();
        }

        private void OnMissionSelected(MissionDefinition selectedMission) {
//            state.currentMission = selectedMission;
        }

    }

}