using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SpaceGame.Editor.MissionWindow {

    public partial class MissionTreeView : TreeView {

        private MissionDefinition mission;
        private List<int> utilList;
        private Action<MissionTreeSelection> onSelectionChanged;

        public MissionTreeView(MissionDefinition mission, TreeViewState state, Action<MissionTreeSelection> onSelectionChanged) : base(state) {
            this.mission = mission;
            this.utilList = new List<int>(16);
            this.onSelectionChanged = onSelectionChanged;
            Reload();
        }

        protected override void SelectionChanged(IList<int> selectedIds) {
            onSelectionChanged(GetFilteredSelection(selectedIds));
        }

        private MissionTreeSelection GetFilteredSelection(IList<int> selectedIds) {
            List<AssetDefinition> selection = GetSelectionOfSameType(selectedIds);
            if (selection == null || selection.Count == 0) {
                return new MissionTreeSelection(ItemType.None, null);
            }
            else {
                ItemType itemType = ((MissionTreeItem) FindItem(selectedIds[0], rootItem)).itemType;
                return new MissionTreeSelection(itemType, selection);
            }
        }

        protected override TreeViewItem BuildRoot() {
            TreeViewItem root = new TreeViewItem(-9990, -1);

            foreach (FactionDefinition factionDefinition in mission.factionsDefinitions) {

                MissionTreeItem factionItem = new MissionTreeItem(factionDefinition);
                foreach (FlightGroupDefinition flightGroup in factionDefinition.flightGroups) {

                    MissionTreeItem flightGroupItem = new MissionTreeItem(flightGroup);

                    foreach (EntityDefinition entity in flightGroup.entities) {
                        flightGroupItem.AddChild(new MissionTreeItem(entity));
                    }
                    factionItem.AddChild(flightGroupItem);

                }

                root.AddChild(factionItem);

            }

            if (!root.hasChildren) {
                root.children = new List<TreeViewItem>();
            }
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        [PublicAPI]
        public void UpdateDisplayName(AssetDefinition asset) {
            TreeViewItem item = FindItem(asset.id, rootItem);
            if (item != null) item.displayName = asset.name;
        }

        [PublicAPI]
        public void UpdateDisplayName(int id, string name) {
            TreeViewItem item = FindItem(id, rootItem);
            if (item != null) item.displayName = name ?? string.Empty;
        }

        protected override void ContextClickedItem(int itemId) {
            MissionTreeItem item = FindMissionItem(itemId);
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create Entity"), false, OnCreateEntity, item);
            menu.AddItem(new GUIContent("Create Faction"), false, OnCreateFaction, item);
            menu.AddItem(new GUIContent("Create Flight Group"), false, OnCreateFlightGroup, item);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Delete"), false, OnDeleteItem, item);
            menu.ShowAsContext();
        }

        private void OnCreateEntity(object data) {
            EntityDefinition entityDefinition;
            MissionTreeItem clickedItem = (MissionTreeItem) data;

            switch (clickedItem.itemType) {
                case ItemType.Faction:
                    entityDefinition = clickedItem.GetFaction().GetDefaultFlightGroup().AddEntity();
                    break;
                case ItemType.Entity:
                case ItemType.FlightGroup:
                    entityDefinition = clickedItem.GetFlightGroup().AddEntity();
                    Debug.Log(clickedItem.GetFlightGroup().entities.Count);
                    break;
                default:
                    return;
            }

            Reload();
            SelectFireAndFrame(entityDefinition.id);
        }

        private void OnCreateFlightGroup(object data) {
            MissionTreeItem item = (MissionTreeItem) data;
            FlightGroupDefinition flightGroup = item.GetFaction().AddFlightGroup();
            if (flightGroup != null) {
                Reload();
                SelectFireAndFrame(flightGroup.id);
            }
        }

        private void OnCreateFaction(object data) {
            FactionDefinition faction = mission.AddFaction();
            Reload();
            SelectFireAndFrame(faction.id);
        }

        private void OnDeleteItem(object userdata) {
            MissionTreeItem item = (MissionTreeItem) userdata;
            AssetDefinition removed;
            switch (item.itemType) {
                case ItemType.Faction:
                    removed = mission.RemoveFaction(item.GetFaction());
                    break;
                case ItemType.FlightGroup:
                    removed = item.GetFaction().RemoveFlightGroup(item.GetFlightGroup());
                    break;
                case ItemType.Entity:
                    removed = item.GetFlightGroup().RemoveEntity(item.GetEntity());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (removed != null && IsInSelectedHierarchy(removed.id)) {
                ClearSelection();
            }
            Reload();
        }

        private bool IsInSelectedHierarchy(int id) {
            TreeViewItem item = FindItem(id, rootItem);
            IList<int> selection = GetSelection();
            while (item != rootItem) {
                if (selection.Contains(item.id)) {
                    return true;
                }
                item = item.parent;
            }
            return false;
        }

        private void ClearSelection() {
            utilList.Clear();
            SetSelection(utilList);
        }

        private void SelectFireAndFrame(int id) {
            utilList.Clear();
            utilList.Add(id);
            SetSelection(utilList, TreeViewSelectionOptions.RevealAndFrame | TreeViewSelectionOptions.FireSelectionChanged);
        }

        private void SelectFireAndFrame(TreeViewItem item) {
            utilList.Clear();
            utilList.Add(item.id);
            SetSelection(utilList, TreeViewSelectionOptions.RevealAndFrame | TreeViewSelectionOptions.FireSelectionChanged);
        }

        public void OnGUILayout() {
            OnGUI(GUILayoutUtility.GetRect(0, 10000, 0, 10000));
        }

        protected override bool CanStartDrag(CanStartDragArgs args) {
            return args.draggedItemIDs.Count == 1; // || SelectionOfSameType(args.draggedItemIDs);
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args) {
            DragAndDrop.PrepareStartDrag();
            bool isMultiDrag = args.draggedItemIDs.Count > 1;
            if (isMultiDrag) { }
            else {
                DragAndDrop.SetGenericData("MissionEditor_Drag", SortItemIDsInRowOrder(args.draggedItemIDs)[0]);
            }
            DragAndDrop.StartDrag("MissionEditor");
        }

        private void OnEntityDrop(AssetDefinition dropTarget, EntityDefinition child, int index) {
            MissionTreeItem dropItem = FindMissionItem(child.id);
            MissionTreeItem dropTargetItem = FindMissionItem(dropTarget.id);
            if (dropTarget is FactionDefinition) {
                dropItem.GetFlightGroup().RemoveEntity(child);
                dropTargetItem.GetFaction().GetDefaultFlightGroup().AddEntity(child);
            }
            else if (dropTarget is EntityDefinition || dropTarget is FlightGroupDefinition) {
                if (dropTargetItem.parent == dropItem.parent) {
                    dropItem.GetFlightGroup().MoveEntity(child, index);
                }
                else {
                    dropItem.GetFlightGroup().RemoveEntity(child);
                    dropTargetItem.GetFlightGroup().InsertEntity(child, index);
                }
            }

        }

        private void OnFlightGroupDrop(AssetDefinition dropTarget, FlightGroupDefinition child, int index) {
//            MissionTreeItem dropItem = FindMissionItem(child.id);
//            MissionTreeItem dropTargetItem = FindMissionItem(dropTarget.id);
//            if (dropTarget is FactionDefinition) {
//                if (dropTarget == dropItem.GetFaction()) {
//                    dropItem.GetFaction().MoveFlightGroup(child, index);
//                }
//                else {
//                    dropItem.GetFaction().RemoveFlightGroup(child);
//                    dropTargetItem.InsertFaction(index);
//                }
//                dropItem.GetFlightGroup().RemoveEntity(dropItem.GetEntity());
//                dropTargetItem.GetFaction().GetDefaultFlightGroup().AddEntity(dropItem.GetEntity());
//            }
//            else if (dropTarget is FlightGroupDefinition) {
//                if (dropTargetItem.parent == dropItem.parent) {
//                    dropItem.GetFlightGroup().MoveEntity(child, index);
//                }
//                else {
//                    dropItem.GetFlightGroup().RemoveEntity(dropItem.GetEntity());
//                    dropTargetItem.GetFlightGroup().InsertEntity(dropItem.GetEntity(), index);
//                }
//            }

        }

        private void OnFactionDrop(AssetDefinition dropTarget, FactionDefinition child, int index) {
            if (dropTarget == null) {
                mission.MoveFaction(child, index);
            }
            else if (dropTarget is FactionDefinition) {
                if (index == -1) {
                    index = mission.factionsDefinitions.IndexOf((FactionDefinition) dropTarget);
                }
                mission.MoveFaction(child, index);
                Reload();
            }
//            MissionTreeItem dropItem = FindMissionItem(child.id);
//            MissionTreeItem dropTargetItem = FindMissionItem(dropTarget.id);
//            if (dropTarget == null || dropTarget is FactionDefinition) {

//            }
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args) {
            if (args.performDrop) {

                if (args.dragAndDropPosition == DragAndDropPosition.OutsideItems) return DragAndDropVisualMode.Rejected;

                MissionTreeItem newParentItem;

                if (args.parentItem == rootItem) {
                    newParentItem = ToMissionItem(args.parentItem.children[args.insertAtIndex]);
                }
                else {
                    newParentItem = ToMissionItem(args.parentItem);
                }

                MissionTreeItem droppedItem = FindMissionItem((int) DragAndDrop.GetGenericData("MissionEditor_Drag"));

                if (droppedItem.IsEntity) {
                    OnEntityDrop(newParentItem.asset, droppedItem.GetEntity(), args.insertAtIndex);
                }
                else if (droppedItem.IsFlightGroup) {
                    OnFlightGroupDrop(newParentItem.asset, droppedItem.GetFlightGroup(), args.insertAtIndex);
                }
                else if (newParentItem.IsFaction) {
                    OnFactionDrop(newParentItem.asset, droppedItem.GetFaction(), args.insertAtIndex);
                }

            }
            Reload();
            return DragAndDropVisualMode.Move;
        }

        private MissionTreeItem FindNextValidEntityParent(MissionTreeItem parent) {
            while (parent != null && !parent.IsFaction && !parent.IsEntity) {
                parent = parent.parent as MissionTreeItem;
            }
            return parent;
        }

        private MissionTreeItem ToMissionItem(TreeViewItem item) {
            return (MissionTreeItem) item;
        }

        private List<AssetDefinition> GetSelectionOfSameType(IList<int> selectedIds) {
            if (selectedIds.Count == 0) return null;
            List<AssetDefinition> retn = new List<AssetDefinition>(selectedIds.Count);
            MissionTreeItem firstItem = FindMissionItem(selectedIds[0]);
            ItemType itemType = firstItem.itemType;
            retn.Add(firstItem.asset);
            for (int i = 1; i < selectedIds.Count; i++) {
                MissionTreeItem item = FindMissionItem(selectedIds[i]);
                if (item.itemType != itemType) {
                    return null;
                }
                retn.Add(item.asset);
            }
            return retn;
        }

        private bool SelectionOfSameType(IList<int> selectedIds) {
            return GetSelectionOfSameType(selectedIds) != null;
        }

        private MissionTreeItem FindMissionItem(int id) {
            return (MissionTreeItem) FindItem(id, rootItem);
        }

    }

}