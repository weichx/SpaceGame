using System;
using System.Reflection;
using Weichx.Util;
using Src.Editor;
using UnityEngine;

namespace SpaceGame.Editor {

    using UnityEditor;

    [CustomEditor(typeof(WaypointPath))]
    public class WaypointPathInspector : Editor {

        private static readonly string[] colorNames = {
            "-- None --", "Gray", "Blue", "Teal", "Green", "Yellow", "Orange", "Red", "Purple"
        };

        private static readonly int[] colorValues = { -1, 0, 1, 2, 3, 4, 5, 6, 7};
        
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            WaypointPath waypointPath = target as WaypointPath;
            if (waypointPath == null) return;

            Transform transform = waypointPath.transform;
            Transform[] children = transform.GetChildren();

            int tagColorIdx = Mathf.Clamp(waypointPath.tagColorIndex, -1, 7);
            
            int newSelection = EditorGUILayout.IntPopup("Set Tag Color", tagColorIdx, colorNames, colorValues);

            if (tagColorIdx != newSelection) {
                waypointPath.tagColorIndex = newSelection;
                children.ForEach((child) => IconManager.SetIcon(child.gameObject, (IconManager.LabelIcon) newSelection));
            }

            EditorGUILayout.BeginHorizontal();
            if (tagColorIdx == -1 && GUILayout.Button("Show Tags")) {
                children.ForEach((child) => IconManager.SetIcon(child.gameObject, (IconManager.LabelIcon) tagColorIdx));
            }
            else if (tagColorIdx != -1 && GUILayout.Button("Hide Tags")) {
                children.ForEach((child) =>IconManager.SetIcon(child.gameObject, IconManager.LabelIcon.None));
                waypointPath.tagColorIndex = -1;
            }

            if (GUILayout.Button("Name Children")) {
                Array.Sort(children, (a, b) => {
                    float aDist = a.DistanceToSquared(transform.position);
                    float bDist = b.DistanceToSquared(transform.position);
                    return aDist < bDist ? -1 : 1;
                });
                
                children.ForEachIndex((child, index) => {
                    child.SetSiblingIndex(index);
                    child.name = transform.name + "[" + index + "]";
                });
            }
            
            EditorGUILayout.EndHorizontal();
            
        }

//        private void DrawIcon(GameObject gameObject, int idx) {
//            GUIContent[] largeIcons = GetTextures("sv_label_", string.Empty, 0, 8);
//            GUIContent icon = idx >= 0 && idx < 8 ? largeIcons[idx] : "sv_icon_";
//            Type egu = typeof(EditorGUIUtility);
//            BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
//            object[] args = {gameObject, icon.image};
//            MethodInfo setIcon = egu.GetMethod("SetIconForObject", flags, null, new[] {typeof(UnityEngine.Object), typeof(Texture2D)}, null);
//            if (setIcon != null) setIcon.Invoke(null, args);
//        }
//
//        private GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count) {
//            GUIContent[] array = new GUIContent[count];
//            for (int i = 0; i < count; i++) {
//                array[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postFix);
//            }
//
//            return array;
//        }

    }

}