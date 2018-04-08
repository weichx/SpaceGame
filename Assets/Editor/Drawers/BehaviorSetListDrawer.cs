using System.Collections.Generic;
using Weichx.Util;
using Rotorz.ReorderableList;
using SpaceGame;
using SpaceGame.Assets;
using SpaceGame.EditorComponents;
using SpaceGame.Engine;
using SpaceGameEditor.Drawers;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace Drawers {

    [PropertyDrawerFor(typeof(BehaviorSet[]))]
    [PropertyDrawerFor(typeof(List<BehaviorSet>))]
    [PropertyDrawerFor(typeof(ListX<BehaviorSet>))]
    public class BehaviorSetListDrawer : ReflectedPropertyDrawerX {

        private ReflectedPropertyAdapter adapter;
        private ReorderableListControl listControl;

        public override void OnInitialize() {

            listControl = new ReorderableListControl(
                ReorderableListFlags.HideAddButton |
                ReorderableListFlags.DisableDuplicateCommand
            );

            GUISkin skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/Editor Default Resources/MissionWindowSkin.asset");
            listControl.ContainerStyle = skin?.FindStyle("goal_list") ?? listControl.ContainerStyle;

            adapter = new ReflectedPropertyAdapter((ReflectedListProperty) property);

            listControl.AddMenuClicked += CreateMenu;
        }

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            guiRect.SetRect(position);

            EditorGUIX.Foldout(guiRect.GetFieldRect(), property);

            if (property.IsExpanded) {
                listControl.Draw(guiRect.GetRect(), adapter);
            }
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            float height = EditorGUIUtility.singleLineHeight;
            if (property.IsExpanded) {
                height += listControl != null ? listControl.CalculateListHeight(adapter) : 0;
            }
            return height;
        }

        private void CreateMenu(object sender, AddMenuClickedEventArgs args) {
            GameDatabase db = GameDatabase.ActiveInstance;
            IReadonlyListX<BehaviorSet> behaviorSets = db.GetAssetList<BehaviorSet>();
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < behaviorSets.Count; i++) {
                GUIContent content = new GUIContent($"Add {behaviorSets[i].name}");
                BehaviorSet bs = behaviorSets[i];
                menu.AddItem(content, false, () => {
                    propertyAsList.AddElement(bs);
                });
            }
            menu.ShowAsContext();
        }
    }

}