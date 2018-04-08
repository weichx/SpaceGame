using System;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using SpaceGame.AI;
using SpaceGame.Assets;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;
using Weichx.Util;

namespace Drawers {

    [PropertyDrawerFor(typeof(ListX<AIAction>))]
    [PropertyDrawerFor(typeof(List<AIAction>))]
    [PropertyDrawerFor(typeof(AIAction[]))]
    public class AIActionListDrawer : ListDrawer {

        protected override void CreateMenu(object sender, AddMenuClickedEventArgs args) {
            Type[] subClasses = EditorReflector.FindSubClasses<AIAction>();
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < subClasses.Length; i++) {
                Type type = subClasses[i];
                if (!EditorReflector.IsDefaultConstructable(type)) continue;

                GUIContent content = new GUIContent($"Create {StringUtil.NicifyName(type.Name, "Action")}");

                menu.AddItem(content, false, () => {
                    propertyAsList.AddElement(EditorReflector.MakeInstance(type));
                });

            }
            menu.ShowAsContext();
        }

    }

}