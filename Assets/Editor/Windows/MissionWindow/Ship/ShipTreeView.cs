using System;
using System.Collections.Generic;
using System.Linq;
using SpaceGame.Assets;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Networking;
using Weichx.Util;

namespace SpaceGame.Editor.MissionWindow {

    public class ShipTreeView : TreeBase {

        public delegate void SelectionChangedCallback(ShipTreeViewSelection selection);

        public event SelectionChangedCallback selectionChanged;

        public ShipTreeView(TreeViewState state) : base(state) { }

        public void SetDataRebuildAndSelect(int selectionId = -1) {
            Reload();
            if (selectionId != -1) {
                SelectFireAndFrame(selectionId);
            }
        }

        protected override TreeViewItem BuildRoot() {
            TreeViewItem root = new TreeViewItem(-9999, -1);
            IReadonlyListX<ShipTypeGroup> shipTypes = GameDatabase.ActiveInstance.GetAssetList<ShipTypeGroup>();
            for (int i = 0; i < shipTypes.Count; i++) {
                ShipTypeGroupItem item = new ShipTypeGroupItem(i, shipTypes[i]);
                root.AddChild(item);
                if (shipTypes[i].ships == null) shipTypes[i].ships = new List<ShipType>();
                for (int j = 0; j < shipTypes[i].ships.Count; j++) {
                    ShipType shipType = shipTypes[i].ships[j];
                    item.AddChild(new ShipTypeItem(j, shipType));
                }
            }

            if (root.children == null) {
                root.children = new List<TreeViewItem>();
            }

            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        protected override void ContextClicked() {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create Ship Group"), false, () => {
                SetDataRebuildAndSelect(GameDatabase.ActiveInstance.CreateAsset<ShipTypeGroup>().id);
            });
            menu.ShowAsContext();
            Event.current.Use();
        }

        protected override void ContextClickedItem(int id) {
            GenericMenu menu = new GenericMenu();
            TreeViewItem item = FindItem(id, rootItem);
            Debug.Assert(item != null, "item != null");

            ShipTypeGroupItem typeGroupItem = item as ShipTypeGroupItem;
            if (typeGroupItem != null) {
                menu.AddItem(new GUIContent("Create Ship Group"), false, () => {
                    SetDataRebuildAndSelect(GameDatabase.ActiveInstance.CreateAsset<ShipTypeGroup>().id);
                });
                menu.AddItem(new GUIContent("Create Ship Type"), false, () => {
                    ShipType shipType = GameDatabase.ActiveInstance.CreateAsset<ShipType>();
                    GameDatabase.ActiveInstance.FindAsset<ShipTypeGroup>(typeGroupItem.shipTypeGroup.id).ships.Add(shipType);
                    SetDataRebuildAndSelect(shipType.id);

                });
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent($"Delete Ship Group {typeGroupItem.displayName}"), false, () => {
                    GameDatabase.ActiveInstance.DestroyAsset(typeGroupItem.shipTypeGroup);
                    SetDataRebuildAndSelect();
                });
            }
            else if (item is ShipTypeItem) {
                menu.AddItem(new GUIContent("Create Ship Type"), false, () => {
                    ShipType shipType = GameDatabase.ActiveInstance.CreateAsset<ShipType>();
                    GameDatabase.ActiveInstance.FindAsset<ShipTypeGroup>(((ShipTypeItem) item).shipType.id).ships.Add(shipType);
                    SetDataRebuildAndSelect(shipType.id);
                });
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent($"Delete Ship Type {item.displayName}"), false, () => {
                    GameDatabase.ActiveInstance.DestroyAsset(((ShipTypeItem) item).shipType);
                    SetDataRebuildAndSelect();
                });
            }
            menu.ShowAsContext();
            Event.current.Use();
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args) {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData("ShipTreeView", args.draggedItemIDs[0]);
            DragAndDrop.StartDrag("ShipTreeView_Drag");
        }

        private void OnShipTypeDrop(ShipType shipType, TreeViewItem droppedOn, int index) {
            if (droppedOn is ShipTypeItem) {
                ShipTypeItem dropShipItem = (ShipTypeItem) droppedOn;
                //setShipTypeShipGroup?.Invoke(shipType, dropShipItem.shipType.shipGroupId, index);
            }
            else if (droppedOn is ShipTypeGroupItem) {
                //setShipTypeShipGroup?.Invoke(shipType, ((ShipTypeGroupItem)droppedOn).shipTypeGroup.id, index);
            }
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args) {
            if (args.performDrop) {
                TreeViewItem droppedOn = args.parentItem;
                TreeViewItem droppedItem = FindItem((int) DragAndDrop.GetGenericData("ShipTreeView"), rootItem);

                ShipTypeItem item = droppedItem as ShipTypeItem;
                if (item != null) {
                    OnShipTypeDrop(item.shipType, droppedOn, args.insertAtIndex);
                }
                else if (droppedItem is ShipTypeGroupItem) {
                    GameDatabase.ActiveInstance.SetAssetIndex(((ShipTypeGroupItem) droppedItem).shipTypeGroup, args.insertAtIndex);
                }

            }
            return DragAndDropVisualMode.Move;
        }

        protected override bool CanStartDrag(CanStartDragArgs args) {
            return args.draggedItemIDs.Count == 1;
        }

        public struct ShipTreeViewSelection {

            public readonly ShipType ship;
            public readonly ShipTypeGroup shipGroup;

            public ShipTreeViewSelection(ShipTypeGroup shipGroup, ShipType ship) {
                this.shipGroup = shipGroup;
                this.ship = ship;
            }

        }

        protected override void SelectionChanged(IList<int> selectedIds) {
            int id = selectedIds.Count > 0 ? selectedIds[0] : -1;
            TreeViewItem item = FindItem(id, rootItem);
            ShipTypeGroupItem typeGroupItem = item as ShipTypeGroupItem;
            ShipTypeItem shipItem = item as ShipTypeItem;
            selectionChanged?.Invoke(new ShipTreeViewSelection(typeGroupItem?.shipTypeGroup, shipItem?.shipType));
        }

        private class ShipTypeItem : TreeViewItem {

            public readonly int index;
            public readonly ShipType shipType;

            public ShipTypeItem(int index, ShipType shipType) {
                this.index = index;
                this.shipType = shipType;
                this.id = shipType.id;
                this.displayName = shipType.name;
            }

        }

        private class ShipTypeGroupItem : TreeViewItem {

            public readonly int index;
            public readonly ShipTypeGroup shipTypeGroup;

            public ShipTypeGroupItem(int index, ShipTypeGroup shipTypeGroup) {
                this.index = index;
                this.id = shipTypeGroup.id;
                this.shipTypeGroup = shipTypeGroup;
                this.displayName = shipTypeGroup.name;
            }

        }

    }

}