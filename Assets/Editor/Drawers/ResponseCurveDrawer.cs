using Editor.GUIComponents;
using SpaceGame.AI;
using SpaceGame.EditorComponents;
using Src.Editor;
using Src.Editor.GUIComponents;
using UnityEditor;
using UnityEngine;
using Weichx.Util.Texture2DExtensions;

namespace SpaceGameEditor.Drawers {
    
   

    [CustomPropertyDrawer(typeof(ResponseCurve))]
    public class ResponseCurveDrawer : PropertyDrawer {

        private ResponseCurve curve;
        private Texture2D graphTexture;
        private bool isCurveShown;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if (!isCurveShown) {
                return EditorGUIUtility.singleLineHeight * 3f;
            }

            return EditorGUIUtility.singleLineHeight * 8f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (graphTexture == null) {
                graphTexture = new Texture2D(1, 1, TextureFormat.RGBA32, true);
            }

            curve = SerializedPropertyUtil.GetTargetObjectOfProperty(property) as ResponseCurve ?? new ResponseCurve();
            GUIRect rect = new GUIRect(position);
            isCurveShown = curve.__editorOnlyFoldout__ = EditorGUI.Foldout(rect.GetFieldRect(), curve.__editorOnlyFoldout__, label.text);

            GUIStyle style = new GUIStyle(GUI.skin.box);

            if (!isCurveShown) {
                DrawGraph(64, 32);
                GUIContent content = new GUIContent();
                content.text = curve.DisplayString;
                content.image = graphTexture;
                style.alignment = TextAnchor.MiddleLeft;
                GUI.Box(rect.GetFieldRect(2), content, style);
                return;
            }
            
            GUIRect[] splits = rect.SplitHorizontal(0.5f);
            GUIRect left = splits[0];
            GUIRect right = splits[1];

            DrawGraph(right.GetRect().width, right.GetRect().height);

            GUIContent graphContent = new GUIContent();
            graphContent.image = graphTexture;
            GUI.Box(right.GetRect(), graphContent, style);
            float oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;
            
            DrawerUtil.PushIndentLevel(1);
            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.PropertyField(left.GetFieldRect(), property.FindPropertyRelative("curveType"));
                EditorGUI.PropertyField(left.GetFieldRect(), property.FindPropertyRelative("slope"));
                EditorGUI.PropertyField(left.GetFieldRect(), property.FindPropertyRelative("exp"));
                EditorGUI.PropertyField(left.GetFieldRect(), property.FindPropertyRelative("vShift"));
                EditorGUI.PropertyField(left.GetFieldRect(), property.FindPropertyRelative("hShift"));
                EditorGUI.PropertyField(left.GetFieldRect(), property.FindPropertyRelative("threshold"));
                GUIRect lineRect = new GUIRect(left.GetFieldRect());
                GUIRect[] lineRectParts = lineRect.SplitHorizontal(0.5f);
                EditorGUI.PropertyField(lineRectParts[0].GetRect(), property.FindPropertyRelative("invert"));


                if (GUI.Button(lineRectParts[1].GetRect(), "Reset Curve")) {
                    property.FindPropertyRelative("curveType").intValue = (int) ResponseCurveType.Polynomial;
                    property.FindPropertyRelative("slope").floatValue = 1f;
                    property.FindPropertyRelative("exp").floatValue = 1f;
                    property.FindPropertyRelative("vShift").floatValue = 0f;
                    property.FindPropertyRelative("hShift").floatValue = 0f;
                    property.FindPropertyRelative("threshold").floatValue = 0f;
                    property.FindPropertyRelative("invert").boolValue = false;
                }
            }
            DrawerUtil.PopIndentLevel();
            EditorGUIUtility.labelWidth = oldWidth;
            if (EditorGUI.EndChangeCheck()) {
                property.serializedObject.ApplyModifiedProperties();
            }
            
        }

        private void DrawGraph(float width, float height) {
            graphTexture.Resize((int) width, (int) height);
            Rect graphRect = new Rect(0, 0, width, height);
            GraphHelper.DrawGraphLines(graphRect, graphTexture, curve.Evaluate);
            graphTexture.FlipVertically();
            graphTexture.Apply(true);
        }

    }

}