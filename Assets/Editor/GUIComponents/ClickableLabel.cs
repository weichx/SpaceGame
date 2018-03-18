using System;
using UnityEditor;
using UnityEngine;

namespace SpaceGame.EditorComponents {

    public class ClickableLabel<T> {

        public T data;
        public Action<T> onClick;
        public GUIContent content;

        public ClickableLabel(string labelText, T data, Action<T> callback) {
            this.content = new GUIContent(labelText);
            this.data = data;
            this.onClick = callback;
            content.image = EditorGUIUtility.Load("xwing_icon.png") as Texture2D;
        }

        public void OnGUI(Rect rect, GUIStyle style) {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Rect toggleRect = new Rect(rect);
            toggleRect.width = 16;
            rect.x += 16;
            rect.width -= 16;
            EditorGUI.Toggle(toggleRect, GUIContent.none, true);
            EditorGUI.LabelField(rect, content, style);
            EventType evt = Event.current.GetTypeForControl(controlID);
            switch (evt) {
                case EventType.MouseDown:
                    if (rect.Contains(Event.current.mousePosition)) {
                        onClick(data);
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    DragAndDrop.PrepareStartDrag();

                    DragAndDrop.SetGenericData("data", "more data");

                    DragAndDrop.StartDrag("Dragging title");

                    Event.current.Use();
                    break;
                case EventType.DragUpdated:
                    
                    break;
            }

        }

        public void OnGUILayout(bool selected) {
            string styleName = selected ? "ClickableLabel" : "label";
            GUIStyle style = GUI.skin.FindStyle(styleName);
            if (style == null) {
                Debug.Log("nope");
                return;
            }
            OnGUI(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.label), style);
        }

    }

}