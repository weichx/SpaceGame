using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class TransformInspector : UnityEditor.Editor {

    /// <inheritdoc />
    /// <summary>
    /// Draw the inspector widget.
    /// </summary>
    public override void OnInspectorGUI() {
        Transform trans = target as Transform;
        if (trans == null) return;

        EditorGUIUtility.labelWidth = 15f;
        Vector3 pos;
        Vector3 rot;
        Vector3 scale;

        // Position
        EditorGUILayout.BeginHorizontal();
        {
            if (DrawButton("P", "Reset Position", IsResetPositionValid(trans), 20f)) {
                Undo.RegisterCompleteObjectUndo(trans, "Reset Position");
                trans.localPosition = Vector3.zero;
            }

            pos = DrawVector3(trans.localPosition);
        }
        EditorGUILayout.EndHorizontal();

        // Rotation
        EditorGUILayout.BeginHorizontal();
        {
            if (DrawButton("R", "Reset Rotation", IsResetRotationValid(trans), 20f)) {
                Undo.RegisterCompleteObjectUndo(trans, "Reset Rotation");
                trans.localEulerAngles = Vector3.zero;
            }

            rot = DrawVector3(trans.localEulerAngles);
        }
        EditorGUILayout.EndHorizontal();

        // Scale
        EditorGUILayout.BeginHorizontal();
        {
            if (DrawButton("S", "Reset Scale", IsResetScaleValid(trans), 20f)) {
                Undo.RegisterCompleteObjectUndo(trans, "Reset Scale");
                trans.localScale = Vector3.one;
            }

            scale = DrawVector3(trans.localScale);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        int hiddenCount = CountHiddenChildren(trans);
        bool flagsChanged = false;
        bool shouldShow = false;

        if (DrawButton("Hide Children", "Hide Children", trans.childCount > 0)) {
            Undo.RegisterCompleteObjectUndo(trans, "Hide Children");
            HideChildrenInHeirarchy(trans);
            flagsChanged = true;
        }

        string text = "Show Children";
        if (hiddenCount > 0) text += " (" + hiddenCount + ")";

        if (DrawButton(text, "Show Children", CountHiddenChildren(trans) > 0)) {
            Undo.RegisterCompleteObjectUndo(trans, "Show Children");
            ShowChildrenInHeirarchy(trans);
            flagsChanged = true;
            shouldShow = true;
        }

        EditorGUILayout.EndHorizontal();

        // If something changes, set the transform values
        if (GUI.changed) {
            Undo.RegisterCompleteObjectUndo(trans, "Transform Change");
            trans.localPosition = Validate(pos);
            trans.localEulerAngles = Validate(rot);
            trans.localScale = Validate(scale);
        }

        if (flagsChanged) {
            EditorApplication.delayCall += () => {
                Collapse(trans.gameObject, shouldShow);
            };
            
        }
    }

    // re-paints the hierarchy
    private static void Collapse(GameObject go, bool collapse) {
        // bail out immediately if the go doesn't have children
        if (go.transform.childCount == 0) return;
        // get a reference to the hierarchy window
        EditorWindow hierarchy = GetFocusedWindow("Hierarchy");
        Selection.activeObject = go;
        // create a new key event (RightArrow for collapsing, LeftArrow for folding)
        Event key = new Event {keyCode = collapse ? KeyCode.RightArrow : KeyCode.LeftArrow, type = EventType.KeyDown};
        hierarchy.SendEvent(key);
    }

    private static EditorWindow GetFocusedWindow(string window) {
        FocusOnWindow(window);
        return EditorWindow.focusedWindow;
    }

    private static void FocusOnWindow(string window) {
        EditorApplication.ExecuteMenuItem("Window/" + window);
    }

    private static int CountHiddenChildren(Transform transform) {
        int childCount = transform.childCount;
        int count = 0;

        for (int i = 0; i < childCount; i++) {
            Transform child = transform.GetChild(i);
            bool hidden = (child.hideFlags & HideFlags.HideInHierarchy) != 0;
            if (hidden) count++;
        }

        return count;
    }

    private static void HideChildrenInHeirarchy(Transform transform) {
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++) {
            Transform child = transform.GetChild(i);
            child.hideFlags = child.hideFlags | HideFlags.HideInHierarchy;
        }
    }

    private static void ShowChildrenInHeirarchy(Transform transform) {
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++) {
            Transform child = transform.GetChild(i);
            child.hideFlags = child.hideFlags & (~HideFlags.HideInHierarchy);
        }
    }

    /// <summary>
    /// Helper function that draws a button in an enabled or disabled state.
    /// </summary>
    private static bool DrawButton(string title, string tooltip, bool enabled, float width = -1) {
        if (enabled) {
            // Draw a regular button
            return width > 0 ? GUILayout.Button(new GUIContent(title, tooltip), GUILayout.Width(width)) : GUILayout.Button(new GUIContent(title, tooltip));
        }
        else {
            // Button should be disabled -- draw it darkened and ignore its return value
            Color color = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.25f);

            if (width > 0) {
                GUILayout.Button(new GUIContent(title, tooltip), GUILayout.Width(width));
            }
            else {
                GUILayout.Button(new GUIContent(title, tooltip));
            }

            GUI.color = color;
            return false;
        }
    }

    /// <summary>
    /// Helper function that draws a field of 3 floats.
    /// </summary>
    private static Vector3 DrawVector3(Vector3 value) {
        GUILayoutOption opt = GUILayout.MinWidth(30f);
        value.x = EditorGUILayout.FloatField("X", value.x, opt);
        value.y = EditorGUILayout.FloatField("Y", value.y, opt);
        value.z = EditorGUILayout.FloatField("Z", value.z, opt);
        return value;
    }

    /// <summary>
    /// Helper function that determines whether its worth it to show the reset position button.
    /// </summary>
    private static bool IsResetPositionValid(Transform targetTransform) {
        Vector3 v = targetTransform.localPosition;
        return (v.x != 0f || v.y != 0f || v.z != 0f);
    }

    /// <summary>
    /// Helper function that determines whether its worth it to show the reset rotation button.
    /// </summary>
    private static bool IsResetRotationValid(Transform targetTransform) {
        Vector3 v = targetTransform.localEulerAngles;
        return (v.x != 0f || v.y != 0f || v.z != 0f);
    }

    /// <summary>
    /// Helper function that determines whether its worth it to show the reset scale button.
    /// </summary>
    private static bool IsResetScaleValid(Transform targetTransform) {
        Vector3 v = targetTransform.localScale;
        return (v.x != 1f || v.y != 1f || v.z != 1f);
    }

    /// <summary>
    /// Helper function that removes not-a-number values from the vector.
    /// </summary>
    private static Vector3 Validate(Vector3 vector) {
        vector.x = float.IsNaN(vector.x) ? 0f : vector.x;
        vector.y = float.IsNaN(vector.y) ? 0f : vector.y;
        vector.z = float.IsNaN(vector.z) ? 0f : vector.z;
        return vector;
    }

}