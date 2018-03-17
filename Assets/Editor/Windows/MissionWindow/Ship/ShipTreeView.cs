using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using Weichx.EditorReflection;
using Weichx.Util;

namespace SpaceGame.Editor.MissionWindow {

    public class ShipTreeView : TreeView {

        private ReflectedListProperty shipList;
        private Action<IList<int>> onSelection;

        public ShipTreeView(ReflectedListProperty shipList, Action<IList<int>> onSelection) 
            : base(new TreeViewState()) {
            this.shipList = shipList;
            this.onSelection = onSelection;
        }

        protected override TreeViewItem BuildRoot() {
            return new TreeViewItem {id = 0, depth = -1};
        }

        protected static TreeViewItem CreateTreeViewItem(ReflectedProperty shipDef) {
            ShipDefinition ship = shipDef.GetValue<ShipDefinition>();
            return new TreeViewItem(ship.id, 0, shipDef[nameof(ShipDefinition.name)].stringValue);
        }

        public void UpdateDisplayName(int id, string name) {
            TreeViewItem item = rootItem.children.Find(id, (child, checkId) => child.id == checkId);
            if (item != null) item.displayName = name;
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root) {
            var rows = GetRows() ?? new List<TreeViewItem>(8);
            rows.Clear();
            int childCount = shipList.ChildCount;
            for (int i = 0; i < childCount; i++) {
                TreeViewItem item = CreateTreeViewItem(shipList[i]);
                rows.Add(item);
                root.AddChild(item);
            }

            SetupDepthsFromParentsAndChildren(root);

            return rows;
        }

        protected override void SelectionChanged(IList<int> selectedIds) {
            onSelection(selectedIds);
        }

    }

}