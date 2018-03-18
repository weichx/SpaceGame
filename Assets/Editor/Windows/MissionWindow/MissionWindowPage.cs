using System.Collections.Generic;
using SpaceGame.FileTypes;
using UnityEditor;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public abstract class MissionWindowPage {

        protected MissionWindowState state;
        protected ReflectedProperty selection;
        protected ReflectedListProperty list;
        protected BaseTreeView treeView;
        protected IList<int> selectedIds;
        protected GameDataFile gameData;

        protected MissionWindowPage(MissionWindowState state, GameDataFile gameData) {
            this.state = state;
            this.gameData = gameData;
            this.selectedIds = new List<int>(4);
        }

        public abstract void OnGUI();

        public virtual void OnEnable() { }
        public virtual void OnDisable() { }

        protected void DeleteSelected() {
            if (selection == null) return;
            string name = selection["name"].stringValue;
            if (EditorUtility.DisplayDialog("Are you sure?", $"Really delete {name}?", "Yup", "Nope")) {
                list.RemoveElement(selection);
                selection = null;
                treeView.SetSelection(new List<int>());
                treeView.Reload();
            }
        }

    }

}