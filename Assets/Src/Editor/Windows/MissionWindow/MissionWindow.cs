using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor.MissionWindow {

    public class MissionWindow : EditorWindow {

        private GUISkin skin;
        private MissionWindowState state;
        private int currentPage;

        private MissionWindowPage[] pages;
        private readonly string[] tabs = {
            "Overview", 
            "Entities", 
            "Ships"
        };
        
        private void OnEnable() {
            currentPage = EditorPrefs.GetInt("MissionWindow.CurrentPage");
            state = MissionWindowState.Restore();
            pages = new MissionWindowPage[] {
                new OverviewPage(state), 
                new EntityPage(state), 
                new OverviewPage(state)
            };
            skin = EditorGUIUtility.Load("MissionWindowSkin.asset") as GUISkin;
            pages[currentPage].OnEnable();
        }

        private void OnDisable() {
            state.Save();
            Debug.Log("Saved");
            pages[currentPage].OnDisable();
        }

        private void OnInspectorUpdate() {
            Repaint();
        }

        public void OnGUI() {
            GUI.skin = skin;
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle style = skin.GetStyle("mission_window_heading");
            EditorGUILayout.LabelField(state.GetMissionName(), style);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (pages == null) return;
            
            int lastPage = currentPage;
            currentPage = GUILayout.Toolbar(lastPage, tabs);
            if (state.CurrentMission == null) currentPage = 0;
            if (lastPage != currentPage) {
                EditorPrefs.SetInt("MissionWindow.CurrentPage", currentPage);
                pages[lastPage].OnDisable();
                pages[currentPage].OnEnable();
            }
            pages[currentPage].OnGUI();
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