using System.Collections.Generic;
using SpaceGame;
using SpaceGame.AI;
using SpaceGame.Assets;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;
using Weichx.ReflectionAttributes.Markers;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(EntitySelector))]
    public class EntitySelectorDrawer : ReflectedPropertyDrawer {

        private List<string> entityNames = new List<string>();
        private List<EntityDefinition> entities = new List<EntityDefinition>();

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            MissionDefinition mission = GameDatabase.ActiveInstance.GetCurrentMission();

            Debug.Assert(property.DeclaredType == typeof(int), "property.DeclaredType == typeof(int)");

            List<FactionDefinition> factions = mission.GetFactions();

            entities.Clear();
            entityNames.Clear();

            int counter = 0;
            int currentIndex = -1;
            int currentId = property.intValue;

            foreach (FactionDefinition faction in factions) {
                string factionName = $"[{faction.name}]";

                foreach (FlightGroupDefinition flightGroup in faction.flightGroups) {
                    string flightGroupName = $"[{flightGroup.name}]";

                    foreach (EntityDefinition entity in flightGroup.entities) {
                        entityNames.Add($"{factionName}::{flightGroupName} {entity.name}");
                        if (entity.id == currentId) {
                            currentIndex = counter;
                        }
                        entities.Add(entity);
                        counter++;
                    }
                }

            }
            
            int newIndex = EditorGUI.Popup(position, property.Label, currentIndex, entityNames.ToArray());
            if (currentIndex != newIndex) {
                property.Value = entities[newIndex].id;
            }

        }

    }

}