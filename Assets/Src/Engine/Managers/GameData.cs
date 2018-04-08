using Weichx.Util;
using SpaceGame.AI;
using UnityEngine;

namespace SpaceGame.Engine {

    public class GameData {

        public static readonly GameData Instance = new GameData();

        public readonly ListX<Entity> entityMap;
        public readonly ListX<Transform> transformMap;
        public readonly ListX<TransformInfo> transformInfoMap;
        public readonly ListX<WaypointPath> waypointPaths;
        public readonly ListX<AIInfo> aiInfoMap;
        public readonly ListX<FlightController> flightControllers;

        protected GameData() {
            flightControllers = new ListX<FlightController>();
            entityMap = new ListX<Entity>(32);
            aiInfoMap = new ListX<AIInfo>(32);
            transformMap = new ListX<Transform>(32);
            transformInfoMap = new ListX<TransformInfo>(32);
            waypointPaths = new ListX<WaypointPath>(16);
        }

        public Entity CreateEntity(EntityDefinition entityDefinition) {
            Entity entity = GameDatabase.ActiveInstance.FindSceneEntityById(entityDefinition.sceneEntityId);
            Debug.Assert(entity != null, nameof(entity) + " != null");
            if (!entityMap.Contains(entity)) {
                entity.index = entityMap.Count; // assume we never move or remove for now
                entityMap.Add(entity);
                flightControllers.Add(new FlightController());
                transformMap.Add(entity.transform);
                transformInfoMap.Add(new TransformInfo(entity));
                aiInfoMap.Add(new AIInfo(entity.index));
                return entity;
            }
            return null;
        }

    }

}