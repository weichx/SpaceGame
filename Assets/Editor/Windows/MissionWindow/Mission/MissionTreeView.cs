using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public partial class MissionTreeView : TreeView {

        private const string FactionListField = nameof(MissionDefinition.factionsDefinitions);
        private const string FactionEntityListField = nameof(FactionDefinition.entities);

        private ReflectedObject mission;
        private List<int> utilList;
        private Action<MissionTreeSelection> onSelectionChanged;

        public MissionTreeView(ReflectedObject mission, Action<MissionTreeSelection> onSelectionChanged) : base(new TreeViewState()) {
            this.mission = mission;
            this.utilList = new List<int>(16);
            this.onSelectionChanged = onSelectionChanged;
            Reload();
        }

        protected override void SelectionChanged(IList<int> selectedIds) {
            onSelectionChanged(GetFilteredSelection(selectedIds));
        }

        private MissionTreeSelection GetFilteredSelection(IList<int> selectedIds) {
            List<ReflectedProperty> selectedProperties = GetSelectionOfSameType(selectedIds);
            if (selectedProperties == null || selectedProperties.Count == 0) {
                return new MissionTreeSelection(ItemType.None, null);
            }
            else {
                ItemType itemType = ((MissionTreeItem) FindItem(selectedIds[0], rootItem)).itemType;
                return new MissionTreeSelection(itemType, selectedProperties);
            }
        }

        protected override TreeViewItem BuildRoot() {
            TreeViewItem root = new TreeViewItem(-9990, -1);

            ReflectedListProperty factionList = mission.GetList(FactionListField);
            int childCount = factionList.ElementCount;

            for (int i = 0; i < childCount; i++) {
                MissionTreeItem factionItem = new MissionTreeItem(factionList[i]);
                root.AddChild(factionItem);

                ReflectedListProperty entities = factionItem.property.GetList(FactionEntityListField);
                for (int j = 0; j < entities.ChildCount; j++) {
                    ReflectedProperty child = entities[j];
                    factionItem.AddChild(new MissionTreeItem(child));
                }

            }

            if (!root.hasChildren) {
                // default this or we get a null exception
                root.AddChild(new TreeViewItem(1, 0));
            }

            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        public void UpdateDisplayName(ReflectedProperty property) {
            int id = property[nameof(AssetDefinition.id)].intValue;
            string name = property[nameof(AssetDefinition.name)].stringValue;
            TreeViewItem item = FindItem(id, rootItem);
            if (item != null) item.displayName = name;
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
            menu.AddItem(new GUIContent("Delete"), false, OnDeleteItem, item);
            menu.ShowAsContext();
        }

        private void OnDeleteItem(object userdata) {
            MissionTreeItem item = (MissionTreeItem) userdata;
            switch (item.itemType) {

                case ItemType.None:
                    break;
                case ItemType.Faction:
                    mission.GetList(FactionListField).RemoveElement(item.property);
                    Reload();
                    break;
                case ItemType.FlightGroup:
                    break;
                case ItemType.Entity:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnCreateEntity(object userdata) {
            MissionTreeItem item = (MissionTreeItem) userdata;
            EntityDefinition entityDefinition = new EntityDefinition();

            switch (item.itemType) {
                case ItemType.Faction:
                    //todo maybe delegate this
                    item.property.GetList(FactionEntityListField).AddElement(entityDefinition);
                    break;
                case ItemType.Entity:
                    MissionTreeItem itemParent = item.ParentAsMissionTreeItem;
                    if (itemParent.itemType == ItemType.Faction) {
                        item.ParentAsMissionTreeItem.property.GetList(FactionEntityListField).AddElement(entityDefinition);
                    }
                    else if (itemParent.itemType == ItemType.FlightGroup) { }
                    break;
                case ItemType.FlightGroup:
                    break;
            }

            Reload();
            SelectFireAndFrame(entityDefinition.id);
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
                DragAndDrop.SetGenericData("MissionEditor_Drag", "something");
            }
            DragAndDrop.StartDrag("MissionEditor");
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args) {
            if (args.performDrop) {
                MissionTreeItem parent = ToMissionItem(args.parentItem);
                MissionTreeItem droppedItem = FindMissionItem((int) DragAndDrop.GetGenericData("MissionEditor_Drag"));

                if (parent.IsFaction) {
                    
                    ReflectedListProperty droppedParentFactionList = parent.property.GetList(FactionEntityListField);
                    
                    switch (droppedItem.itemType) {
                        case ItemType.None:
                            break;
                        case ItemType.Faction:
                            break;
                        case ItemType.FlightGroup:
                            break;
                        case ItemType.Entity:
                            if (droppedItem.parent == parent) {
                                droppedParentFactionList.MoveElement(droppedItem.property, args.insertAtIndex);
                            }
                            else {
                                droppedParentFactionList.RemoveElement(droppedItem.property);
                                parent.property.GetList(FactionEntityListField).InsertElement(droppedItem.property, args.insertAtIndex);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else if (parent.IsEntity) {
                    if (droppedItem.IsEntity) {
                        MissionTreeItem targetParent = FindNextValidEntityParent(parent);
                        
                    }
                }
                else if (parent.IsFlightGroup) { }

                switch (args.dragAndDropPosition) {

                    case DragAndDropPosition.UponItem:
                        break;
                    case DragAndDropPosition.BetweenItems:
//                        FindNextValidInsertParent(args.parentItemargs.parentItem
                        break;
                    case DragAndDropPosition.OutsideItems:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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

        private void DragEntityToFaction() { }

        private List<ReflectedProperty> GetSelectionOfSameType(IList<int> selectedIds) {
            if (selectedIds.Count == 0) return null;
            List<ReflectedProperty> retn = new List<ReflectedProperty>(selectedIds.Count);
            MissionTreeItem firstItem = FindMissionItem(selectedIds[0]);
            ItemType itemType = firstItem.itemType;
            retn.Add(firstItem.property);
            for (int i = 1; i < selectedIds.Count; i++) {
                MissionTreeItem item = FindMissionItem(selectedIds[i]);
                if (item.itemType != itemType) {
                    return null;
                }
                retn.Add(item.property);
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