using System;
using System.Collections.Generic;
using SpaceGame.Assets;
using Src.Game.Assets;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

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
        public AITreeView(TreeViewState state) : base(state) { }

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

        protected override bool CanStartDrag(CanStartDragArgs args) {
            return args.draggedItemIDs.Count == 1;
        }

        protected override void ContextClicked() {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Create Behavior Set"), false, () => {
                createBehaviorSet?.Invoke();
            });

            Event.current.Use();
            menu.ShowAsContext();
        }

        protected override void ContextClickedItem(int id) {
            GenericMenu menu = new GenericMenu();
            AITreeItem item = (AITreeItem) FindItem(id, rootItem);

            menu.AddItem(new GUIContent("Create Behavior Set"), false, () => {
                createBehaviorSet?.Invoke();
            });
            menu.AddItem(new GUIContent("Create Behavior Definition"), false, () => {
                createBehaviorDefintion?.Invoke(item.GetBehaviorSet());
            });
            menu.AddItem(new GUIContent("Create Action Definition"), false, () => {
                createActionDefintion?.Invoke(item.GetBehaviorDefinition());
            });

            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent($"Delete {item.displayName}"), false, () => {
                deleteAsset?.Invoke(item.asset);
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
                if (IsBehaviorSet) return asset as BehaviorSet;
                return ParentAsAITreeItem?.GetBehaviorSet();
            }

            public BehaviorDefinition GetBehaviorDefinition() {
                switch (itemType) {
                    case ItemType.BehaviorSet:
                        return ((BehaviorSet) asset).GetDefaultBehavior();
                    case ItemType.BehaviorDefinition:
                        return asset as BehaviorDefinition;
                }
                if (itemType != ItemType.ActionDefinition) return null;
                return ParentAsAITreeItem?.GetBehaviorDefinition();
            }

            public ActionDefinition ActionDefinition() {
                if (itemType != ItemType.ActionDefinition) return null;
                return asset as ActionDefinition;
            }

        }

    }

}