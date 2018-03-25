using System;
using System.Collections.Generic;
using UnityEngine;
using Weichx.Util;
using Debug = System.Diagnostics.Debug;

namespace SpaceGame {

    public class GameDatabase {

        public readonly List<FactionDefinition> factionsDefinitions;
        public readonly List<EntityDefinition> entityDefinitions;
        public readonly List<FlightGroupDefinition> flightGroupDefinitions;
        public readonly List<ShipDefinition> shipDefinitions;
        public readonly List<MissionDefinition> missionDefinitions;

        [SerializeField] private MissionDefinition currentMission;
        [SerializeField] private int idGenerator;

        public GameDatabase() {
            this.idGenerator = 0;
            this.shipDefinitions = new List<ShipDefinition>();
            this.entityDefinitions = new List<EntityDefinition>();
            this.factionsDefinitions = new List<FactionDefinition>();
            this.flightGroupDefinitions = new List<FlightGroupDefinition>();
            this.missionDefinitions = new List<MissionDefinition>();
        }

        public static GameDatabase ActiveInstance { get; set; }

        private List<T> GetList<T>() {
            Type type = typeof(T);
            if (type == typeof(ShipDefinition)) return this.shipDefinitions as List<T>;
            if (type == typeof(EntityDefinition)) return this.entityDefinitions as List<T>;
            if (type == typeof(FactionDefinition)) return this.factionsDefinitions as List<T>;
            if (type == typeof(FlightGroupDefinition)) return this.flightGroupDefinitions as List<T>;
            if (type == typeof(MissionDefinition)) return this.missionDefinitions as List<T>;
            return null;
        }

        public T CreateAsset<T>() where T : AssetDefinition {
            T instance = Activator.CreateInstance(typeof(T), true) as T;
            Debug.Assert(instance != null, nameof(instance) + " != null");
            instance.id = ++idGenerator;
            GetList<T>().Add(instance);
            return instance;
        }

        public T FindOrCreate<T>(int selectedId) where T : AssetDefinition  {
            return FindAsset<T>(selectedId) ?? CreateAsset<T>();
        }

        public T FindAsset<T>(int selectedId) where T : AssetDefinition {
            return GetList<T>().Find(selectedId, (def, id) => def.id == id);
        }

        public T DestroyAsset<T>(int id) where T : AssetDefinition {
            List<T> list = GetList<T>();
            int index = list.FindIndex(id, (def) => def.id == id);
            if (index != -1) {
                T removed = list[index];
                list.RemoveAt(index);
                return removed;
            }
            return null;
        }

        public FlightGroupDefinition GetDefaultFlightGroup(int factionId) {
            List<FlightGroupDefinition> flightGroups = flightGroupDefinitions.FindAll((fg) =>
                fg.factionId == factionId
            );
            if (flightGroups.Count == 0) {
                return CreateAsset<FlightGroupDefinition>();
            }
            flightGroups.Sort((a, b) => a.id < b.id ? 1 : -1);
            return flightGroups[0];
        }

        public void SetCurrentMission(MissionDefinition missionDefinition) {
            this.currentMission = missionDefinition;
        }
        
        public MissionDefinition GetCurrentMission() {
            if (currentMission != null) return currentMission;
            if (missionDefinitions.Count > 0) {
                currentMission = missionDefinitions[0];
            }
            else {
                currentMission = CreateAsset<MissionDefinition>();
            }
            return currentMission;

        }

    }

}