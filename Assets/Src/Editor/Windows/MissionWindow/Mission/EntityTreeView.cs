using System;
using System.Collections.Generic;
using SpaceGame.Editor.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace SpaceGame.Editor.MissionWindow {

    // todo -- enable hierarchy when we have the concept of entity groups
    // probably Faction -> Flight Group -> Entity, possibly more levels for spawn groups or ship types
    public class EntityTreeView : TreeView {

        private const string GUID = nameof(MissionDefinition.guid);
        private const string NAME = nameof(MissionDefinition.name);

        private ReflectedListProperty entityDefinitionList;
        private Action<ReflectedProperty> onSelectionChanged;
        
        public EntityTreeView(ReflectedListProperty entityDefinitionList, TreeViewState state, Action<ReflectedProperty> onSelectionChanged) : base(state) {
            this.onSelectionChanged = onSelectionChanged;
            this.entityDefinitionList = entityDefinitionList;
            Reload();
        }

        protected override TreeViewItem BuildRoot() {
            return new TreeViewItem {id = 0, depth = -1};
        }

        protected static TreeViewItem CreateTreeViewItem(ReflectedProperty entityDefinition) {
            return new TreeViewItem(entityDefinition[GUID].stringValue.GetHashCode(), 0, entityDefinition[NAME].stringValue);
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root) {
            var rows = GetRows() ?? new List<TreeViewItem>(200);

            rows.Clear();
            int childCount = entityDefinitionList.ChildCount;
            for (int i = 0; i < childCount; i++) {
                TreeViewItem item = CreateTreeViewItem(entityDefinitionList[i]);
                rows.Add(item);
                root.AddChild(item);
            }

            SetupDepthsFromParentsAndChildren(root);

            return rows;
        }

        protected override bool CanStartDrag(CanStartDragArgs args) {
            return true;
        }

        protected override bool CanMultiSelect(TreeViewItem item) {
            return false;
        }
        
        protected override void SetupDragAndDrop(SetupDragAndDropArgs args) {
            DragAndDrop.PrepareStartDrag();

            DragAndDrop.SetGenericData("GUID", args.draggedItemIDs[0]);

            DragAndDrop.StartDrag("EntityDefinition");
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args) {
            if (args.dragAndDropPosition != DragAndDropPosition.BetweenItems) {
                return DragAndDropVisualMode.Rejected;
            }
            if (args.performDrop) {

                switch (args.dragAndDropPosition) {
                    case DragAndDropPosition.BetweenItems:
                        int insertIndex = args.insertAtIndex;
                        int hash = (int) DragAndDrop.GetGenericData("GUID");
                        int oldIndex = entityDefinitionList.FindIndex((e) => e[GUID].stringValue.GetHashCode() == hash);
                        entityDefinitionList.SetChildIndex(oldIndex, insertIndex);
                        break;
                    case DragAndDropPosition.UponItem:
                    case DragAndDropPosition.OutsideItems:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Reload();
            }
            return DragAndDropVisualMode.Move;
        }

        protected override void SelectionChanged (IList<int> selectedIds) {
            if (onSelectionChanged != null) {
                int hash = selectedIds[0];
                int index = entityDefinitionList.FindIndex((e) => e[GUID].stringValue.GetHashCode() == hash);
                onSelectionChanged(entityDefinitionList.ChildAt(index));
            }
        }
    }


}