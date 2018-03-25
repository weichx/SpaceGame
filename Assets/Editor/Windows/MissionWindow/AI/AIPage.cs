//using System.Collections.Generic;
//using SpaceGame.AI;
//using SpaceGame.EditorComponents;
//using SpaceGame.FileTypes;
//using UnityEditor;
//using UnityEditor.IMGUI.Controls;
//using UnityEngine;
//using Weichx.EditorReflection;
//
//namespace SpaceGame.Editor.MissionWindow {
//
//    public class AIPage : MissionWindowPage {
//
//        [SerializeField] private HorizontalPaneState splitterState;
//
//        private TreeViewState treeState;
//        private ReflectedProperty selectedDecision;
//        private const string NameField = nameof(Decision.name);
//
//        public AIPage(MissionWindowState state, GameDataFile gameData) : base(state, gameData) {
//            this.selectedIds = new List<int>(4);
//            splitterState = new HorizontalPaneState();
//            splitterState.initialLeftPaneWidth = 150;
//        }
//
//        public override void OnGUI() {
//            InfamyGUI.HorizontalSplitPane(splitterState, RenderList, RenderDetails);
//        }
//
//        private void CreateDecision() {
//            Decision decision = new Decision();
//            selectedIds.Clear();
//            selectedIds.Add(decision.Id);
//            list.AddElement(decision);
//          //  treeView.Reload();
//          //  treeView.SetSelection(selectedIds, TreeViewSelectionOptions.FireSelectionChanged | TreeViewSelectionOptions.RevealAndFrame);
//        }
//
//        private Vector2 scrollVec;
//        private void RenderList() {
//            EditorGUILayout.BeginVertical((GUILayoutOption[]) null);
//            InfamyGUI.Button("Create Decision", CreateDecision);
//           // this.treeView.OnGUI(GUILayoutUtility.GetRect(0, 10000, 0, 10000));
//            EditorGUILayout.EndVertical();
//        }
//
//        private void RenderDetails() {
//            if (selectedDecision == null) return;
//            scrollVec = EditorGUILayout.BeginScrollView(scrollVec);
//
////            treeView.UpdateDisplayName(
////                selectedDecision.GetValue<Decision>().Id,
////                selectedDecision[NameField].stringValue
////            );
//
//            EditorGUILayout.BeginVertical((GUILayoutOption[]) null);
//            EditorGUILayoutX.PropertyField(selectedDecision);
//            GUILayout.FlexibleSpace();
//            InfamyGUI.Button("Delete Decision", DeleteDecision);
//            EditorGUILayout.EndHorizontal();
//            EditorGUILayout.EndScrollView();
//
//        }
//
//        private void DeleteDecision() {
//            if (selectedDecision == null) return;
//            string name = selectedDecision[NameField].stringValue;
//            if (EditorUtility.DisplayDialog("Are you sure?", $"Really delete {name}?", "Yup", "Nope")) {
//                list.RemoveElement(selectedDecision);
//                selectedDecision = null;
//               // treeView.SetSelection(new List<int>());
//               // treeView.Reload();
//            }
//        }
//
//        private bool FindSelectedDecision(ReflectedProperty property) {
//            return selectedIds.Contains(property.GetValue<Decision>().Id);
//        }
//
//        public override void OnEnable() {
//            gameData = Resources.Load<GameDataFile>("Game Data");
//            list = new ReflectedObject(gameData.GetDecisions()).Root as ReflectedListProperty;
//           // treeView = new AITreeView(list, OnSelectionChanged);
//        }
//
//        private void OnSelectionChanged(IList<int> selectedIds) {
//            this.selectedIds = new List<int>(selectedIds);
//            selectedDecision = list.Find(FindSelectedDecision);
//        }
//
//        public override void OnDisable() {
//            list.ApplyChanges();
//        }
//
//    }
//
//}