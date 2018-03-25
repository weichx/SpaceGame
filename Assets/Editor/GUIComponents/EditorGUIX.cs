using UnityEngine;
using System;
using System.Collections.Generic;
using Editor.GUIComponents;
using SpaceGame.AI;
using UnityEditor;
using Weichx.EditorReflection;
using Weichx.ReflectionAttributes;

namespace SpaceGame.EditorComponents {

    public static class EditorGUIX {

        private static GUIRect s_guiRect = new GUIRect();
        private static GUIContent tempContent = new GUIContent();
        public static float singleLineHeight => EditorGUIUtility.singleLineHeight;
        public static bool wideMode => EditorGUIUtility.wideMode;
        private static Stack<float> labelWidthStack = new Stack<float>();

        public static void PushLabelWidth(float width) {
            labelWidthStack.Push(EditorGUIUtility.labelWidth);
            EditorGUIUtility.labelWidth = width;
        }

        public static void PopLabelWidth() {
            EditorGUIUtility.labelWidth = labelWidthStack.Pop();
        }
        
        public static Type TypePopup<T>(Rect position, Type currentType, GUIStyle style = null) {
            Type[] subclasses = EditorReflector.FindSubClassesWithNull(typeof(T));
            string[] subclassNames = EditorReflector.FindSubClassNamesWithNull(typeof(T));
            int currentIndex = Array.IndexOf(subclasses, currentType);
            return subclasses[EditorGUI.Popup(position, currentIndex, subclassNames, style ?? EditorStyles.popup)];
        }

        public static Type ConstructableTypePopup<T>(Rect position, Type currentType, Func<string, string> formatLabels, GUIStyle style = null) {
            Type[] subclasses = EditorReflector.FindConstructableSubClassesWithNull(typeof(T));
            string[] subclassNames = EditorReflector.FindConstructableSubClassNamesWithNull(typeof(T));
            for (int i = 0; i < subclassNames.Length; i++) {
                subclassNames[i] = formatLabels(subclassNames[i]);
            }
            int currentIndex = Array.IndexOf(subclasses, currentType);
            return subclasses[EditorGUI.Popup(position, currentIndex, subclassNames, style ?? EditorStyles.popup)];
        }

        public static Type ConstructableTypePopup<T>(Rect position, Type currentType, GUIStyle style = null) {
            Type[] subclasses = EditorReflector.FindConstructableSubClassesWithNull(typeof(T));
            string[] subclassNames = EditorReflector.FindConstructableSubClassNamesWithNull(typeof(T));
            int currentIndex = Array.IndexOf(subclasses, currentType);
            return subclasses[EditorGUI.Popup(position, currentIndex, subclassNames, style ?? EditorStyles.popup)];
        }

        public static Type TypePopup<T>(GUIRect guiRect, GUIContent label, Type currentType) {
            Rect rect = guiRect.GetFieldRect();
            rect = EditorGUI.PrefixLabel(rect, label);
            return TypePopup<T>(rect, currentType);
        }

        public static void TypePopup<T>(GUIRect guiRect, GUIContent label, ReflectedProperty property) {
            Rect rect = guiRect.GetFieldRect();
            rect = EditorGUI.PrefixLabel(rect, label);
            TypePopup<T>(rect, property);
        }

        public static Type TypePopup<T>(Rect position, GUIContent label, Type currentType) {
            Rect rect = EditorGUI.PrefixLabel(position, label);
            return TypePopup<T>(rect, currentType);
        }

        public static Type TypePopup<T>(GUIRect guiRect, Type currentType) {
            return TypePopup<T>(guiRect.GetFieldRect(), currentType);
        }

        public static Type ConstructableTypePopup<T>(GUIRect guiRect, Type currentType) {
            return ConstructableTypePopup<T>(guiRect.GetFieldRect(), currentType);
        }

        public static void TypePopup<T>(GUIRect guiRect, ReflectedProperty property) {
            Type currentType = (Type) property.Value;
            property.Value = TypePopup<T>(guiRect.GetFieldRect(), currentType);
        }

