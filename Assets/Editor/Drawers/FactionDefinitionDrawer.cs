using System;
using Editor.GUIComponents;
using Rotorz.ReorderableList;
using SpaceGame;
using SpaceGame.AI;
using SpaceGame.EditorComponents;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(FactionDefinition))]
    public class FactionDefinitionDrawer : ReflectedPropertyDrawer {

        private const string GoalsField = nameof(FactionDefinition.goals);
        private const string NameField = nameof(FactionDefinition.name);

        private bool initialized;
        private ReflectedPropertyAdapter adapter;
        private GUIRect guiRect = new GUIRect();
        private ReorderableListControl listControl;
        private ReflectedProperty property;

        private static readonly Type[] signature = {typeof(MissionDefinition)};

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {
            this.property = property;
            if (property == null) return;
            Initialize(property);
            guiRect.SetRect(position);

            float oldLabelWidth = EditorGUIUtility.labelWidth;
            
            EditorGUIUtility.labelWidth = 100;
            EditorGUIX.PropertyField(guiRect.GetFieldRect(), property[NameField]);
            EditorGUIUtility.labelWidth = oldLabelWidth;
            
            EditorGUIX.PropertyField(guiRect, property[GoalsField]);
//            EditorGUIX.Foldout(guiRect.GetFieldRect(), property[GoalsField]);
//            if (property[GoalsField].IsExpanded) {
//                listControl.Draw(guiRect.GetRect(), adapter);
//            }
        }

        
        private void Initialize(ReflectedProperty property) {
            if (!initialized) {
                initialized = true;
                listControl = new ReorderableListControl(
                    ReorderableListFlags.HideAddButton |
                    ReorderableListFlags.DisableDuplicateCommand |
                    ReorderableListFlags.HideRemoveButtons
                );
                GUISkin skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/Editor Default Resources/MissionWindowSkin.asset");
                listControl.ContainerStyle = skin.FindStyle("faction_list");

                adapter = new ReflectedPropertyAdapter((ReflectedListProperty) property["goals"]);

                listControl.AddMenuClicked += (sender, args) => {
                    Type[] subClasses = EditorReflector.FindSubClasses<Goal>();
                    GenericMenu menu = new GenericMenu();

                    for (int i = 0; i < subClasses.Length; i++) {
                        Type type = subClasses[i];
                        if (EditorReflector.IsDefaultConstructable(type)) {
                            if (EditorReflector.HasConstructor(subClasses[i], signature)) {
                                menu.AddItem(new GUIContent($"Create {type.Name}"), false, CreateGoalWithMission, type);
                            }
                            else {
                                menu.AddItem(new GUIContent($"Create {type.Name}"), false, CreateGoal, type);
                            }
                        }
                    }
                    menu.ShowAsContext();
                };
            }
        }

        private void CreateGoalWithMission(object goalType) {
            Type type = (Type) goalType;
            MissionDefinition mission = property[nameof(FactionDefinition.mission)].Value as MissionDefinition;
            object goal = EditorReflector.MakeInstance(type, signature, new object[] {mission});
            property.GetList(nameof(FactionDefinition.goals)).AddElement(goal);
        }

        private void CreateGoal(object goalType) {
            Type type = (Type) goalType;
            object goal = EditorReflector.MakeInstance(type);
            property.GetList(nameof(FactionDefinition.goals)).AddElement(goal);
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            float height = 2f * EditorGUIUtility.singleLineHeight;
            height += listControl != null ? listControl.CalculateListHeight(adapter) : 0;
            return height;
        }

    }

}