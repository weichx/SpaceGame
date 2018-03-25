using System.Collections.Generic;
using SpaceGame.EditorComponents;
using SpaceGame.FileTypes;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public class ShipPage : MissionWindowPage {

        protected ReflectedObject selection;

        private ShipTreeView treeView;

        private const string IdField = nameof(ShipDefinition.id);
        private const string NameField = nameof(ShipDefinition.name);

        public ShipPage(MissionWindowState state, GameDatabase db) : base(state, db) {}

        public override void OnGUI() {
            InfamyGUI.HorizontalSplitPane(state.shipPageSplitterState, RenderList, RenderDetails);
        }

        private void RenderList() {
            EditorGUILayoutX.BeginVertical();
            treeView.OnGUILayout();
            GUILayout.FlexibleSpace();
            InfamyGUI.Button("Create Ship", CreateShipDefintion);
            EditorGUILayoutX.EndVertical();
        }

        private void RenderDetails() {
            if (selection == null) return;
            treeView.UpdateDisplayName(
                selection[IdField].intValue,
                selection[NameField].stringValue
            );
            EditorGUILayout.BeginVertical((GUILayoutOption[]) null);
            EditorGUILayoutX.PropertyField(selection);
            if (selection.HasModifiedProperties) {
                selection.ApplyModifiedProperties();
                treeView.SetDataAndRebuild(db.shipDefinitions);
            }
            GUILayout.FlexibleSpace();
            InfamyGUI.Button("Delete Defintion", DeleteShipDefinition);
            EditorGUILayout.EndHorizontal();
        }

        private void CreateShipDefintion() {
            treeView.SetSingleSelection(db.CreateAsset<ShipDefinition>().id);
        }

        private void DeleteShipDefinition() {
            if (selection == null) return;
            string name = selection[NameField].stringValue;
            if (EditorUtility.DisplayDialog("Are you sure?", $"Really delete {name}?", "Yup", "Nope")) {
                db.DestroyAsset<ShipDefinition>(selection[IdField].intValue);
                treeView.ClearSelection();
                treeView.Reload();
            }
        }

        public override void OnEnable() {
            state.shipPageTreeViewState = state.shipPageTreeViewState ?? new TreeViewState();
            treeView = new ShipTreeView(state.shipPageTreeViewState);
            treeView.selectionChanged += OnSelectionChanged;
            treeView.hierarchyChanged += OnHierarchyChanged;
            treeView.SetDataAndRebuild(db.shipDefinitions);
        }

        public override void OnDisable() {
            treeView.selectionChanged -= OnSelectionChanged;
            treeView.hierarchyChanged -= OnHierarchyChanged;
        }

        private void OnHierarchyChanged() { }

        private void OnSelectionChanged(ShipDefinition newSelection) {
            selection?.ApplyModifiedProperties();
            if (newSelection != null) {
                selection = new ReflectedObject(newSelection);
            }
            else {
                selection = null;
            }
        }

    }

}