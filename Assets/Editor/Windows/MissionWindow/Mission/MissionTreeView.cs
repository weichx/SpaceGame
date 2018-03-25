using System.Collections.Generic;
using SpaceGame.Assets;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SpaceGame.Editor.MissionWindow {

    public partial class MissionTreeView : TreeBase {

        private MissionDefinition mission;

        public delegate void SelectionChangedCallback(MissionTreeSelection selection);

        public delegate EntityDefinition CreateEntityCallback(FlightGroupDefinition flightGroup);

        public delegate FlightGroupDefinition CreateFlightGroupCallback(FactionDefinition faction);

        public delegate FactionDefinition CreateFactionCallback();

        public delegate void DeleteAssetCallback(MissionAsset missionAsset);

        public delegate void SetEntityFactionCallback(EntityDefinition entity, FactionDefinition faction, int index);

        public delegate void SetEntityFlightGroupCallback(EntityDefinition entity, FlightGroupDefinition fg, int index);

        public delegate void SetFlightGroupFactionCallback(FlightGroupDefinition flightGroup, FactionDefinition factionDefinition, int index);

        public delegate void SetFactionIndexCallback(FactionDefinition factionDefinition, int index);

        public event SelectionChangedCallback selectionChanged;

        public event SetFactionIndexCallback setFactionIndex;
        public event SetFlightGroupFactionCallback setFlightGroupFaction;
        public event SetEntityFactionCallback setEntityFaction;
        public event SetEntityFlightGroupCallback setEntityFlightGroup;
        public event CreateEntityCallback createEntity;
        public event CreateFactionCallback createFaction;
        public event CreateFlightGroupCallback createFlightGroup;
        public event DeleteAssetCallback deleteAsset;

        public MissionTreeView(TreeViewState state) : base(state) { }

        public void SetDataRebuildAndSelect(MissionDefinition mission, int selectionId = -1) {
            this.mission = mission;
            Reload();
            if (selectionId != -1) {
                SelectFireAndFrame(selectionId);
            }
        }
        
        protected override void SelectionChanged(IList<int> selectedIds) {
            selectionChanged?.Invoke(GetFilteredSelection(selectedIds));
        }

        private MissionTreeSelection GetFilteredSelection(IList<int> selectedIds) {
            List<MissionAsset> selection = GetSelectionOfSameType(selectedIds);
            if (selection == null || selection.Count == 0) {
                return new MissionTreeSelection(ItemType.Root, null);
            }
            else {
                ItemType itemType = ((MissionTreeItem) FindItem(selectedIds[0], rootItem)).itemType;
                return new MissionTreeSelection(itemType, selection);
            }
        }

        protected override TreeViewItem BuildRoot() {
            MissionTreeItem root = new MissionTreeItem();

            mission.factions.ForEach((faction) => {

                MissionTreeItem factionItem = new MissionTreeItem(faction);

                faction.flightGroups.ForEach((flightGroup) => {

                    MissionTreeItem flightGroupItem = new MissionTreeItem(flightGroup);

                    factionItem.AddChild(flightGroupItem);

                    flightGroup.entities.ForEach((entity) => {
                        flightGroupItem.AddChild(new MissionTreeItem(entity));
                    });

                });

                root.AddChild(factionItem);

            });

            if (!root.hasChildren) {
                root.children = new List<TreeViewItem>();
            }

            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        public void PingSelection() {
            selectionChanged?.Invoke(GetFilteredSelection(state.selectedIDs));
        }

        protected override void ContextClicked() {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create Faction"), false, OnCreateFaction, null);
            menu.ShowAsContext();
            Event.current.Use();
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
            Event.current.Use();
        }

        private void OnCreateEntity(object data) {
            MissionTreeItem clickedItem = (MissionTreeItem) data;
            switch (clickedItem.itemType) {
                case ItemType.Entity:
                case ItemType.Faction:
                case ItemType.FlightGroup:
                    createEntity?.Invoke(clickedItem.GetFlightGroup());
                    break;
                default:
                    return;
            }
        }

        private void OnCreateFlightGroup(object data) {
            MissionTreeItem item = (MissionTreeItem) data;
            createFlightGroup?.Invoke(item.GetFaction());
        }

        private void OnCreateFaction(object data) {
            createFaction?.Invoke();
        }

        private void OnDeleteItem(object userdata) {
            MissionTreeItem item = (MissionTreeItem) userdata;
            deleteAsset?.Invoke(item.asset);
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

        private void OnEntityDrop(EntityDefinition entity, MissionAsset dropTarget, int index) {
            MissionTreeItem dropTargetItem = FindMissionItem(dropTarget.id);
            FactionDefinition factionDefinition = dropTarget as FactionDefinition;
            if (factionDefinition != null) {
                setEntityFaction?.Invoke(entity, factionDefinition, index);
            }
            else if (dropTarget is EntityDefinition) {
                setEntityFlightGroup?.Invoke(entity, dropTargetItem.GetFlightGroup(), index);
            }
            else if (dropTarget is FlightGroupDefinition) {
                setEntityFlightGroup?.Invoke(entity, (FlightGroupDefinition) dropTarget, index);
            }
        }

        private void OnFlightGroupDrop(FlightGroupDefinition child, MissionAsset dropTarget, int index) {
            MissionTreeItem dropTargetItem = FindMissionItem(dropTarget.id);
            FactionDefinition faction = dropTarget as FactionDefinition;
            FlightGroupDefinition flightGroup = dropTarget as FlightGroupDefinition;
            if (faction != null) {
                setFlightGroupFaction?.Invoke(child, faction, index);
            }
            else if (flightGroup != null) {
                setFlightGroupFaction?.Invoke(child, dropTargetItem.GetFaction(), index);
            }

        }

        private void OnFactionDrop(FactionDefinition faction, int index) {
            setFactionIndex?.Invoke(faction, index);
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args) {
            if (args.performDrop) {

                if (args.dragAndDropPosition == DragAndDropPosition.OutsideItems) {
                    return DragAndDropVisualMode.Rejected;
                }

                MissionTreeItem droppedItem = FindMissionItem((int) DragAndDrop.GetGenericData("MissionEditor_Drag"));

                if (args.parentItem == rootItem && !droppedItem.IsFaction) {
                    return DragAndDropVisualMode.None;
                }

                MissionTreeItem droppedOn = ToMissionItem(args.parentItem);

                if (!droppedItem.CanDropOn(droppedOn)) {
                    return DragAndDropVisualMode.Rejected;
                }

                if (droppedItem.CanDropOn(droppedOn)) {
                    switch (args.dragAndDropPosition) {
                        case DragAndDropPosition.BetweenItems:
                            if (droppedItem.IsEntity) {
                                OnEntityDrop(droppedItem.GetEntity(), droppedOn.asset, args.insertAtIndex);
                            }
                            else if (droppedItem.IsFlightGroup) {
                                OnFlightGroupDrop(droppedItem.GetFlightGroup(), droppedOn.asset, args.insertAtIndex);
                            }
                            else if (droppedItem.IsFaction) {
                                OnFactionDrop(droppedItem.GetFaction(), args.insertAtIndex);
                            }
                            break;
                        case DragAndDropPosition.UponItem:
                            if (droppedItem.IsEntity) {
                                OnEntityDrop(droppedItem.GetEntity(), droppedOn.GetFlightGroup(), -1);
                            }
                            else if (droppedItem.IsFlightGroup) {
                                OnFlightGroupDrop(droppedItem.GetFlightGroup(), droppedOn.GetFaction(), -1);
                            }
                            else if (droppedItem.IsFaction) {
                                OnFactionDrop(droppedItem.GetFaction(), args.insertAtIndex);
                            }
                            break;
                    }
                }
            }
            return DragAndDropVisualMode.Move;
        }

        private MissionTreeItem ToMissionItem(TreeViewItem item) {
            return item as MissionTreeItem;
        }

        private List<MissionAsset> GetSelectionOfSameType(IList<int> selectedIds) {
            if (selectedIds.Count == 0) return null;
            List<MissionAsset> retn = new List<MissionAsset>(selectedIds.Count);
            MissionTreeItem firstItem = FindMissionItem(selectedIds[0]);
            if (firstItem == null) {
                return null;
            }
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