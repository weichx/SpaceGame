using System;
using UnityEditor;
using UnityEngine;

namespace Src.Editor {

    public static class EditorMenuItems {


        [MenuItem("GameObject/Reset Scene Camera &r")]
        public static void CenterCamera() {
            try {
                SceneView.lastActiveSceneView.pivot = new Vector3(0, 500, 0);
//                SceneView.lastActiveSceneView.LookAt(Vector3.zero);
                SceneView.lastActiveSceneView.rotation = Quaternion.LookRotation(Vector3.down);
//                SceneView.lastActiveSceneView.s
            } catch(Exception e) {}
        }

    }

}