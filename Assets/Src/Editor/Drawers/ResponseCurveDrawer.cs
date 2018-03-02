using SpaceGame.AI;
using SpaceGame.Util.Texture2DExtensions;
using Src.Editor;
using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor {

    static class RectExtensions {

        public static Rect GetPropertyRect(this Rect self) {
            return new Rect() {
                x = self.x,
                y = self.y + EditorGUIUtility.singleLineHeight,
                width = self.width,
                height = self.height - EditorGUIUtility.singleLineHeight
            };
        }

    }

    public class GUIRect {

        private Rect rect;

        public GUIRect(Rect rect) {
            this.rect = rect;
        }

        public Rect GetFieldRect(int lineHeight = 1) {
            Rect retn = new Rect(rect);
            retn.height = lineHeight * EditorGUIUtility.singleLineHeight;
            rect.y += (lineHeight * EditorGUIUtility.singleLineHeight);
            rect.height -= (lineHeight * EditorGUIUtility.singleLineHeight);
            return retn;
        }

        public Rect GetRawRect() {
            return rect;
        }

        public GUIRect[] SplitHorizontal(float percentage) {
            GUIRect[] retn = new GUIRect[2];
            retn[0] = new GUIRect(new Rect() {
                x = rect.x,
                y = rect.y,
                width = rect.width * percentage,
                height = rect.height
            });
            retn[1] = new GUIRect(new Rect() {
                x = rect.x + (rect.width * percentage),
                y = rect.y,
                height = rect.height,
                width = rect.width * percentage
            });
            return retn;
        }

    }

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

            DrawGraph(right.GetRawRect().width, right.GetRawRect().height);

            GUIContent graphContent = new GUIContent();
            graphContent.image = graphTexture;
            GUI.Box(right.GetRawRect(), graphContent, style);
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
                EditorGUI.PropertyField(lineRectParts[0].GetRawRect(), property.FindPropertyRelative("invert"));


                if (GUI.Button(lineRectParts[1].GetRawRect(), "Reset Curve")) {
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