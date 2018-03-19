using System;
using System.Collections.Generic;
using Weichx.ReflectionAttributes;
using Weichx.Util;

namespace SpaceGame {

    public class MissionDefinition : AssetDefinition {

        [ReadOnly] public readonly string createdAt;

        public readonly List<FactionDefinition> factionsDefinitions;
        //public readonly List<EntityDefinition> entityDefinitions;
        //public readonly List<FlightGroupDefinition> flightGroupDefinitions;

        public MissionDefinition() {
            this.name = "Unnamed Mission";
            this.factionsDefinitions = new List<FactionDefinition>(4);
            //     this.entityDefinitions = new List<EntityDefinition>(32);
            //     this.flightGroupDefinitions = new List<FlightGroupDefinition>(8);
            this.createdAt = $"{DateTime.Now.ToShortTimeString()} on {DateTime.Now.ToShortDateString()}";
        }

        public FactionDefinition AddFaction() {
            FactionDefinition faction = new FactionDefinition("Faction");
            faction.AddFlightGroup();
            factionsDefinitions.Add(faction);
            return faction;
        }

        public FactionDefinition RemoveFaction(FactionDefinition faction) {
            return factionsDefinitions.Remove(faction) ? faction : null;
        }

        public static MissionDefinition CreateMission() {
            MissionDefinition mission = new MissionDefinition();
            mission.AddFaction();
            return mission;
        }

        public bool MoveFaction(FactionDefinition child, int index) {
            if (!factionsDefinitions.Contains(child)) return false;
            return factionsDefinitions.MoveToIndex(child, index);
        }

    }

}