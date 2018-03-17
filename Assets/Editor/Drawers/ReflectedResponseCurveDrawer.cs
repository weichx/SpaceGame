using Editor.GUIComponents;
using SpaceGame.AI;
using SpaceGame.Editor.GUIComponents;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;
using Weichx.Util.Texture2DExtensions;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(ResponseCurve))]
    public class ReflectedResponseCurveDrawer : ReflectedPropertyDrawer {

        private ResponseCurve curve;
        private static Texture2D graphTexture;
        private static GUIRect guiRect = new GUIRect();
        private static GUIStyle style = new GUIStyle(GUI.skin.box);

        private static readonly string[] presetCurveNames = {
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

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {
            guiRect.SetRect(position);
            if (graphTexture == null) {
                graphTexture = new Texture2D(1, 1, TextureFormat.RGBA32, true);
            }

            curve = (ResponseCurve) property.Value ?? new ResponseCurve();

            if(property.IsExpanded) EditorGUIX.Foldout(guiRect, property);
            
            if (!property.IsExpanded) {
                DrawGraph(64, 32);
                GUIContent content = EditorGUIX.TempLabel(curve.ShortDisplayString);
                content.image = graphTexture;
                style.alignment = TextAnchor.MiddleLeft;
                GUI.Box(guiRect.GetFieldRect(2), content, style);
                content.image = null;
                if (Event.current.type == EventType.MouseDown) {
                    if (position.Contains(Event.current.mousePosition)) {
                        property.IsExpanded = true;
                        Event.current.Use();
                    }
                }
                return;
            }

            GUIRect[] splits = guiRect.SplitHorizontal(0.5f);
            GUIRect left = splits[0];
            GUIRect right = splits[1];

            DrawGraph(right.GetRect().width, right.GetRect().height);

            GUIContent graphContent = EditorGUIX.TempLabel(string.Empty);
            graphContent.image = graphTexture;
            GUI.Box(right.GetRect(), graphContent, style);
            graphContent.image = null;
            float oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;

            EditorGUIX.PropertyField(left.GetFieldRect(), property.FindProperty("curveType"));
            EditorGUIX.PropertyField(left.GetFieldRect(), property.FindProperty("slope"));
            EditorGUIX.PropertyField(left.GetFieldRect(), property.FindProperty("exp"));
            EditorGUIX.PropertyField(left.GetFieldRect(), property.FindProperty("vShift"));
            EditorGUIX.PropertyField(left.GetFieldRect(), property.FindProperty("hShift"));
            EditorGUIX.PropertyField(left.GetFieldRect(), property.FindProperty("threshold"));

            Rect lineRect = left.GetFieldRect();
            Rect toggleRect = new Rect(lineRect) {
                width = EditorGUIUtility.labelWidth + 16f
            };
            Rect selectRect = new Rect(lineRect) {
                x = lineRect.x + toggleRect.width,
                width = lineRect.width - toggleRect.width
            };

            EditorGUIX.PropertyField(toggleRect, property.FindProperty("invert"));

            int idx = EditorGUI.Popup(selectRect, 0, presetCurveNames);
            property.Value = GetPreset(presetCurveNames[idx], curve);
            
            EditorGUIUtility.labelWidth = oldWidth;
            property.ApplyChanges();

        }

        private void DrawGraph(float width, float height) {
            if (Event.current.type == EventType.Repaint) {
                graphTexture.Resize((int) width, (int) height);
                Rect graphRect = new Rect(0, 0, width, height);
                GraphHelper.DrawGraphLines(graphRect, graphTexture, curve.Evaluate);
                graphTexture.FlipVertically();
                graphTexture.Apply(true);
            }
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            if (!property.IsExpanded) {
                return EditorGUIUtility.singleLineHeight * 2f;
            }

            return EditorGUIUtility.singleLineHeight * 8f;
        }

    }

}