        public static void TypePopup<T>(Rect position, ReflectedProperty property) {
            Type currentType = (Type) property.Value;
            property.Value = TypePopup<T>(position, currentType);
        }

        public static void ConstructableTypePopup<T>(GUIRect guiRect, ReflectedProperty property) {
            Type currentType = (Type) property.Value;
            property.Value = ConstructableTypePopup<T>(guiRect.GetFieldRect(), currentType);
        }

        private class ResponseCurveFieldState {

            public Texture2D texture;

            public ResponseCurveFieldState() {
                this.texture = new Texture2D(1, 1);
            }

        }

        private static ResponseCurve GetPreset(string presetName, ResponseCurve input) {
            switch (presetName) {
                case "Linear":  return ResponseCurve.CreateLinearCurve();
                case "2 Poly":  return ResponseCurve.Create2PolyCurve();
                case "4 Poly":  return ResponseCurve.Create4PolyCurve();
                case "6 Poly":  return ResponseCurve.Create6PolyCurve();
                case "8 Poly":  return ResponseCurve.Create8PolyCurve();
                case "-2 Poly": return ResponseCurve.CreateInverted2PolyCurve();
                case "-4 Poly": return ResponseCurve.CreateInverted4PolyCurve();
                case "-6 Poly": return ResponseCurve.CreateInverted6PolyCurve();
                case "-8 Poly": return ResponseCurve.CreateInverted8PolyCurve();
                default:
                    return input;
            }
        }

        public static ResponseCurve ResponseCurveField(Rect position, ResponseCurve curve) {

            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            ResponseCurveFieldState state = (ResponseCurveFieldState) GUIUtility.GetStateObject(
                typeof(ResponseCurveFieldState),
                controlID);

            Rect left = new Rect(position);
            left.width *= 0.5f;
            Rect right = new Rect(position);
            right.x += left.width;
            right.width -= left.width;

            if (Event.current.type == EventType.Repaint) {
                curve.DrawGraph(state.texture, right.width, right.height);
            }

            EditorGUI.DrawPreviewTexture(right, state.texture);

            float oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 80f;

            s_guiRect.SetRect(left);
            curve.curveType = (ResponseCurveType) EditorGUI.EnumPopup(s_guiRect.GetFieldRect(), "Curve Type", curve.curveType);
            curve.slope = EditorGUI.FloatField(s_guiRect.GetFieldRect(), "Slope", curve.slope);
            curve.exp = EditorGUI.FloatField(s_guiRect.GetFieldRect(), "Exp", curve.exp);
            curve.vShift = EditorGUI.FloatField(s_guiRect.GetFieldRect(), "V Shift", curve.vShift);
            curve.hShift = EditorGUI.FloatField(s_guiRect.GetFieldRect(), "H Shift", curve.hShift);
            curve.threshold = EditorGUI.FloatField(s_guiRect.GetFieldRect(), "Threshold", curve.threshold);

            Rect lineRect = s_guiRect.GetFieldRect();

            Rect toggleRect = new Rect(lineRect) {
                width = EditorGUIUtility.labelWidth + 16f
            };

            Rect selectRect = new Rect(lineRect) {
                x = lineRect.x + toggleRect.width,
                width = lineRect.width - toggleRect.width
            };

            curve.invert = EditorGUI.Toggle(toggleRect, "Invert", curve.invert);

            int idx = EditorGUI.Popup(selectRect, 0, PresetCurveNames);
            curve = ApplyResponseCurvePreset(PresetCurveNames[idx], curve);

            EditorGUIUtility.labelWidth = oldWidth;
            return curve;
        }

        public static void DrawProperties(GUIRect guiRect, ReflectedProperty root, string[] skipList = null) {
            List<ReflectedProperty> properties = root.GetChildren();
            for (int i = 0; i < properties.Count; i++) {
                ReflectedProperty property = properties[i];
                if (!property.IsHidden && (skipList == null || Array.IndexOf(skipList, property.name) == -1)) {
                    float height = property.GetPropertyHeight();
                    Internal_PropertyField(guiRect.SliceHeight(height), property, property.GUIContent, false);
                }
            }
        }

        public static void DrawProperties(Rect position, ReflectedObject obj, string[] skipList = null) {
            DrawProperties(position, obj.Root, skipList);
        }

