using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public abstract class BaseTreeView : TreeView {

        protected ReflectedListProperty list;
        protected Action<IList<int>> onSelection;

        protected BaseTreeView(ReflectedListProperty list, Action<IList<int>> onSelection)
            : base(new TreeViewState()) {
            this.list = list;
            this.onSelection = onSelection;
            Reload();
        }

        public void OnGUILayout() {
            OnGUI(GUILayoutUtility.GetRect(0, 10000, 0, 10000));
        }

        protected override TreeViewItem BuildRoot() {
            return new TreeViewItem {id = 0, depth = -1};
        }

        protected static TreeViewItem CreateTreeViewItem(ReflectedProperty shipDef) {
            IIdentitifiable idable = shipDef.GetValue<IIdentitifiable>();
            return new TreeViewItem(idable.Id, 0, idable.Name ?? string.Empty);
        }

        public void UpdateDisplayName(int id, string name) {
            TreeViewItem item = FindItem(id, rootItem);
            if (item != null) item.displayName = name ?? string.Empty;
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root) {
            var rows = GetRows() ?? new List<TreeViewItem>(8);
            rows.Clear();
            int childCount = list.ChildCount;
            for (int i = 0; i < childCount; i++) {
                TreeViewItem item = CreateTreeViewItem(list[i]);
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