using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Weichx.Util;

namespace SpaceGame.Assets {

    public class MissionDefinition : GameAsset {

        public delegate void MissionDataChangedCallback(int changedItemId);

        public readonly List<FactionDefinition> factions;

        public event MissionDataChangedCallback onChange;

        [SerializeField] private int assetIdGenerator;

        [UsedImplicitly]
        private MissionDefinition() { }

        public MissionDefinition(int id, string name) : base(id, name) {
            this.factions = new List<FactionDefinition>();
        }

        public int NextAssetId => ++assetIdGenerator;

        public EntityDefinition CreateEntity(FlightGroupDefinition flightGroup) {
            if (HasFlightGroup(flightGroup)) {
                EntityDefinition entity = new EntityDefinition(NextAssetId);
                flightGroup.AddEntity(entity);
                onChange?.Invoke(entity.id);
                return entity;
            }
            else {
                throw new ArgumentException("Flight group not part of any faction in mission");
            }
        }

        public FlightGroupDefinition CreateFlightGroup(FactionDefinition faction) {
            if (faction == null || !factions.Contains(faction)) {
                throw new ArgumentException("Faction not part of this mission!");
            }

            FlightGroupDefinition flightGroup = new FlightGroupDefinition(NextAssetId);
            faction.AddFlightGroup(flightGroup);
            onChange?.Invoke(flightGroup.id);
            return flightGroup;
        }

        public FactionDefinition CreateFaction() {
            FactionDefinition faction = new FactionDefinition(NextAssetId);
            faction.AddFlightGroup(new FlightGroupDefinition(NextAssetId));
            factions.Add(faction);
            onChange?.Invoke(faction.id);
            return faction;
        }

        public void SetEntityFlightGroup(EntityDefinition entity, FlightGroupDefinition flightGroup, int index) {
            Debug.Assert(HasFlightGroup(flightGroup), $"Flight group {flightGroup.name} not part of any faction in mission");

            if (entity.flightGroupId == flightGroup.id) {
                if (index == -1) index = 0;
                flightGroup.entities.MoveToIndex(entity, index);
            }
            else {
                FlightGroupDefinition currentFlightGroup = GetFlightGroup(entity.flightGroupId);
                currentFlightGroup?.RemoveEntity(entity);
                flightGroup.AddEntity(entity, index);
            }
            onChange?.Invoke(entity.id);
        }

        public void SetFlightGroupFaction(FlightGroupDefinition flightGroup, FactionDefinition faction, int index) {
            if (flightGroup.factionId != faction.id) {
                FactionDefinition oldFaction = GetFaction(flightGroup.factionId);
                if (oldFaction != null && oldFaction.flightGroups.Count == 1) {
                    Debug.Log($"Can't Move Flight Group {flightGroup.name}, every faction needs at least one flight group");
                    return;
                }
                oldFaction?.RemoveFlightGroup(flightGroup);
            }
            faction.AddFlightGroup(flightGroup, index);
            onChange?.Invoke(flightGroup.id);
        }

        public void SetEntityFaction(EntityDefinition entity, FactionDefinition faction, int index) {
            if (entity.factionId == faction.id) return;
            GetFlightGroup(entity.flightGroupId)?.RemoveEntity(entity);
            faction.GetDefaultFlightGroup().AddEntity(entity, index);
            onChange?.Invoke(entity.id);
        }

        public void SetFactionIndex(FactionDefinition faction, int index) {
            Debug.Assert(faction != null, "factiondefinition != null");
            Debug.Assert(factions.Contains(faction), "factions.Contains(factiondefinition)");

            if (index == -1) {
                index = factions.Count - 1;
            }
            factions.MoveToIndex(faction, index);
            onChange?.Invoke(faction.id);
        }

        public void DeleteAsset(MissionAsset asset) {
            Debug.Assert(asset != null, "asset != null");

            EntityDefinition entity = asset as EntityDefinition;
            FlightGroupDefinition flightGroup = asset as FlightGroupDefinition;
            FactionDefinition faction = asset as FactionDefinition;

            if (entity != null) {
                GetFlightGroup(entity.flightGroupId)?.RemoveEntity(entity);
            }
            else if (flightGroup != null) {
                faction = GetFaction(flightGroup.factionId);
                if (faction.flightGroups.Count == 1) {
                    Debug.Log($"Can't Delete Flight Group {flightGroup.name}, every faction needs at least one flight group");
                    return;
                }
                GetFaction(flightGroup.factionId)?.RemoveFlightGroup(flightGroup);
            }
            else if (faction != null) {
                factions.Remove(faction);
            }
            else {
                throw new ArgumentException($"Can\'t delete: {asset.GetType().Name}");
            }
        }

        private FactionDefinition GetFaction(int id) {
            return factions.Find((faction) => faction.id == id);
        }

        private FlightGroupDefinition GetFlightGroup(int id) {
            return (from faction in factions from fg in faction.flightGroups where fg.id == id select fg).FirstOrDefault();
        }

        private bool HasFlightGroup(FlightGroupDefinition flightGroup) {
            return (
                       from faction in factions
                       from fg in faction.flightGroups
                       where fg == flightGroup
                       select fg
                   ).FirstOrDefault() != null;
        }

        public List<EntityDefinition> GetEntities() {
            return GetFlightGroups().SelectMany((flightGroup) => flightGroup.entities).ToList();
        }

        public List<FlightGroupDefinition> GetFlightGroups() {
            return factions.SelectMany((faction) => faction.flightGroups).ToList();
        }

        public FactionDefinition GetFactionById(int factionId) {
            return factions.Find(factionId, (f, fid) => f.id == fid);
        }

        public List<FactionDefinition> GetFactions() {
            return factions;
        }

    }

}