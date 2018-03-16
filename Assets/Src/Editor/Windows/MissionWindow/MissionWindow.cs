using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor.MissionWindow {

    public class MissionWindow : EditorWindow {

        private GUISkin skin;
        private MissionWindowState state;

        private MissionWindowPage[] pages;

        private readonly string[] tabs = {
            "Missions",
            "Entities",
            "Ships"
        };

        private void OnEnable() {
            state = MissionWindowState.Restore("MissionWindow");
            pages = new MissionWindowPage[] {
                new MissionPage(state) 
//                new EntityPage(state)
            };
            skin = EditorGUIUtility.Load("MissionWindowSkin.asset") as GUISkin;
            pages[state.currentPageIndex].OnEnable();
        }

        private void OnDisable() {
            state.Save();
            pages[state.currentPageIndex].OnDisable();
        }

        private void OnInspectorUpdate() {
            Repaint();
        }

        public void OnGUI() {
            GUI.skin = skin;
            if (state == null) return;

            if (pages == null) return;

            int lastPage = state.currentPageIndex;
            state.currentPageIndex = GUILayout.Toolbar(lastPage, tabs);

            if (state.currentMission == null) {
                state.currentPageIndex = 0;
            }

            if (lastPage != state.currentPageIndex) {
                pages[lastPage].OnDisable();
                pages[state.currentPageIndex].OnEnable();
            }

            pages[state.currentPageIndex].OnGUI();
        }

    }

}

/*

Ship Details Page
    - Define Ship Stats
    - Define Load out options
    - Define Chassis
Entity Page
    - Faction
    - Ship Type
    - Call Sign
    - Active in Mission
    - Cargo
    - Starting Stats / Overrides
    - Flight Group
    - AI Packages
    - Goals
AI Page
    - Build AI Behaviors
    - Apply Considerations
    
Weapons Page
    -  

*/