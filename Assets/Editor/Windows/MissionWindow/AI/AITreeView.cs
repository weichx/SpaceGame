using System;
using System.Collections.Generic;
using SpaceGame.Assets;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Weichx.Util;

namespace SpaceGame.Editor.MissionWindow {

    public class AITreeView : TreeBase {

        private List<BehaviorSet> behaviorSets;

        public delegate void SelectionChangedCallback(GameAsset selection);

        public delegate void CreateActionCallback(BehaviorDefinition behaviorDefinition);

        public delegate void CreateBehaviorDefinitionCallback(BehaviorSet behaviorSet);

        public delegate void CreateBehaviorSetCallback();

        public delegate void DeleteAssetCallback(GameAsset gameAsset);

        public delegate void SetActionBehaviorDefinitionCallback(ActionDefinition actionDefinition, BehaviorDefinition behaviorDefinition, int index);

        public delegate void SetBehaviorBehaviorSetCallback(BehaviorDefinition behaviorDefinition, BehaviorSet behaviorSet, int index);

        public delegate void SetBehaviorSetIndexCallback(BehaviorSet behaviorSet, int index);

        public event SelectionChangedCallback selectionChanged;

        public event SetBehaviorSetIndexCallback setBehaviorSetIndex;
        public event SetBehaviorBehaviorSetCallback setBehaviorBehaviorSet;
        public event SetActionBehaviorDefinitionCallback setActionBehavior;
        public event CreateActionCallback createActionDefintion;
        public event CreateBehaviorSetCallback createBehaviorSet;
        public event CreateBehaviorDefinitionCallback createBehaviorDefintion;
        public event DeleteAssetCallback deleteAsset;

        private GameDatabase db;

        public AITreeView(GameDatabase db, TreeViewState state) : base(state) {
            this.db = db;
        }

        public void SetDataRebuildAndSelect(List<BehaviorSet> behaviorSets, int selectionId = -1) {
            this.behaviorSets = behaviorSets;
            Reload();
            if (selectionId != -1) {
                SelectFireAndFrame(selectionId);
            }
        }

        protected override TreeViewItem BuildRoot() {
            AITreeItem root = new AITreeItem();

            foreach (BehaviorSet behaviorSet in behaviorSets) {

                AITreeItem behaviorSetItem = new AITreeItem(behaviorSet);
                root.AddChild(behaviorSetItem);

                foreach (BehaviorDefinition behaviorDefinition in behaviorSet.behaviors) {

                    AITreeItem behaviorItem = new AITreeItem(behaviorDefinition);
                    behaviorSetItem.AddChild(behaviorItem);

                    foreach (ActionDefinition action in behaviorDefinition.actions) {
                        behaviorItem.AddChild(new AITreeItem(action));
                    }

                }
            }

            if (root.children == null) {
                root.children = new List<TreeViewItem>();
            }
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        protected override void SelectionChanged(IList<int> selectedIds) {
            int id = selectedIds.Count > 0 ? selectedIds[0] : -1;
            AITreeItem item = (AITreeItem) FindItem(id, rootItem);
            selectionChanged?.Invoke(item.asset);
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args) {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData("AITree_Data", args.draggedItemIDs[0]);
            DragAndDrop.StartDrag("AITree_Drag");
        }

        private void OnActionDrop(ActionDefinition action, GameAsset droppedOn, int index) {

            if (droppedOn is BehaviorSet) {
                BehaviorSet behaviorSet = (BehaviorSet) droppedOn;
                behaviorSet.GetDefaultBehavior().AddActionDefinition(action, index);
            }
            else if (droppedOn is BehaviorDefinition) {
                ((BehaviorDefinition) droppedOn).AddActionDefinition(action, index);
            }
            else if (droppedOn is ActionDefinition) {
                ActionDefinition droppedOnAction = (ActionDefinition) droppedOn;
                droppedOnAction.GetBehaviorDefinition().AddActionDefinition(action, index);
            }
            SetDataRebuildAndSelect(db.behaviorSets, action.id);
        }

        private void OnBehaviorDefintionDrop(BehaviorDefinition behaviorDefinition, GameAsset droppedOn, int index) {
            if (droppedOn is BehaviorSet) {
                (droppedOn as BehaviorSet).AddBehaviorDefinition(behaviorDefinition, index);
            }
            else if (droppedOn is BehaviorDefinition) {
                BehaviorDefinition bh = droppedOn as BehaviorDefinition;
                bh.GetBehaviorSet().AddBehaviorDefinition(behaviorDefinition);
            }
            SetDataRebuildAndSelect(db.behaviorSets, behaviorDefinition.id);
        }

        private void OnBehaviorSetDrop(BehaviorSet behaviorSet, int index) {
            db.behaviorSets.MoveToIndex(behaviorSet, index);
            SetDataRebuildAndSelect(db.behaviorSets, behaviorSet.id);
        }
        
        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args) {
            if (args.dragAndDropPosition == DragAndDropPosition.OutsideItems) {
                return DragAndDropVisualMode.Rejected;
            }

            if (!args.performDrop) return DragAndDropVisualMode.Move;
            AITreeItem droppedItem = (AITreeItem) FindItem((int) DragAndDrop.GetGenericData("AITree_Data"), rootItem);

            if (args.parentItem == rootItem && !droppedItem.IsBehaviorSet) {
                return DragAndDropVisualMode.Rejected;
            }

            AITreeItem droppedOn = (AITreeItem) args.parentItem;

            if (!droppedItem.CanDropOn(droppedOn)) {
                return DragAndDropVisualMode.Rejected;
            }

            if (droppedItem.IsActionDefintion) {
                OnActionDrop(droppedItem.GetActionDefinition(), droppedOn.asset, args.insertAtIndex);
            }
            else if (droppedItem.IsBehaviorDefinition) {
                OnBehaviorDefintionDrop(droppedItem.GetBehaviorDefinition(), droppedOn.asset, args.insertAtIndex);
            }
            else if (droppedItem.IsBehaviorSet) {
                OnBehaviorSetDrop(droppedItem.GetBehaviorSet(), args.insertAtIndex);
            }

            return DragAndDropVisualMode.Move;
        }

      

