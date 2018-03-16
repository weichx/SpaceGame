using System;
using System.Collections.Generic;
using SpaceGame.Editor.GUIComponents;
using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor {

    public static class InfamyGUI {

        private static GUIContent staticLabel;

        public static GUIContent GetLabel(string text, Texture2D image = null, string toolTip = null) {
            staticLabel = staticLabel ?? new GUIContent();
            staticLabel.text = text;
            staticLabel.image = image;
            staticLabel.tooltip = toolTip;
            return staticLabel;
        }

        public static void HorizontalSplitPane(HorizontalPaneState paneState, Action left, Action right) {
            EditorGUILayoutHorizontalPanes.Begin(paneState);
            left();
            EditorGUILayoutHorizontalPanes.Splitter();
            right();
            EditorGUILayoutHorizontalPanes.End();
        }

        public static void VerticalSplitPane(VerticalPaneState paneState, Action top, Action bottom) {
            EditorGUILayoutVerticalPanes.Begin();
            top();
            EditorGUILayoutVerticalPanes.Splitter();
            bottom();
            EditorGUILayoutVerticalPanes.End();
        }

        public static T ItemPopup<T>(T selection, IReadOnlyList<T> items, string[] keys) {
            if (items.Count == 0) return default(T);
            Debug.Assert(items.Count == keys.Length, "items.Count == keys.Length");
            int idx = 0;
            for (int i = 0; i < items.Count; i++) {
                if (items[i].Equals(selection)) {
                    idx = i;
                    break;
                }
            }
            int newIdx = EditorGUILayout.Popup(idx, keys, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            return items[newIdx];
        }

        public static void SelectableLabel<T>(string text, bool isSelected, T item, Action<T> onClick) {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            string styleName = isSelected ? "SelectableLabel" : "label";
            GUIStyle style = GUI.skin.FindStyle(styleName);
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.label);
            EditorGUI.LabelField(rect, GetLabel(text), style);
            if (Event.current.GetTypeForControl(controlID) == EventType.MouseDown) {
                if (rect.Contains(Event.current.mousePosition)) {
                    onClick(item);
                    Event.current.Use();
                }
            }
        }

        public static void Button(string label, Action onClick, GUIStyle style = null) {
            if (style != null && GUILayout.Button(label, style)) {
                    onClick?.Invoke();
            }
            else if (GUILayout.Button(label)) {
                onClick?.Invoke();
            }
        }

        public static void Button<T>(string label, T data, Action<T> onClick) {
            if (GUILayout.Button(label)) {
                onClick?.Invoke(data);
            }
        }

    }

}