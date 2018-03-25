using System.Collections.Generic;
using SpaceGame;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;
using Weichx.ReflectionAttributes.Markers;

namespace Editor.Drawers {

    [PropertyDrawerFor(typeof(FactionSelector))]
    public class FactionSelectorDrawer : ReflectedPropertyDrawer {

        private List<string> factionNames = new List<string>(4);

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            MissionDefinition mission = GameDatabase.ActiveInstance.GetCurrentMission();

            Debug.Assert(property.DeclaredType == typeof(int), "property.DeclaredType == typeof(int)");

            List<FactionDefinition> factions = mission.GetFactions();

            foreach (FactionDefinition faction in factions) {
                factionNames.Add(faction.name);
            }

            int currentIndex = factions.FindIndex((f) => f.id == property.intValue);
            int newIndex = EditorGUI.Popup(position, property.Label, currentIndex, factionNames.ToArray());
            if (currentIndex != newIndex) {
                property.Value = factions[newIndex].id;
            }
        }

    }

}