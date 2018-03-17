using SpaceGame.AI;
using SpaceGame.Editor.GUIComponents;
using UnityEditor;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public class AIPage : MissionPage {

        private Decision decision;
        private ReflectedObject decisionRef;

        public AIPage(MissionWindowState state) : base(state) {
            decision = new Decision();
            decisionRef = new ReflectedObject(decision);
        }

        public override void OnGUI() {
            EditorGUILayoutX.DrawProperties(decisionRef);
        }

        public override void OnEnable() { }

        public override void OnDisable() { }

    }

}