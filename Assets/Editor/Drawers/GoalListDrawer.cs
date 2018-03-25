using System;
using System.Collections.Generic;
using Editor.GUIComponents;
using Rotorz.ReorderableList;
using SpaceGame.AI;
using SpaceGame.EditorComponents;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(List<Goal>))]
    public class GoalListDrawer : ReflectedPropertyDrawer {

        private ReflectedPropertyAdapter adapter;
        private ReorderableListControl listControl;
        private static readonly GUIRect s_guiRect = new GUIRect();

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            s_guiRect.SetRect(position);
            
            EditorGUIX.Foldout(s_guiRect.GetFieldRect(), property);
            
            if (property.IsExpanded) {
                listControl.Draw(s_guiRect.GetRect(), adapter);
            }
        }

        public override void OnInitialize() {

            listControl = new ReorderableListControl(
                ReorderableListFlags.HideAddButton |
                ReorderableListFlags.DisableDuplicateCommand |
                ReorderableListFlags.HideRemoveButtons
            );
            
            GUISkin skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/Editor Default Resources/MissionWindowSkin.asset");
            listControl.ContainerStyle = skin.FindStyle("goal_list");

            adapter = new ReflectedPropertyAdapter((ReflectedListProperty) property);

            listControl.AddMenuClicked += CreateMenu;
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            float height = EditorGUIUtility.singleLineHeight;
            if (property.IsExpanded) {
                height += listControl != null ? listControl.CalculateListHeight(adapter) : 0;
            }
            return height;
        }
        
        private void CreateMenu(object sender, AddMenuClickedEventArgs args) {
            Type[] subClasses = EditorReflector.FindSubClasses<Goal>();
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < subClasses.Length; i++) {
                Type type = subClasses[i];
                if (!EditorReflector.IsDefaultConstructable(type)) continue;

                GUIContent content = new GUIContent($"Create {type.Name}");
                menu.AddItem(content, false, CreateGoal, type);
            }
            menu.ShowAsContext();
        }

        private void CreateGoal(object goalType) {
            propertyAsList.AddElement(EditorReflector.MakeInstance((Type)goalType));
        }

    }

}