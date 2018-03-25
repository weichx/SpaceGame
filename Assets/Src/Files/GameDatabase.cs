using System;
using System.Collections.Generic;
using System.Reflection;
using SpaceGame.Assets;
using Src.Game.Assets;
using UnityEngine;
using Weichx.Util;

namespace SpaceGame {

    public class GameDatabase {

        private static Type[] signature = { typeof(int), typeof(string) };
        private static object[] constructorParams = new object[2];
        public readonly List<ShipTypeGroup> shipTypeGroups;
        public readonly List<MissionDefinition> missionDefinitions;
        public readonly List<ShipType> shipTypes;
        
        public readonly List<BehaviorSet> behaviorSets;
        public readonly List<BehaviorDefinition> behaviorDefinitions;
        public readonly List<ActionDefinition> actionDefinitions;
        
        [SerializeField] private MissionDefinition currentMission;
        [SerializeField] private int idGenerator;

        public GameDatabase() {
            this.idGenerator = 0;
            this.shipTypeGroups = new List<ShipTypeGroup>();
            this.shipTypes = new List<ShipType>();
            this.missionDefinitions = new List<MissionDefinition>();
            this.behaviorSets = new List<BehaviorSet>(4);
            this.actionDefinitions = new List<ActionDefinition>(4);
            this.behaviorDefinitions = new List<BehaviorDefinition>(4);
        }

        public static GameDatabase ActiveInstance { get; set; }

        private List<T> GetList<T>() {
            Type type = typeof(T);
            if (type == typeof(BehaviorSet)) return behaviorSets as List<T>;
            if (type == typeof(BehaviorDefinition)) return behaviorDefinitions as List<T>;
            if (type == typeof(ActionDefinition)) return actionDefinitions as List<T>;
            if (type == typeof(ShipTypeGroup)) return this.shipTypeGroups as List<T>;
            if (type == typeof(ShipType)) return this.shipTypes as List<T>;
            if (type == typeof(MissionDefinition)) return this.missionDefinitions as List<T>;
            return null;
        }

        private const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public T CreateAsset<T>() where T : GameAsset {
            constructorParams[0] = ++idGenerator;
            constructorParams[1] = $"{StringUtil.NicifyName(typeof(T).Name, "Asset")} {idGenerator}";
            T instance = (T) typeof(T).GetConstructor(bindFlags, null, signature, null)?.Invoke(constructorParams);
            UnityEngine.Debug.Assert(instance != null, $"GameAssets must have an (int, string) constructor. {typeof(T).Name} does not.");
            GetList<T>().Add(instance);
            return instance;
        }

        public T FindOrCreate<T>(int selectedId) where T : GameAsset {
            return FindAsset<T>(selectedId) ?? CreateAsset<T>();
        }

        public T FindAsset<T>(int selectedId) where T : GameAsset {
            return GetList<T>().Find(selectedId, (def, id) => def.id == id);
        }

        public T DestroyAsset<T>(int id) where T : GameAsset {
            List<T> list = GetList<T>();
            int index = list.FindIndex((def) => def.id == id);
            if (index != -1) {
                T removed = list[index];
                list.RemoveAt(index);
                if (removed is ShipType) {
                    ShipTypeGroup shipGroup = FindAsset<ShipTypeGroup>((removed as ShipType).shipGroupId);
                    shipGroup?.ships.Remove(removed as ShipType);
                }
                return removed;
            }
            return null;
        }

        public List<ShipTypeGroup> GetShipTypeGroups() {
            return shipTypeGroups;
        }

        public void SetCurrentMission(MissionDefinition mission) {
            this.currentMission = mission;
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

        public void SetShipTypeShipGroup(ShipType shipType, int groupId, int index = -1) {
            int oldGroupId = shipType.shipGroupId;
            shipType.shipGroupId = groupId;
            ShipTypeGroup shipGroup = shipTypeGroups.Find(groupId, (group, id) => group.id == id);
            if (oldGroupId == shipGroup.id) {
                shipGroup.ships.MoveToIndex(shipType, index);
            }
            else {
                ShipTypeGroup oldGroup = shipTypeGroups.Find(oldGroupId, (group, id) => group.id == id);
                oldGroup?.ships.Remove(shipType);
                shipGroup.AddShipDefinition(shipType, index);
            }
        }

    }

}