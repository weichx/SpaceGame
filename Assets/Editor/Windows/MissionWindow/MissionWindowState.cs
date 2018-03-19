using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Weichx.Persistence;

namespace SpaceGame.Editor.MissionWindow {

    public class MissionWindowState {

        private const string EditorPrefKey = "MissionEditorState";

        public TreeViewState missionPageTreeViewState;
        public int currentPageIndex;
        public string activeMissionGuid;

        public MissionWindowState() {
            missionPageTreeViewState = new TreeViewState();
        }
        
        [SerializeField] 
        private MissionDefinition activeMission;
        
        public static MissionWindowState Restore() {
            string serializedState = EditorPrefs.GetString(EditorPrefKey);
            MissionWindowState state = Snapshot<MissionWindowState>.Deserialize(serializedState);
            return state;
        }

        public void Save() {
            EditorPrefs.SetString(EditorPrefKey, Snapshot<MissionWindowState>.Serialize(this));
        }

    }

}