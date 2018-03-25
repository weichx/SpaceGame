using System.Collections.Generic;
using SpaceGame.FileTypes;
using UnityEditor;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public abstract class MissionWindowPage {

        protected MissionWindowState state;
        protected GameDatabase db;

        protected MissionWindowPage(MissionWindowState state, GameDatabase db) {
            this.state = state;
            this.db = db;
        }

        public abstract void OnGUI();

        public virtual void OnEnable() { }
        public virtual void OnDisable() { }

    }

}