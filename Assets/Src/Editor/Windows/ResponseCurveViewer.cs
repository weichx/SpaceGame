using System.Collections.Generic;
using System.Reflection;
using SpaceGame.AI;
using SpaceGame.Editor.GUIComponents;
using SpaceGame.Util.Texture2DExtensions;
using UnityEngine;
using UnityEditor;

namespace SpaceGame.Editor.Windows {


    public class ResponseCurveViewer : EditorWindow {
        
        private Texture2D graphTexture;
        private ResponseCurve curve;
        
        private void OnGUI() {
            RenderCurve();
        }
        
        private void RenderCurve() {
            bool updateTexture;
            
            if (graphTexture == null) {
                graphTexture = new Texture2D(1, 1, TextureFormat.RGBA32, true);
            }

            if (curve == null) {
                curve = new ResponseCurve();
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.EndHorizontal();

            DrawerUtil.PushIndentLevel(1);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                GUILayout.BeginVertical(GUILayout.MaxWidth(400f));
                curve.curveType = (ResponseCurveType) EditorGUILayout.EnumPopup("Curve Type", curve.curveType);
                curve.slope = EditorGUILayout.FloatField("Slope", curve.slope);
                curve.exp = EditorGUILayout.FloatField("Exp", curve.exp);
                curve.vShift = EditorGUILayout.FloatField("Vertical Shift", curve.vShift);
                curve.hShift = EditorGUILayout.FloatField("Horizontal Shift", curve.hShift);
                curve.threshold = EditorGUILayout.FloatField("Threshold", curve.threshold);
                curve.invert = EditorGUILayout.Toggle(new GUIContent("Inverted"), curve.invert);
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUIUtility.labelWidth);
                if (GUILayout.Button("Reset")) {
                    curve.Reset();
                }

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                updateTexture = EditorGUI.EndChangeCheck();
            }
            //draw the graph
            {
                if (updateTexture || graphTexture.width != 512) {
                    graphTexture.Resize(512, (int) (8.75f * EditorGUIUtility.singleLineHeight));
                    Rect rect = new Rect() {
                        x = 0,
                        y = 0,
                        width = graphTexture.width,
                        height = graphTexture.height
                    };
                    GraphHelper.DrawGraphLines(rect, graphTexture, (float input) => {
                        return curve.Evaluate(input);
                    });
                    graphTexture.FlipVertically();
                    graphTexture.Apply(true);
                }

                DrawerUtil.DrawLayoutTexture(graphTexture);
            }

            EditorGUILayout.EndHorizontal();
            DrawerUtil.PopIndentLevel();
        }

    }

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