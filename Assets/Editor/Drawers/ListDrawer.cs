using Rotorz.ReorderableList;
using SpaceGame.EditorComponents;
using SpaceGameEditor.Drawers;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace Drawers {

    public abstract class ListDrawer : ReflectedPropertyDrawerX {

        protected ReflectedPropertyAdapter adapter;
        protected ReorderableListControl listControl;

        protected virtual bool useCreateMenu => true;
        
        protected virtual ReorderableListFlags listFlags =>
            (ReorderableListFlags.HideAddButton |
             ReorderableListFlags.DisableDuplicateCommand);

        public override void OnInitialize() {

            listControl = new ReorderableListControl(listFlags);

            GUISkin skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/Editor Default Resources/MissionWindowSkin.asset");
            listControl.ContainerStyle = skin?.FindStyle("goal_list") ?? listControl.ContainerStyle;

            adapter = new ReflectedPropertyAdapter(propertyAsList);

            if (useCreateMenu) {
                listControl.AddMenuClicked += CreateMenu;
            }
            
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

        protected virtual void CreateMenu(object sender, AddMenuClickedEventArgs args) { }

    }

}