using System.Collections.Generic;
using Weichx.Util;
using Rotorz.ReorderableList;
using SpaceGame.AI;
using SpaceGame.EditorComponents;
using SpaceGameEditor.Drawers;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace Drawers {

    [PropertyDrawerFor(typeof(Consideration[]))]
    [PropertyDrawerFor(typeof(List<Consideration>))]
    [PropertyDrawerFor(typeof(ListX<Consideration>))]
    public class ConsiderationArrayDrawer : ReflectedPropertyDrawerX {

        private ReflectedPropertyAdapter adapter;

        public override void OnInitialize() {
            adapter = new ReflectedPropertyAdapter(propertyAsList);
        }

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            guiRect.SetRect(position);
            EditorGUIX.Foldout(guiRect.GetFieldRect(), property);
            if (property.IsExpanded) {
                ReorderableListGUI.ListFieldAbsolute(guiRect.GetRect(), adapter);
            }
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            float height = EditorGUIUtility.singleLineHeight;
            if (property.IsExpanded) {
                height += ReorderableListGUI.CalculateListFieldHeight(adapter);
            }
            return height;
        }

    }

}