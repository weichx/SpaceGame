﻿using System;
using System.Collections.Generic;
using SpaceGame.AI;
using UnityEditor.IMGUI.Controls;
using Weichx.EditorReflection;
using Weichx.Util;

namespace SpaceGame.Editor.MissionWindow {

    public class AITreeView : TreeView {

        private ReflectedListProperty decisions;
        private Action<IList<int>> onSelection;
        
        public AITreeView(ReflectedListProperty decisions, Action<IList<int>> onSelection) : base(new TreeViewState()) {
            this.decisions = decisions;
            this.onSelection = onSelection;    
            Reload();
        }

        protected override TreeViewItem BuildRoot() {
            return new TreeViewItem {id = 0, depth = -1};
        }
        
        protected static TreeViewItem CreateTreeViewItem(ReflectedProperty entity) {
            Decision e = entity.GetValue<Decision>();
            return new TreeViewItem(e.id, 0, entity["name"].stringValue);
        }

        public void UpdateDisplayName(int id, string name) {
            TreeViewItem item = rootItem.children.Find(id, (child, checkId) => child.id == checkId);
            if (item != null) item.displayName = name;
        }
        
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root) {
            var rows = GetRows() ?? new List<TreeViewItem>(8);
            rows.Clear();
            int childCount = decisions.ChildCount;
            for (int i = 0; i < childCount; i++) {
                TreeViewItem item = CreateTreeViewItem(decisions[i]);
                rows.Add(item);
                root.AddChild(item);
            }

            SetupDepthsFromParentsAndChildren(root);

            return rows;
        }

        protected override void SelectionChanged (IList<int> selectedIds) {
            onSelection(selectedIds);
        }
    }

}