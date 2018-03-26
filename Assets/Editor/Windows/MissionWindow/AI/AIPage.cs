using SpaceGame.AI;
using SpaceGame.Assets;
using SpaceGame.EditorComponents;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public class AIPage : MissionWindowPage {

        private AITreeView treeView;
        private Vector2 scrollVec;
        private GameAsset selectedAsset;
        private ReflectedObject reflectedSelection;

        public AIPage(MissionWindowState state, GameDatabase db) : base(state, db) { }

        public override void OnEnable() {
            treeView = new AITreeView(db, state.aiPageTreeViewState);
            treeView.createBehaviorSet += OnCreateBehaviorSet;
            treeView.createBehaviorDefintion += OnCreateBehaviorDefinition;
            treeView.createActionDefintion += OnCreateActionDefinition;
            treeView.selectionChanged += OnSelectionChanged;
            treeView.SetDataRebuildAndSelect(db.behaviorSets);
        }

        private void OnCreateActionDefinition(BehaviorDefinition behaviorDefinition) {
            ActionDefinition actionDefinition = db.CreateAsset<ActionDefinition>();
            behaviorDefinition.AddActionDefinition(actionDefinition);
            treeView.SetDataRebuildAndSelect(db.behaviorSets, actionDefinition.id);
        }

        private void OnCreateBehaviorSet() {
            BehaviorSet behaviorSet = db.CreateAsset<BehaviorSet>();
            treeView.SetDataRebuildAndSelect(db.behaviorSets, behaviorSet.id);
        }

        private void OnCreateBehaviorDefinition(BehaviorSet behaviorset) {
            BehaviorDefinition behaviorDefinition = db.CreateAsset<BehaviorDefinition>();
            behaviorset.AddBehaviorDefinition(behaviorDefinition);
            treeView.SetDataRebuildAndSelect(db.behaviorSets, behaviorDefinition.id);
        }

        private void OnSelectionChanged(GameAsset selection) {
            selectedAsset = selection;
            if (selection != null) {
                reflectedSelection = new ReflectedObject(selectedAsset);
            }
            else {
                reflectedSelection = null;
            }
        }

        public override void OnDisable() { }

        public override void OnGUI() {
            InfamyGUI.HorizontalSplitPane(state.aiPageSplitterState, RenderList, RenderDetails);
            if (treeView != null && selectedAsset != null) {
                treeView.UpdateDisplayName(selectedAsset);
            }
        }

        private void RenderList() {
            EditorGUILayoutX.BeginVertical();
            treeView.OnGUILayout();
            EditorGUILayoutX.EndVertical();
        }

        private void RenderDetails() {
            if (selectedAsset == null) return;
            scrollVec = EditorGUILayout.BeginScrollView(scrollVec);
            {

                EditorGUILayout.BeginVertical((GUILayoutOption[]) null);
                EditorGUILayoutX.PropertyField(reflectedSelection);
                EditorGUILayout.EndHorizontal();

            }
            EditorGUILayout.EndScrollView();
            reflectedSelection?.ApplyModifiedProperties();

        }

    }

}