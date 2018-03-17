using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Src.Editor.GUIComponents {

    public static class DrawerUtil {

        private static Stack<float> labelWidthStack = new Stack<float>();
        private static FieldInfo rectField;

        public static void DrawLayoutTexture(Texture2D texture, float height = -1) {
//            if (rectField == null) {
//                rectField = typeof(EditorGUILayout).GetField("s_LastRect", BindingFlags.Static | BindingFlags.NonPublic);
//            }
            if (height < 0) height = texture.height;
            Rect position = EditorGUILayout.GetControlRect(false, height);
//            rectField.SetValue(null, position);
            GUI.DrawTexture(position, texture);
        }

        public static void PushLabelWidth(float width) {
            labelWidthStack.Push(EditorGUIUtility.labelWidth);
            EditorGUIUtility.labelWidth = width;
        }

        public static void PopLabelWidth() {
            EditorGUIUtility.labelWidth = labelWidthStack.Pop();
        }

        private static Stack<int> indentStack = new Stack<int>();

        public static void PushIndentLevel(int indent) {
            EditorGUI.indentLevel += indent;
            indentStack.Push(indent);
        }

        public static void PopIndentLevel() {
            EditorGUI.indentLevel -= indentStack.Pop();
        }

    }

}