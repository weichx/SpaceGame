using SpaceGame.EditorComponents;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using Weichx.Persistence;

namespace SpaceGame.Editor.MissionWindow {

    public class MissionWindowState {

        private const string EditorPrefKey = "MissionEditorState";

        public TreeViewState missionPageTreeViewState;
        public TreeViewState shipPageTreeViewState;
        public TreeViewState aiPageTreeViewState;

        public HorizontalPaneState missionPageSplitterState;
        
        public int currentPageIndex;
        public HorizontalPaneState shipPageSplitterState;

        public MissionWindowState() {
            missionPageTreeViewState = new TreeViewState();
            missionPageSplitterState = new HorizontalPaneState();
            shipPageSplitterState = new HorizontalPaneState();
        }
                
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