using System.Collections.Generic;
using JetBrains.Annotations;
using SpaceGame.FileTypes;
using UnityEngine;
using Weichx.Util;

namespace SpaceGame {

    public class FactionDefinition : AssetDefinition {

        private Texture2D iconTexture;

        public AssetPointer<Texture2D> iconPointer;
        [HideInInspector] public readonly List<FlightGroupDefinition> flightGroups;

        [PublicAPI]
        public FactionDefinition() : this("Faction") { }

        public FactionDefinition(string name) : base(name) {
            this.iconPointer = new AssetPointer<Texture2D>();
            this.flightGroups = new List<FlightGroupDefinition>(16);
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

        public FlightGroupDefinition FindFlightGroupById(int id) {
            return flightGroups.Find(id, (fg, targetId) => fg.id == targetId);
        }

        public FlightGroupDefinition AddFlightGroup() {
            FlightGroupDefinition flightGroupDefinition = new FlightGroupDefinition("Flight Group");
            flightGroupDefinition.factionId = id;
            flightGroups.Add(flightGroupDefinition);
            return flightGroupDefinition;
        }

        public FlightGroupDefinition InsertFlightGroup(FlightGroupDefinition flightGroup, int index) {
            int currentIndex = flightGroups.IndexOf(flightGroup);
            if (currentIndex == -1) {
                flightGroups.Insert(index, flightGroup);
            }
            else {
                flightGroups.MoveToIndex(currentIndex, index);
            }

            return flightGroup;
        }

        public FlightGroupDefinition RemoveFlightGroup(FlightGroupDefinition flightGroup) {
            if (flightGroup == flightGroups[0]) return null;
            return flightGroups.Remove(flightGroup) ? flightGroup : null;
        }

        public FlightGroupDefinition GetDefaultFlightGroup() {
            return flightGroups[0];
        }

        public static FactionDefinition CreateFaction() {
            FactionDefinition faction = new FactionDefinition();
            faction.AddFlightGroup();
            return faction;
        }

    }

}