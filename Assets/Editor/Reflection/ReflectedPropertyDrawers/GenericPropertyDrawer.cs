using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Weichx.EditorReflection {

    public class GenericPropertyDrawer : ReflectedPropertyDrawer {

        private static GUIContent tempContent = new GUIContent();

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {
            Type type = property.Type;

            if (type.IsEnum) {
                property.Value = EditorGUI.EnumPopup(position, property.GUIContent, (Enum) property.Value);
            }
            else if (type.IsSubclassOf(typeof(Object))) {
                property.Value = EditorGUI.ObjectField(position, label, (Object) property.Value, type, true);
            }
            else if (property is ReflectedListProperty) {
                RenderList(position, (ReflectedListProperty) property);
            }
            else if (property is ReflectedInstanceProperty) {
                RenderWithFields(position, (ReflectedInstanceProperty) property);
            }

        }

        private static void RenderWithFields(Rect position, ReflectedInstanceProperty instanceProperty) {
            Rect headRect = new Rect(position) {
                height = EditorGUIUtility.singleLineHeight
            };

            Rect bodyRect = new Rect(position) {
                y = headRect.y + headRect.height,
                height = position.height - headRect.height
            };
            instanceProperty.IsExpanded = EditorGUI.Foldout(headRect, instanceProperty.IsExpanded, instanceProperty.GUIContent);
            DrawProperties(bodyRect, instanceProperty);
        }

        private static void RenderList(Rect position, ReflectedListProperty listProperty) {
            Rect headRect = new Rect(position) {
                height = EditorGUIUtility.singleLineHeight
            };
            Rect sizeRect = new Rect(headRect) {
                y = headRect.y + headRect.height,
                height = EditorGUIUtility.singleLineHeight
            };
            Rect bodyRect = new Rect(position) {
                y = sizeRect.y + sizeRect.height,
                height = position.height - sizeRect.height - headRect.height
            };
            listProperty.IsExpanded = EditorGUI.Foldout(headRect, listProperty.IsExpanded, listProperty.GUIContent);
            if (listProperty.IsExpanded) {
                EditorGUI.indentLevel++;
                tempContent.text = "Size";
                listProperty.ElementCount = EditorGUI.IntField(sizeRect, tempContent, listProperty.ElementCount);
                EditorGUI.indentLevel++;
                DrawProperties(bodyRect, listProperty);
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
        }

        private static void DrawProperties(Rect position, ReflectedProperty root) {
            List<ReflectedProperty> properties = root.GetChildren();
            float remainingHeight = position.height;
            for (int i = 0; i < properties.Count; i++) {
                ReflectedProperty child = properties[i];
                float height = child.Drawer.GetPropertyHeight(child);
                remainingHeight -= height;
                position.height = height;
                Internal_PropertyField(position, child, child.GUIContent, child.IsHidden);
                position.y += height;
                position.height = remainingHeight;
            }
        }

        private static void Internal_PropertyField(Rect position, ReflectedProperty property, GUIContent label, bool isHidden) {
            if (isHidden) return;

            if (label == null) label = property.GUIContent;

            Debug.Assert(property.Drawer != null, "property.Drawer != null");
            if(!property.Drawer.IsInitialized) property.Drawer.OnInitialize();
            property.Drawer.OnGUI(position, property, label);

        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            if (property is ReflectedListProperty) {
                float height = EditorGUIUtility.singleLineHeight;
                if (property.IsExpanded) {
                    height += EditorGUIUtility.singleLineHeight;
                    height += GetChildHeights(property);
                }
                return height;
            }
            else if (property is ReflectedInstanceProperty) {
                float height = EditorGUIUtility.singleLineHeight;
                if (property.IsExpanded) {
                    height += GetChildHeights(property);
                }
                return height;
            }
            return EditorGUIUtility.singleLineHeight;
        }

    }

}