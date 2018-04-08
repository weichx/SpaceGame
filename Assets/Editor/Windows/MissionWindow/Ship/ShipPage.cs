using SpaceGame.EditorComponents;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public class ShipPage : MissionWindowPage {

        protected ReflectedObject selection;

        private Vector2 scroll;
        private ShipTreeView treeView;

        private const string IdField = nameof(ShipType.id);
        private const string NameField = nameof(ShipType.name);

        public ShipPage(MissionWindowState state, GameDatabase db) : base(state, db) { }

        public override void OnEnable() {
            state.shipPageTreeViewState = state.shipPageTreeViewState ?? new TreeViewState();
            treeView = new ShipTreeView(state.shipPageTreeViewState);
            treeView.selectionChanged += OnSelectionChanged;
            treeView.SetDataRebuildAndSelect();
        }

        public override void OnDisable() {
            treeView.selectionChanged -= OnSelectionChanged;
        }

        public override void OnGUI() {
            InfamyGUI.HorizontalSplitPane(state.shipPageSplitterState, RenderList, RenderDetails);
        }

        private void RenderList() {
            EditorGUILayoutX.BeginVertical();
            treeView.OnGUILayout();
            EditorGUILayoutX.EndVertical();
        }

        private void RenderDetails() {
            if (selection == null) return;
            
            scroll = GUILayout.BeginScrollView(scroll);
            {

                treeView.UpdateDisplayName(
                    selection[IdField].intValue,
                    selection[NameField].stringValue
                );
                EditorGUILayout.BeginVertical((GUILayoutOption[]) null);
                {
                    EditorGUILayoutX.PropertyField(selection);

                    if (selection.HasModifiedProperties) {
                        selection.ApplyModifiedProperties();
                        treeView.SetDataRebuildAndSelect();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
        }
 
        private void OnSelectionChanged(ShipTreeView.ShipTreeViewSelection newSelection) {
            selection?.ApplyModifiedProperties();
            if (newSelection.shipGroup != null) {
                selection = new ReflectedObject(newSelection.shipGroup);
            }
            else if (newSelection.ship != null) {
                selection = new ReflectedObject(newSelection.ship);
            }
            else {
                selection = null;
            }
        }

    }

}