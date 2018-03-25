using System.Collections.Generic;
using JetBrains.Annotations;
using SpaceGame.AI;
using SpaceGame.FileTypes;
using UnityEngine;
using Weichx.ReflectionAttributes;
using Weichx.Util;

namespace SpaceGame {

    public class FactionDefinition : AssetDefinition {

        public AssetPointer<Texture2D> iconPointer;
        
        [CreateOnReflect]
        public readonly List<FlightGroupDefinition> flightGroups;
        
        [DefaultExpanded] [CreateOnReflect]
        public readonly List<Goal> goals;
        
        private Texture2D iconTexture;
        
        [HideInInspector]
        public readonly MissionDefinition mission;
        
        [UsedImplicitly]
        public FactionDefinition() { }

        public FactionDefinition(int id, MissionDefinition mission) : base(id) {
            this.name = $"Faction {id}";
            this.mission = mission;
            this.flightGroups = new List<FlightGroupDefinition>();
        }

        public FactionDefinition(int id, string name) : base(id, name) {
            this.iconPointer = new AssetPointer<Texture2D>();
            this.flightGroups = new List<FlightGroupDefinition>();
        }

        public Texture2D icon {
            get {
                if (iconTexture) {
                    return iconTexture;
                }
                return null;
            }
            set {
                iconPointer = new AssetPointer<Texture2D>(value);
                iconTexture = iconPointer.GetAsset();
            }
        }

        public FlightGroupDefinition GetDefaultFlightGroup() {
            return flightGroups[0];
        }

        public FlightGroupDefinition AddFlightGroup(FlightGroupDefinition flightGroup, int index = -1) {
            if (flightGroup.factionId == id) {
                if (index == -1) index = 0;
                Debug.Assert(flightGroups.Contains(flightGroup), "flightGroups.Contains(flightGroup)");
                flightGroups.MoveToIndex(flightGroup, index);
                return flightGroup;
            }
            flightGroup.factionId = id;
            if (index == -1) {
                flightGroups.Add(flightGroup);
            }
            else {
                flightGroups.Insert(index, flightGroup);
            }
            return flightGroup;
        }
        
        public void RemoveFlightGroup(FlightGroupDefinition flightGroup) {
            if (flightGroups.Remove(flightGroup)) {
                flightGroup.factionId = -1;
            }
        }

    }

}