using System.Collections.Generic;
using System.Runtime.Remoting;
using SpaceGame.AI;
using SpaceGame.Editor.GUIComponents;
using SpaceGame.FileTypes;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public class AIPage : MissionPage {

        [SerializeField] private HorizontalPaneState splitterState;

        private AITreeView treeView;
        private TreeViewState treeState;
        private ReflectedListProperty decisionList;
        private ReflectedProperty selectedDecision;
        private IList<int> selectedIds;
        private DecisionEvaluatorDataFile evaluatorData;
        private const string NameField = nameof(Decision.name);

        public AIPage(MissionWindowState state) : base(state) {
            this.selectedIds = new List<int>(4);
            splitterState = new HorizontalPaneState();
            splitterState.initialLeftPaneWidth = 150;
        }

        public override void OnGUI() {
            InfamyGUI.HorizontalSplitPane(splitterState, RenderList, RenderDetails);
        }

        private void CreateDecision() {
            decisionList.AddElement(new Decision());
            treeView.Reload();
        }

        private void RenderList() {
            EditorGUILayout.BeginVertical((GUILayoutOption[]) null);
            InfamyGUI.Button("Create Decision", CreateDecision);
            treeView.OnGUI(GUILayoutUtility.GetRect(0, 10000, 0, 10000));
            EditorGUILayout.EndVertical();
        }

        private void RenderDetails() {
            if (selectedDecision == null) return;

            treeView.UpdateDisplayName(
                selectedDecision.GetValue<Decision>().id,
                selectedDecision[NameField].stringValue
            );
            
            EditorGUILayout.BeginVertical((GUILayoutOption[]) null);
            EditorGUILayoutX.PropertyField(selectedDecision);
            GUILayout.FlexibleSpace();
            InfamyGUI.Button("Delete Decision", DeleteDecision);
            EditorGUILayout.EndHorizontal();

        }

        private void DeleteDecision() {
            if (selectedDecision == null) return;
            string name = selectedDecision[NameField].stringValue;
            if (EditorUtility.DisplayDialog("Are you sure?", $"Really delete {name}?", "Yup", "Nope")) {
                decisionList.RemoveElement(selectedDecision);
                selectedDecision = null;
                treeView.SetSelection(new List<int>());
                treeView.Reload();
            }
        }

        private bool FindSelectedDecision(ReflectedProperty property) {
            return selectedIds.Contains(property.GetValue<Decision>().id);
        }

        public override void OnEnable() {
            evaluatorData = Resources.Load<DecisionEvaluatorDataFile>("AI/Evaluators");
            decisionList = new ReflectedObject(evaluatorData.GetDecisions()).Root as ReflectedListProperty;
            treeView = new AITreeView(decisionList, OnSelectionChanged);
            treeView.SetSelection(selectedIds);
        }

        private void OnSelectionChanged(IList<int> selectedIds) {
            this.selectedIds = new List<int>(selectedIds);
            selectedDecision = decisionList.Find(FindSelectedDecision);
        }
        
        public override void OnDisable() {
            decisionList.ApplyChanges();
            evaluatorData.Save((List<Decision>) decisionList.Value);
        }

    }

}