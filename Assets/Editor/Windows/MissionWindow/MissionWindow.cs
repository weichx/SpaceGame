using SpaceGame.FileTypes;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGame.Editor.MissionWindow {

    public class MissionWindow : EditorWindow {

        private MissionWindowState state;
        private GameDataFile gameData;
        private MissionWindowPage[] pages;

        private readonly string[] tabs = {
            "Missions",
            "AI",
            "Ships"
        };

        private void OnEnable() {
            gameData = Resources.Load<GameDataFile>("Game Data");
            state = MissionWindowState.Restore();
            pages = new MissionWindowPage[] {
                new MissionPage(state, gameData),
                new AIPage(state, gameData),
                new ShipPage(state, gameData),
            };
            pages[state.currentPageIndex].OnEnable();
        }

        /*
            hydrate scene from mission file
            add entities to scene by normal unity methods ie drop / duplicate / new GameObject()
            entity definition -> template or instance?
            positioning done in editor
            can spawn entities that didnt start in the scene

            2 structures: Template -> Defines things like ship type, capabilities, basic generic behaviors
                          Instance -> Has a name, can override anything in the template
                          If no instance is explicitly set on an entity, then it can create a blank one from its template
                          All entities require an instance, instances need to be built off of a template. Use generic template if none is specified          
        */

        private void OnDisable() {
            pages[state.currentPageIndex].OnDisable();
            gameData.Save();
            state.Save();
        }

        private void OnInspectorUpdate() {
            Repaint();
        }

        public void OnGUI() {
            if (state == null) return;            

            if (pages == null) return;

            int lastPage = state.currentPageIndex;

            state.currentPageIndex = GUILayout.Toolbar(lastPage, tabs, (GUILayoutOption[]) null);

            if (lastPage != state.currentPageIndex) {
                pages[lastPage].OnDisable();
                gameData.Save();
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