namespace SpaceGame.Editor.MissionWindow {

    public abstract class MissionWindowPage {

        protected MissionWindowState state;

        protected MissionWindowPage(MissionWindowState state) {
            this.state = state;
        }
        
        public abstract void OnGUI();

        public virtual void OnEnable() { }
        public virtual void OnDisable() { }

    }

}