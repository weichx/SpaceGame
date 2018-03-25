using System.Collections.Generic;
using SpaceGame.AI;
using UnityEngine;

namespace SpaceGame.Engine {

    public class GameData {

        public static readonly GameData Instance = new GameData();

        public readonly List<Entity> entityMap;
        public readonly List<Transform> transformMap;
        public readonly List<TransformInfo> transformInfoMap;
        public readonly List<WaypointPath> waypointPaths;
        public readonly List<AIInfo> aiInfoMap;
        
        protected GameData() {
            entityMap = new List<Entity>(32);
            aiInfoMap = new List<AIInfo>(32);
            transformMap = new List<Transform>(32);
            transformInfoMap = new List<TransformInfo>(32);
            waypointPaths = new List<WaypointPath>(16);
        }

        public void RegisterEntity(Entity entity) {
            if (!entityMap.Contains(entity)) {
                entity.index = entityMap.Count; // assume we never remove for now
                entityMap.Add(entity);
                transformMap.Add(entity.transform);
                transformInfoMap.Add(new TransformInfo(entity));
                aiInfoMap.Add(new AIInfo(entity.index));
            }
        }

        public void RegisterWaypointPath(WaypointPath waypoint) {
            if (!waypointPaths.Contains(waypoint)) {
                waypoint.Initialize();
                waypointPaths.Add(waypoint);
            }    
        }
        
        public string GetEntityName(int entityId) {
            return entityMap[entityId].name;
        }

    }

}
