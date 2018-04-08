using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SpaceGame.Editor.MissionWindow {

    public partial class MissionTreeView : TreeBase {

        public delegate void SelectionChangedCallback(MissionTreeSelection selection);

        public event SelectionChangedCallback selectionChanged;

        private GameDatabase db;

        public MissionTreeView(GameDatabase db, TreeViewState state) : base(state) {
            this.db = db;
        }

        public void SetDataRebuildAndSelect(int selectionId = -1) {
            Reload();
            if (selectionId != -1) {
                SelectFireAndFrame(selectionId);
            }
        }

        protected override void SelectionChanged(IList<int> selectedIds) {
            selectionChanged?.Invoke(GetFilteredSelection(selectedIds));
            Entity entity = GameDatabase.ActiveInstance.SceneEntityFromDefinitionId(selectedIds[0]);
            EditorGUIUtility.PingObject(entity?.gameObject);
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

        protected override void DoubleClickedItem(int id) {
            MissionTreeItem item = FindMissionItem(id);
            Entity entity = GameDatabase.ActiveInstance.SceneEntityFromDefinitionId(item.id);
            EditorGUIUtility.PingObject(entity?.gameObject);
            Selection.objects = new Object[] { entity?.gameObject };
        }

        protected override TreeViewItem BuildRoot() {
            MissionTreeItem root = new MissionTreeItem();

            GameDatabase.ActiveInstance.GetCurrentMission().factions.ForEach((faction) => {

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
            menu.AddItem(new GUIContent("Create Faction"), false, () => {
                SelectFireAndFrame(db.GetCurrentMission().CreateFaction().id);
            });
            menu.ShowAsContext();
            Event.current.Use();
        }

        protected override void ContextClickedItem(int itemId) {
            MissionTreeItem item = FindMissionItem(itemId);
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create Entity"), false, () => {
                SelectFireAndFrame(db.GetCurrentMission().CreateEntityDefinition(item.GetFlightGroup()).id);
            });
            menu.AddItem(new GUIContent("Create Faction"), false, () => {
                SelectFireAndFrame(db.GetCurrentMission().CreateFaction().id);
            });
            menu.AddItem(new GUIContent("Create Flight Group"), false, () => {
                SelectFireAndFrame(db.GetCurrentMission().CreateFlightGroup(item.GetFaction()).id);

            });
            menu.AddSeparator("");
            menu.AddItem(new GUIContent($"Delete {item.displayName}"), false, () => {
                db.GetCurrentMission().DeleteAsset(item.asset);
                ClearSelection();
                Reload();
            });
            menu.ShowAsContext();
            Event.current.Use();
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
                db.GetCurrentMission().SetEntityFaction(entity, factionDefinition, index);
            }
            else if (dropTarget is EntityDefinition) {
                db.GetCurrentMission().SetEntityFlightGroup(entity, dropTargetItem.GetFlightGroup(), index);
            }
            else if (dropTarget is FlightGroupDefinition) {
                db.GetCurrentMission().SetEntityFlightGroup(entity, (FlightGroupDefinition) dropTarget, index);
            }
        }

        private void OnFlightGroupDrop(FlightGroupDefinition child, MissionAsset dropTarget, int index) {
            MissionTreeItem dropTargetItem = FindMissionItem(dropTarget.id);
            FactionDefinition faction = dropTarget as FactionDefinition;
            FlightGroupDefinition flightGroup = dropTarget as FlightGroupDefinition;
            if (faction != null) {
                db.GetCurrentMission().SetFlightGroupFaction(child, faction, index);
            }
            else if (flightGroup != null) {
                db.GetCurrentMission().SetFlightGroupFaction(child, dropTargetItem.GetFaction(), index);
            }
        }

        private void OnFactionDrop(FactionDefinition faction, int index) {
            db.GetCurrentMission().SetFactionIndex(faction, index);
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