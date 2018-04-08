using System;
using System.Collections.Generic;
using SpaceGame.Assets;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Weichx.Util;

namespace SpaceGame.Editor.MissionWindow {

    public class AITreeView : TreeBase {

        public delegate void SelectionChangedCallback(GameAsset selection);
        
        public event SelectionChangedCallback selectionChanged;

        private GameDatabase db;

        public AITreeView(GameDatabase db, TreeViewState state) : base(state) {
            this.db = db;
        }

        public void SetDataRebuildAndSelect(int selectionId = -1) {
            Reload();
            if (selectionId != -1) {
                SelectFireAndFrame(selectionId);
            }
        }

        protected override TreeViewItem BuildRoot() {
            AITreeItem root = new AITreeItem();

            IReadonlyListX<BehaviorSet> behaviorSets = db.GetAssetList<BehaviorSet>();
            
            foreach (BehaviorSet behaviorSet in behaviorSets) {

                AITreeItem behaviorSetItem = new AITreeItem(behaviorSet);
                root.AddChild(behaviorSetItem);

//                foreach (BehaviorDefinition behaviorDefinition in behaviorSet.behaviors) {
//
//                    AITreeItem behaviorItem = new AITreeItem(behaviorDefinition);
//                    behaviorSetItem.AddChild(behaviorItem);
//
//                    foreach (ActionDefinition action in behaviorDefinition.actions) {
//                        behaviorItem.AddChild(new AITreeItem(action));
//                    }
//
//                }
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

//        private void OnActionDrop(ActionDefinition action, GameAsset droppedOn, int index) {
//
//            if (droppedOn is BehaviorSet) {
//                BehaviorSet behaviorSet = (BehaviorSet) droppedOn;
//                behaviorSet.GetDefaultBehavior().AddActionDefinition(action, index);
//            }
////            else if (droppedOn is BehaviorDefinition) {
////                ((BehaviorDefinition) droppedOn).AddActionDefinition(action, index);
////            }
////            else if (droppedOn is ActionDefinition) {
////                ActionDefinition droppedOnAction = (ActionDefinition) droppedOn;
////                droppedOnAction.GetBehaviorDefinition().AddActionDefinition(action, index);
////            }
//            SetDataRebuildAndSelect(action.id);
//        }

//        private void OnBehaviorDefintionDrop(BehaviorDefinition behaviorDefinition, GameAsset droppedOn, int index) {
//            if (droppedOn is BehaviorSet) {
//                (droppedOn as BehaviorSet).AddBehaviorDefinition(behaviorDefinition, index);
//            }
//            else if (droppedOn is BehaviorDefinition) {
//                BehaviorDefinition bh = droppedOn as BehaviorDefinition;
//                bh.GetBehaviorSet().AddBehaviorDefinition(behaviorDefinition);
//            }
//            SetDataRebuildAndSelect(behaviorDefinition.id);
//        }

        private void OnBehaviorSetDrop(BehaviorSet behaviorSet, int index) {
           // db.MoveToIndex(behaviorSet, index);
            SetDataRebuildAndSelect(behaviorSet.id);
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
                SetDataRebuildAndSelect(db.CreateAsset<BehaviorSet>().id);
            });

            Event.current.Use();
            menu.ShowAsContext();
        }

        protected override void ContextClickedItem(int id) {
            GenericMenu menu = new GenericMenu();
            AITreeItem item = (AITreeItem) FindItem(id, rootItem);

            menu.AddItem(new GUIContent("Create Behavior Set"), false, () => {
                SetDataRebuildAndSelect(db.CreateAsset<BehaviorSet>().id);
            });
//            menu.AddItem(new GUIContent("Create Behavior Definition"), false, () => {
//                BehaviorDefinition behaviorDefinition = db.CreateAsset<BehaviorDefinition>();
//                item.GetBehaviorSet().AddBehaviorDefinition(behaviorDefinition);
//                SetDataRebuildAndSelect(behaviorDefinition.id);
//            });
//            menu.AddItem(new GUIContent("Create Action Definition"), false, () => {
//                BehaviorDefinition behaviorDefinition = item.GetBehaviorDefinition();
//                ActionDefinition action = db.CreateAsset<ActionDefinition>();
//                behaviorDefinition.AddActionDefinition(action);
//                SetDataRebuildAndSelect(action.id);
//            });

            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent($"Delete {item.displayName}"), false, () => {
                db.DestroyAsset(item.asset);
                SetDataRebuildAndSelect();
            });
            Event.current.Use();
            menu.ShowAsContext();
        }

        private enum ItemType {

            Root,
            BehaviorSet

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
                else {
                    throw new ArgumentException("Asset must be non null and a known type");
                }
            }

            public AITreeItem ParentAsAITreeItem => parent as AITreeItem;

            public bool IsBehaviorSet => itemType == ItemType.BehaviorSet;
            public bool IsRoot => itemType == ItemType.Root;

            public bool CanDropOn(AITreeItem droppedOn) {
                switch (itemType) {
                   
                    case ItemType.BehaviorSet:
                        return droppedOn.IsRoot;
                   
                }
                return false;
            }

            public BehaviorSet GetBehaviorSet() {
                return IsBehaviorSet ? asset as BehaviorSet : ParentAsAITreeItem?.GetBehaviorSet();
            }

           
        }

    }

}