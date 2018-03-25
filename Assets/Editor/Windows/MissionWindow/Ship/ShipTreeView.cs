using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using Weichx.Util;

namespace SpaceGame.Editor.MissionWindow {

    public class ShipTreeView : TreeBase {

        private List<ShipDefinition> shipDefinitions;

        public delegate void HierarchyChangedCallback();

        public delegate void SelectionChangedCallback(ShipDefinition shipDefinition);

        public event SelectionChangedCallback selectionChanged;
        public event HierarchyChangedCallback hierarchyChanged;

        public ShipTreeView(TreeViewState state) : base(state) { }

        public void SetDataAndRebuild(List<ShipDefinition> shipDefinitions) {
            this.shipDefinitions = shipDefinitions;
            Reload();
        }

        protected virtual void OnHierarchyChanged() {
            hierarchyChanged?.Invoke();
        }

        protected virtual void OnSelectionChanged(ShipDefinition shipdefinition) {
            selectionChanged?.Invoke(shipdefinition);
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args) {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData("ShipTreeView", args.draggedItemIDs[0]);
            DragAndDrop.StartDrag("ShipTreeView_Drag");
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args) {
            if (args.performDrop && args.parentItem is ShipTreeRootItem) {
                ShipTreeRootItem parentItem = (ShipTreeRootItem) args.parentItem;
                TreeViewItem item = FindItem((int) DragAndDrop.GetGenericData("ShipTreeView"), rootItem);
                ShipTreeItem shipItem = item as ShipTreeItem;

                if (shipItem == null) return DragAndDropVisualMode.Rejected;

                if (parentItem.shipType == shipItem.shipDefinition.shipType) {
                    int startIndex = shipDefinitions.FindIndex((def) => def.shipType == parentItem.shipType);
                    int currentIndex = shipDefinitions.IndexOf(shipItem.shipDefinition);
                    shipDefinitions.MoveToIndex(currentIndex, startIndex + args.insertAtIndex);
                }
                
                Reload();
            }

            return DragAndDropVisualMode.Move;
        }

        protected override bool CanStartDrag(CanStartDragArgs args) {
            return args.draggedItem is ShipTreeItem;
        }

        protected override TreeViewItem BuildRoot() {
            TreeViewItem root = new TreeViewItem(0, -1);
            string[] names = Enum.GetNames(typeof(ShipType));

            var groups = shipDefinitions
                .GroupBy((shipDef) => shipDef.shipType);

            for (int i = 0; i < names.Length; i++) {

                ShipTreeRootItem item = new ShipTreeRootItem(-(i + 1), (ShipType) i);

                ShipType shipType = (ShipType) i;

                var group = groups.FirstOrDefault((g) => g.Key == shipType);
                if (group != null) {
                    item.children = group.Select((shipDef) => {
                        return new ShipTreeItem(shipDef) as TreeViewItem;
                    }).ToList();
                }

                root.AddChild(item);
            }

            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        protected override void SelectionChanged(IList<int> selectedIds) {
            selectedIds = SortItemIDsInRowOrder(selectedIds);
            if (selectedIds.Count == 0) {
                OnSelectionChanged(null);
            }
            else {
                TreeViewItem item = FindItem(selectedIds[0], rootItem);
                ShipTreeItem treeItem = item as ShipTreeItem;
                if (treeItem != null) {
                    OnSelectionChanged(treeItem.shipDefinition);
                }
                else {
                    OnSelectionChanged(null);
                }
            }
        }

        private class ShipTreeItem : TreeViewItem {

            public readonly ShipDefinition shipDefinition;

            public ShipTreeItem(ShipDefinition shipDefinition) {
                this.shipDefinition = shipDefinition;
                this.id = shipDefinition.id;
                this.displayName = shipDefinition.name;
            }

        }

        private class ShipTreeRootItem : TreeViewItem {

            public readonly ShipType shipType;

            public ShipTreeRootItem(int id, ShipType shipType) {
                this.id = id;
                this.shipType = shipType;
                this.displayName = Enum.GetName(typeof(ShipType), shipType);
            }

        }

    }

}