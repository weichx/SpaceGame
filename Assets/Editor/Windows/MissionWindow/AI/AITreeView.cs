using System.Collections.Generic;
using SpaceGame.AI;
using UnityEditor.IMGUI.Controls;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public class AITreeView : TreeView {

        private ReflectedListProperty decisions;

        public AITreeView(ReflectedListProperty decisions, TreeViewState state) : base(state) {
            this.decisions = decisions;
            Reload();
        }

        protected override TreeViewItem BuildRoot() {
            return new TreeViewItem {id = 0, depth = -1};
        }
        
        protected static TreeViewItem CreateTreeViewItem(ReflectedProperty entity) {
            Decision e = entity.GetValue<Decision>();
            return new TreeViewItem(e.GetHashCode(), 0, e.name);
        }
        
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root) {
            var rows = GetRows() ?? new List<TreeViewItem>(128);

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

    }

}