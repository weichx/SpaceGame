using System.Collections.Generic;
using SpaceGame;
using SpaceGame.AI;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;
using Weichx.ReflectionAttributes.Markers;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(FlightGroupSelector))]
    public class FlightGroupSelectorDrawer : ReflectedPropertyDrawer {

        private List<string> flightGroupNames = new List<string>();

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            MissionDefinition mission = GameDatabase.ActiveInstance.GetCurrentMission();

            Debug.Assert(property.DeclaredType == typeof(int), "property.DeclaredType == typeof(int)");

            List<FlightGroupDefinition> flightGroups = mission.GetFlightGroups();
            flightGroupNames.Clear();
            foreach (FlightGroupDefinition fg in flightGroups) {
                string factionName = $"[{mission.GetFactionById(fg.factionId).name}]";
                flightGroupNames.Add($"{factionName} {fg.name}");
            }
            
            int index = flightGroups.FindIndex((f) => f.id == property.intValue);
            int newIndex = EditorGUI.Popup(position, property.Label, index, flightGroupNames.ToArray());
            if (index != newIndex) {
                property.Value = flightGroups[newIndex].id;
            }
        }

    }

}