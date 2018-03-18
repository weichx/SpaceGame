using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SpaceGame.Editor.MissionWindow {

    public partial class MissionTreeView : TreeView {

        protected List<FactionDescription> factions;
        private List<TreeViewItem> utilList;

        public MissionTreeView(List<FactionDescription> factions) : base(new TreeViewState()) {
            this.factions = factions;
            this.utilList = new List<TreeViewItem>(16);
            Reload();
        }

        protected override TreeViewItem BuildRoot() {
            TreeViewItem root = new TreeViewItem(0, -1);
            for (int i = 0; i < factions.Count; i++) {
                FactionTreeItem factionItem = new FactionTreeItem(factions[i]);
                root.AddChild(factionItem);
                foreach (EntityDefinition entity in factions[i].entities) {
                    factionItem.AddChild(new EntityTreeItem(entity));
                }
            }
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        public void OnGUILayout() {
            OnGUI(GUILayoutUtility.GetRect(0, 10000, 0, 10000));
        }

        private bool SelectionOfSameType(IList<int> selectedIds) {
            Type type = FindItem(selectedIds[0], rootItem).GetType();
            for (int i = 1; i < selectedIds.Count; i++) {
                if (FindItem(selectedIds[i], rootItem).GetType() != type) {
                    return false;
                }
            }
            return true;
        }

        protected override bool CanStartDrag(CanStartDragArgs args) {
            return args.draggedItemIDs.Count == 1 || SelectionOfSameType(args.draggedItemIDs);
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args) {
            DragAndDrop.PrepareStartDrag();
            bool isMultiDrag = args.draggedItemIDs.Count > 1;
            if (isMultiDrag) { }
            else {
                DragAndDrop.SetGenericData("MissionEditor_Drag", "something");
            }
            DragAndDrop.StartDrag("MissionEditor");
        }

        private void DragEntityToFaction() { }
        
        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args) {
            if (args.performDrop) {
                
                switch (args.dragAndDropPosition) {

                    case DragAndDropPosition.UponItem:
                        break;
                    case DragAndDropPosition.BetweenItems:
                        break;
                    case DragAndDropPosition.OutsideItems:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return DragAndDropVisualMode.Move;
        }

    }

}