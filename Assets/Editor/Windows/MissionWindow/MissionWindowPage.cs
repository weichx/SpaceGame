using System.Collections.Generic;
using SpaceGame.FileTypes;
using UnityEditor;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public abstract class MissionWindowPage {

        protected MissionWindowState state;
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

    }

}