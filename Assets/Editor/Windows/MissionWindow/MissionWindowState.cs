using UnityEditor;
using Weichx.Persistence;

namespace SpaceGame.Editor.MissionWindow {

    public class MissionWindowState {

        private const string EditorPrefKey = "MissionEditorState";
                
        public int currentPageIndex;
        
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