        public static void DrawProperties(Rect position, ReflectedProperty root, string[] skipList = null) {
            List<ReflectedProperty> properties = root.GetChildren();
            s_guiRect.SetRect(position);
            for (int i = 0; i < properties.Count; i++) {
                ReflectedProperty property = properties[i];
                if ((skipList == null || Array.IndexOf(skipList, property.name) == -1)) {
                    float height = property.GetPropertyHeight();
                    Internal_PropertyField(s_guiRect.SliceHeight(height), property, property.GUIContent, property.IsHidden);
                }
            }
        }

        public static bool Foldout(GUIRect rect, ReflectedProperty property, GUIContent content = null) {
            bool isExpanded = EditorGUI.Foldout(rect.GetFieldRect(), property.IsExpanded, content ?? property.GUIContent);
            property.IsExpanded = isExpanded;
            return isExpanded;
        }

        public static bool Foldout(Rect rect, ReflectedProperty property, GUIContent content = null) {
            bool isExpanded = EditorGUI.Foldout(rect, property.IsExpanded, content ?? property.GUIContent);
            property.IsExpanded = isExpanded;
            return isExpanded;
        }

        public static void PropertyField(GUIRect rect, ReflectedProperty property, GUIContent label = null) {
            float height = property.Drawer.GetPropertyHeight(property);
            Internal_PropertyField(rect.SliceHeight(height), property, label ?? property.GUIContent, false);
        }

        public static void PropertyField(Rect position, ReflectedProperty property, GUIContent label = null) {
            Internal_PropertyField(position, property, label ?? property.GUIContent, false);
        }

        public static void PropertyField(Rect position, ReflectedObject obj, GUIContent label = null) {
            Internal_PropertyField(position, obj.Root, label ?? obj.Root.GUIContent, false);
        }

        private static void Internal_PropertyField(Rect position, ReflectedProperty property, GUIContent label, bool isHidden) {
            if (isHidden) return;

            if (label == null) label = property.GUIContent;

            Debug.Assert(property.Drawer != null, "property.Drawer != null");

            if (!property.Drawer.IsInitialized) {
                property.Drawer.Initialize();
            }
            GUI.enabled = !property.HasAttribute<ReadOnlyAttribute>(); 
            property.Drawer.OnGUI(position, property, label);
            GUI.enabled = true;

        }

        public static GUIContent TempLabel(string text) {
            tempContent.text = text;
            return tempContent;
        }

        public static bool LabelHasContent(GUIContent label) {
            return label == null || label.text != string.Empty || label.image != null;
        }

        public static float GetChildHeights(ReflectedProperty property) {
            float height = 0;
            if (property.ChildCount > 0) {
                for (int i = 0; i < property.ChildCount; i++) {
                    ReflectedProperty child = property.ChildAt(i);
                    height += child.Drawer.GetPropertyHeight(child);
                }
            }
            return height;
        }

        private static readonly string[] PresetCurveNames = {
            "Preset",
            "Linear",
            "2 Poly",
            "4 Poly",
            "6 Poly",
            "8 Poly",
            "-2 Poly",
            "-4 Poly",
            "-6 Poly",
            "-8 Poly"
        };

        private static ResponseCurve ApplyResponseCurvePreset(string presetName, ResponseCurve input) {
            switch (presetName) {
                case "Linear":  return ResponseCurve.CreateLinearCurve();
                case "2 Poly":  return ResponseCurve.Create2PolyCurve();
                case "4 Poly":  return ResponseCurve.Create4PolyCurve();
                case "6 Poly":  return ResponseCurve.Create6PolyCurve();
                case "8 Poly":  return ResponseCurve.Create8PolyCurve();
                case "-2 Poly": return ResponseCurve.CreateInverted2PolyCurve();
                case "-4 Poly": return ResponseCurve.CreateInverted4PolyCurve();
                case "-6 Poly": return ResponseCurve.CreateInverted6PolyCurve();
                case "-8 Poly": return ResponseCurve.CreateInverted8PolyCurve();
                default:
                    return input;
            }
        }

    }

}