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
            treeView.selectionChanged += OnSelectionChanged;
            treeView.SetDataRebuildAndSelect();
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