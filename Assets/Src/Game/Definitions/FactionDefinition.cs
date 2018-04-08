using System.Collections.Generic;
using JetBrains.Annotations;
using SpaceGame.AI;
using SpaceGame.FileTypes;
using UnityEngine;
using Weichx.ReflectionAttributes;
using Weichx.Util;

namespace SpaceGame {

    public class FactionDefinition : MissionAsset, IParentAsset {

        public AssetPointer<Texture2D> iconPointer;
        
        [CreateOnReflect]
        public readonly List<FlightGroupDefinition> flightGroups;
        
        [DefaultExpanded] [CreateOnReflect]
        public readonly List<Goal> goals;
        
        private Texture2D iconTexture;
                
        [UsedImplicitly]
        private FactionDefinition() { }

        private FactionDefinition(int id, string name) : base(id, name) {
            this.iconPointer = new AssetPointer<Texture2D>();
            this.goals = new List<Goal>();
            this.flightGroups = new List<FlightGroupDefinition>();
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