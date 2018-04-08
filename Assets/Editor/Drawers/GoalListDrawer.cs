using System;
using System.Collections.Generic;
using Drawers;
using Rotorz.ReorderableList;
using SpaceGame.AI;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(List<Goal>))]
    public class GoalListDrawer : ListDrawer {

        protected override void CreateMenu(object sender, AddMenuClickedEventArgs args) {
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
            propertyAsList.AddElement(EditorReflector.MakeInstance((Type) goalType));
        }

    }

}