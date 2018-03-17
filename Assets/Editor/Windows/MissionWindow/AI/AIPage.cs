using System.Collections.Generic;
using SpaceGame.AI;
using SpaceGame.Editor.GUIComponents;
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
        private List<Decision> decisions;
        private IList<int> selectedIds;

        public AIPage(MissionWindowState state) : base(state) {
            this.selectedIds = new List<int>(4);
            decisions = new List<Decision>();
            decisions.Add(new Decision());
            decisions.Add(new Decision());
            decisions.Add(new Decision());
            decisions.Add(new Decision());
            decisions.Add(new Decision());
            splitterState = new HorizontalPaneState();
            splitterState.initialLeftPaneWidth = 150;
            splitterState.minPaneWidthLeft = 25;
            splitterState.minPaneWidthRight = 100;
        }

        public override void OnGUI() {
            InfamyGUI.HorizontalSplitPane(splitterState, RenderList, RenderDetails);

        }

        private void RenderList() {
            treeView.OnGUI(GUILayoutUtility.GetRect(0, 10000, 0, 10000));
        }

        private void RenderDetails() {
            if (selectedDecision == null) return;
            EditorGUILayoutX.PropertyField(selectedDecision);
        }

        private bool FindSelectedDecision(ReflectedProperty property) {
            return selectedIds.Contains(property.GetValue<Decision>().id);
        }

        public override void OnEnable() {
            decisionList = new ReflectedObject(decisions).Root as ReflectedListProperty;
            treeView = new AITreeView(decisionList, OnSelectionChanged);
            treeView.SetSelection(selectedIds);
        }

        private void OnSelectionChanged(IList<int> selectedIds) {
            this.selectedIds = new List<int>(selectedIds);
            selectedDecision = decisionList.Find(FindSelectedDecision);
        }

        public override void OnDisable() { }

    }

}