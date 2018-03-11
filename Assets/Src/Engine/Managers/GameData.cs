using System.Collections.Generic;
using SpaceGame;
using SpaceGame.AI;
using UnityEngine;

namespace Src.Engine {

    public struct FlightInput {

        public int entityId;
        public float throttle;
        public Vector3 targetUp;
        public Vector3 targetForward;

        public FlightInput(int entityId) {
            this.entityId = entityId;
            this.throttle = 0f;
            this.targetUp = Vector3.zero;
            this.targetForward = Vector3.zero;
        }

    }

    public struct PropulsionCapabilities { }
    
    public struct PhysicsInfo {

        public float speed;
        public Vector3 velocity;
        public float acceleration;
        public Vector3 angularVelocity;
        public Vector3 velocityTrend;

    }
    
    public class GameData {

        public static readonly GameData Instance = new GameData();

        public readonly List<Entity> entityMap;
        public readonly List<Transform> transformMap;
        public readonly List<TransformInfo> transformInfoMap;
        public readonly List<FlightInput> flightInputs;
        public readonly List<WaypointPath> waypointPaths;
        public readonly List<AIInfo> aiInfoMap;
        
        protected GameData() {
            entityMap = new List<Entity>(32);
            aiInfoMap = new List<AIInfo>(32);
            transformMap = new List<Transform>(32);
            transformInfoMap = new List<TransformInfo>(32);
            flightInputs = new List<FlightInput>(32);
            waypointPaths = new List<WaypointPath>(16);
        }

        public void RegisterEntity(Entity entity) {
            if (!entityMap.Contains(entity)) {
                entity.id = entityMap.Count;

                entityMap.Add(entity);
                transformMap.Add(entity.transform);
                transformInfoMap.Add(new TransformInfo(entity));
                flightInputs.Add(new FlightInput(entity.id));
                aiInfoMap.Add(new AIInfo(entity.id));
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
