using System.Collections.Generic;
using SpaceGame.EditorComponents;
using SpaceGame.FileTypes;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public class ShipPage : MissionWindowPage {

        [SerializeField] private HorizontalPaneState splitterState;
        protected ReflectedProperty selection;


        private const string NameField = nameof(ShipDefinition.name);

        public ShipPage(MissionWindowState state, GameDataFile gameData) : base(state, gameData) {
            splitterState = new HorizontalPaneState();
        }

        public override void OnGUI() {
            InfamyGUI.HorizontalSplitPane(splitterState, RenderList, RenderDetails);
        }

        private void RenderDetails() {
            if (selection == null || treeView == null) return;
            treeView.UpdateDisplayName(
                selection.GetValue<ShipDefinition>().id,
                selection[NameField].stringValue
            );
            EditorGUILayout.BeginVertical((GUILayoutOption[]) null);
            EditorGUILayoutX.PropertyField(selection);
            GUILayout.FlexibleSpace();
            InfamyGUI.Button("Delete Defintion", DeleteShipDefinition);
            EditorGUILayout.EndHorizontal();
        }

        private void DeleteShipDefinition() {
            if (selection == null) return;
            string name = selection[NameField].stringValue;
            if (EditorUtility.DisplayDialog("Are you sure?", $"Really delete {name}?", "Yup", "Nope")) {
                list.RemoveElement(selection);
                selection = null;
                treeView.SetSelection(new List<int>());
                treeView.Reload();
            }
        }

        private void RenderList() {
            EditorGUILayout.BeginVertical((GUILayoutOption[]) null);
            InfamyGUI.Button("Create Ship Def", CreateShipDefintion);
            treeView.OnGUI(GUILayoutUtility.GetRect(0, 10000, 0, 10000));
            EditorGUILayout.EndVertical();
        }

        private void CreateShipDefintion() {
            ShipDefinition def = new ShipDefinition();
            list.AddElement(def);
            selectedIds.Clear();
            selectedIds.Add(def.id);
            treeView.Reload();
            treeView.SetSelection(selectedIds, TreeViewSelectionOptions.FireSelectionChanged | TreeViewSelectionOptions.RevealAndFrame);
        }

        public override void OnEnable() {
            list = new ReflectedObject(gameData.GetShipDefintions()).Root as ReflectedListProperty;
            treeView = new ShipTreeView(list, OnSelectionChanged);
        }

        private void OnSelectionChanged(IList<int> selectedIds) {
            this.selectedIds = new List<int>(selectedIds);
            selection = list.Find(FindSelected);
        }

        public override void OnDisable() {
            list.ApplyChanges();
        }
        
        private bool FindSelected(ReflectedProperty property) {
            return selectedIds.Contains(property.GetValue<IIdentitifiable>().Id);
        }

    }

}