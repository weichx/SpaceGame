using System;
using Editor.GUIComponents;
using SpaceGame.AI;
using SpaceGame.EditorComponents;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(Consideration), PropertyDrawerForOption.IncludeSubclasses)]
    public class ConsiderationDrawer : ReflectedPropertyDrawer {

        private static readonly GUIRect guiRect = new GUIRect();
        private static readonly ResponseCurve s_nullCurve = new ResponseCurve();

        private Type considerationType;
        private Texture2D graphTexture;
        private GUIStyle style;

        public override void OnInitialize() {
            graphTexture = new Texture2D(1, 1);
        }

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {

            //can't run this in initialize because editor styles is null there
            if (style == null) {
                style = new GUIStyle(EditorStyles.popup);
                style.normal.textColor = Color.blue;
                style.active.textColor = Color.blue;
                style.focused.textColor = Color.blue;
            }

            guiRect.SetRect(position);

            ReflectedProperty curveProperty = property["curve"];

            if (curveProperty == null) {
                DrawHeaderBox(property, false, s_nullCurve);
                return;
            }

            ResponseCurve curve = curveProperty.Value as ResponseCurve;

            if (!property.IsExpanded) {
                DrawCollapsed(property, curve);
            }
            else {
                DrawExpanded(property, curve);
            }

        }

        private string FormatTypeName(string typeName) {
            return ObjectNames.NicifyVariableName(typeName.Replace("Consideration", ""));
        }

        private void DrawExpanded(ReflectedProperty property, ResponseCurve curve) {
            DrawHeaderBox(property, false, curve);
            property["curve"].Value = EditorGUIX.ResponseCurveField(guiRect.GetFieldRect(7), curve);
            guiRect.GetFieldRect();
            EditorGUIX.DrawProperties(guiRect.GetRect(), property, new[] {"curve"});
        }

        private void DrawHeaderBox(ReflectedProperty property, bool withPreview, ResponseCurve curve) {

            Rect headerRect = guiRect.GetFieldRect(2);
            Rect foldoutRect = new Rect(headerRect) {
                y = headerRect.y + 8f,
                width = 20f
            };

            Rect texRect = new Rect(headerRect) {
                x = headerRect.x + 20,
                width = 64f
            };

            Rect selectRect = new Rect(headerRect) {
                x = texRect.x + texRect.width + 8,
                y = texRect.y + 8,
                height = EditorGUIUtility.singleLineHeight,
                width = headerRect.width - texRect.width - foldoutRect.width - 8f
            };

            property.IsExpanded = EditorGUI.Foldout(foldoutRect, property.IsExpanded, string.Empty);

            if (withPreview) {
                if (Event.current.type == EventType.Repaint) {
                    curve.DrawGraph(graphTexture, 64, 32);
                }

                if (Event.current.type == EventType.MouseDown) {
                    if (texRect.Contains(Event.current.mousePosition)) {
                        if (property.Value != null) {
                            property.IsExpanded = true;
                            Event.current.Use();
                        }
                    }
                }

                EditorGUI.DrawPreviewTexture(texRect, graphTexture);
            }
            DrawTypeSelect(selectRect, property);

        }

        private void DrawTypeSelect(Rect selectRect, ReflectedProperty property) {
            Type newConsiderationType;

            if (property.Value != null) {
                considerationType = property.Value.GetType();
                newConsiderationType = EditorGUIX.ConstructableTypePopup<Consideration>(selectRect, considerationType, FormatTypeName, style);
            }
            else {
                considerationType = null;
                newConsiderationType = EditorGUIX.ConstructableTypePopup<Consideration>(selectRect, considerationType, FormatTypeName, style);
            }

            if (newConsiderationType != considerationType) {
                considerationType = newConsiderationType;
                Consideration instance = EditorReflector.MakeInstance<Consideration>(considerationType);
                property.SetValueAndCopyCompatibleProperties(instance);
            }

        }

        private void DrawCollapsed(ReflectedProperty property, ResponseCurve curve) {
            DrawHeaderBox(property, true, curve);
            EditorGUIX.DrawProperties(guiRect.GetRect(), property, new[] {"curve"});
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            float height = 2f * EditorGUIX.singleLineHeight;
            if (property.Value != null) {
                height += GetChildHeights(property, new[] {"curve"});
            }
            if (property.IsExpanded) {
                height += 8f * EditorGUIUtility.singleLineHeight;
            }
            return height;
        }

    }

}