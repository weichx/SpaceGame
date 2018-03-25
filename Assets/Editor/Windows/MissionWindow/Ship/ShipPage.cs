using SpaceGame.Assets;
using SpaceGame.EditorComponents;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Weichx.EditorReflection;
using Weichx.Util;

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
            treeView.createShipType += OnCreateShipDefinition;
            treeView.createShipTypeGroup += OnCreateShipGroup;
            treeView.deleteShipType += OnDeleteAsset;
            treeView.deleteShipTypeGroup += OnDeleteAsset;
            treeView.setShipTypeShipGroup += OnSetShipTypeShipGroup;
            treeView.setShipTypeGroupIndex += OnSetShipTypeGroupIndex;
            treeView.SetDataRebuildAndSelect(db.shipTypeGroups);
        }

        private void OnSetShipTypeGroupIndex(ShipTypeGroup shipTypeGroup, int index) {
            db.shipTypeGroups.MoveToIndex(shipTypeGroup, index);
            treeView.SetDataRebuildAndSelect(db.shipTypeGroups);
        }

        private void OnSetShipTypeShipGroup(ShipType shiptype, int shipGroupId, int index) {
            db.SetShipTypeShipGroup(shiptype, shipGroupId, index);
            treeView.SetDataRebuildAndSelect(db.shipTypeGroups);
        }

        private void OnCreateShipGroup(int index) {
            ShipTypeGroup group = db.CreateAsset<ShipTypeGroup>();
            treeView.SetDataRebuildAndSelect(db.shipTypeGroups, group.id);
        }

        private void OnCreateShipDefinition(int groupId) {
            ShipType shipType = db.CreateAsset<ShipType>();
            db.FindAsset<ShipTypeGroup>(groupId).AddShipDefinition(shipType);
            treeView.SetDataRebuildAndSelect(db.shipTypeGroups, shipType.id);
        }

        private void OnDeleteAsset(int id) {
            db.DestroyAsset<ShipType>(id);
            treeView.SetDataRebuildAndSelect(db.shipTypeGroups);
        }
        
        public override void OnDisable() {
            treeView.selectionChanged -= OnSelectionChanged;
            treeView.createShipType -= OnCreateShipDefinition;
            treeView.createShipTypeGroup -= OnCreateShipGroup;
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
                        treeView.SetDataRebuildAndSelect(db.shipTypeGroups);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
        }

        private void DeleteShipDefinition() {
            if (selection == null) return;
            string name = selection[NameField].stringValue;
            if (EditorUtility.DisplayDialog("Are you sure?", $"Really delete {name}?", "Yup", "Nope")) {
                db.DestroyAsset<ShipType>(selection[IdField].intValue);
                treeView.ClearSelection();
                treeView.Reload();
            }
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