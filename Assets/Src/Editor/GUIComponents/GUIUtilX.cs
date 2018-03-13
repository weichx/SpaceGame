using System;
using System.Collections.Generic;
using UnityEditor;

namespace SpaceGame.Editor.GUIComponents {

    public static class GUIUtilX {

        public static void Repeat<T>(List<T> list, Action<T> render) {
            list.ForEach(render);
        }

        
        public static void Vertical(Action action) {
            EditorGUILayout.BeginVertical();
            action();
            EditorGUILayout.EndVertical();
        }

        public static void Horizontal(Action action) {
            EditorGUILayout.BeginHorizontal();
            action();
            EditorGUILayout.EndHorizontal();
        }

    }

}