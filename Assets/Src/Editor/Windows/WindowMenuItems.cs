using UnityEditor;

namespace SpaceGame.Editor.Windows {

    public static class WindowMenuItems {

        [MenuItem("Window/Mission Editor")]
        private static void MissionEditor() {
            EditorWindow.GetWindow<MissionWindow.MissionWindow>("Mission Editor");
        }

        [MenuItem("Window/Curve Viewer")]
        private static void CurveViewer() {
            EditorWindow.GetWindow<ResponseCurveViewer>("Response Curve");
        }

        [MenuItem("Window/Texture From Color")]
        private static void TextureFromColor() {
            EditorWindow.GetWindow<TextureFromColorWindow>("Texture From Color");
        }

    }

}