        protected override bool CanStartDrag(CanStartDragArgs args) {
            return args.draggedItemIDs.Count == 1;
        }

        protected override void ContextClicked() {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Create Behavior Set"), false, () => {
                BehaviorSet behaviorSet = db.CreateAsset<BehaviorSet>();
                SetDataRebuildAndSelect(db.behaviorSets, behaviorSet.id);
            });

            Event.current.Use();
            menu.ShowAsContext();
        }

        protected override void ContextClickedItem(int id) {
            GenericMenu menu = new GenericMenu();
            AITreeItem item = (AITreeItem) FindItem(id, rootItem);

            menu.AddItem(new GUIContent("Create Behavior Set"), false, () => {
                BehaviorSet behaviorSet = db.CreateAsset<BehaviorSet>();
                SetDataRebuildAndSelect(db.behaviorSets, behaviorSet.id);
            });
            menu.AddItem(new GUIContent("Create Behavior Definition"), false, () => {
                BehaviorDefinition behaviorDefinition = db.CreateAsset<BehaviorDefinition>();
                item.GetBehaviorSet().AddBehaviorDefinition(behaviorDefinition);
                SetDataRebuildAndSelect(db.behaviorSets, behaviorDefinition.id);
            });
            menu.AddItem(new GUIContent("Create Action Definition"), false, () => {
                BehaviorDefinition behaviorDefinition = item.GetBehaviorDefinition();
                ActionDefinition action = db.CreateAsset<ActionDefinition>();
                behaviorDefinition.AddActionDefinition(action);
                SetDataRebuildAndSelect(db.behaviorSets, action.id);
            });

            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent($"Delete {item.displayName}"), false, () => {
                db.DestroyAsset(item.asset);
                SetDataRebuildAndSelect(db.behaviorSets);
            });
            Event.current.Use();
            menu.ShowAsContext();
        }

        private enum ItemType {

            Root,
            BehaviorSet,
            BehaviorDefinition,
            ActionDefinition

        }

        private class AITreeItem : TreeViewItem {

            public readonly ItemType itemType;
            public readonly GameAsset asset;

            public AITreeItem() {
                this.id = -9999;
                this.depth = -1;
                this.itemType = ItemType.Root;
            }

            public AITreeItem(GameAsset asset) {
                this.id = asset.id;
                this.displayName = asset.name;

                this.asset = asset;
                if (asset is BehaviorSet) {
                    this.itemType = ItemType.BehaviorSet;
                }
                else if (asset is BehaviorDefinition) {
                    this.itemType = ItemType.BehaviorDefinition;
                }
                else if (asset is ActionDefinition) {
                    this.itemType = ItemType.ActionDefinition;
                }
                else {
                    throw new ArgumentException("Asset must be non null and a known type");
                }
            }

            public AITreeItem ParentAsAITreeItem => parent as AITreeItem;

            public bool IsBehaviorSet => itemType == ItemType.BehaviorSet;
            public bool IsBehaviorDefinition => itemType == ItemType.BehaviorDefinition;
            public bool IsActionDefintion => itemType == ItemType.ActionDefinition;
            public bool IsRoot => itemType == ItemType.Root;

            public bool CanDropOn(AITreeItem droppedOn) {
                switch (itemType) {
                    case ItemType.ActionDefinition:
                        return !droppedOn.IsRoot;
                    case ItemType.BehaviorSet:
                        return droppedOn.IsRoot;
                    case ItemType.BehaviorDefinition:
                        return droppedOn.IsBehaviorSet || droppedOn.IsBehaviorDefinition;
                }
                return false;
            }

            public BehaviorSet GetBehaviorSet() {
                return IsBehaviorSet ? asset as BehaviorSet : ParentAsAITreeItem?.GetBehaviorSet();
            }

            public BehaviorDefinition GetBehaviorDefinition() {
                switch (itemType) {
                    case ItemType.BehaviorSet:
                        return ((BehaviorSet) asset).GetDefaultBehavior();
                    case ItemType.BehaviorDefinition:
                        return asset as BehaviorDefinition;
                    case ItemType.ActionDefinition:
                        return ParentAsAITreeItem.GetBehaviorDefinition();
                }
                return null;
            }

            public ActionDefinition GetActionDefinition() {
                return itemType == ItemType.ActionDefinition ? asset as ActionDefinition : null;
            }

        }

    }

}