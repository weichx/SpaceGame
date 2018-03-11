using System.Collections.Generic;
using SpaceGame;
using SpaceGame.AI;
using SpaceGame.Events;
using SpaceGame.Util;
using UnityEngine;

namespace Src.Engine {

    /// <summary>
    /// This is the ONLY place where we should be touching the unity transform properties!
    /// </summary>
    public class PropulsionManager : ManagerBase {

        [Header("Arrival")] public ResponseCurve arrivalSpeedCurve; // exp = 0.42, invert = true;
        public float arrivalHyperspaceDistance = 3000f;
        public float hyperspaceEnterSpeed = 1500f;
        public float arrivedThreshold = 2800f;

        [Header("Departure")] public ResponseCurve departureSpeedCurve; // exp = 0.42, invert = false;
        public float departureHyperspaceDistance = 3000f;
        public float hyperspaceExitSpeed = 1500f;
        public float departedThreshold = 2800f;

        private List<Arrival> arrivals;
        private List<Departure> departures;
        private List<int> actives;

        public void Initialize() {
            arrivals = new List<Arrival>();
            departures = new List<Departure>();
            actives = new List<int>();
            AddListener<Evt_EntityArriving>(OnEntityArriving);
            AddListener<Evt_EntityArrived>(OnEntityArrived);
            AddListener<Evt_EntityDeparting>(OnEntityDeparting);
            AddListener<Evt_EntityDeparted>(OnEntityDeparted);
        }

        public void Tick() {
            TickArrivals();
            TickDepartures();
            TickActives();
            List<TransformInfo> transformInfos = GameData.Instance.transformInfoMap;
            List<Transform> transforms = GameData.Instance.transformMap;

            int count = transformInfos.Count;
            for (int i = 0; i < count; i++) {
                transforms[i].position = transformInfos[i].position;
                transforms[i].rotation = transformInfos[i].rotation;
            }
        }

        private void TickActives() {
            // Fighters
            // Largeships
            // Physics
            // No Physics
            // Player
            // Linear Weapons
            // Aspect Warheads
            List<TransformInfo> transformInfos = GameData.Instance.transformInfoMap;
            int count = actives.Count;
            float deltaTime = GameTimer.Instance.deltaTime;
            for (int i = 0; i < count; i++) {
                int entityId = actives[i];
                TransformInfo entityTransform = transformInfos[entityId];
                AIInfo agent = GameData.Instance.aiInfoMap[entityId];
                Quaternion rotation = PropulsionUtil.RotateTowardsDirection(
                    entityTransform.rotation,
                    entityTransform.DirectionTo(agent.Entity.targetPosition),
                    90f,//turnRate,
                    deltaTime
                );

                float speed = 100 * deltaTime;
                Vector3 position = entityTransform.position + (rotation.GetForward() * speed);

                transformInfos[entityId] = new TransformInfo(entityId, position, rotation);
            }
        }

        private void TickArrivals() {
            List<TransformInfo> transformInfos = GameData.Instance.transformInfoMap;
            float startDistanceSquared = arrivalHyperspaceDistance * arrivalHyperspaceDistance;
            float arrivalThresholdSquared = arrivedThreshold * arrivedThreshold;

            float deltaTime = GameTimer.Instance.deltaTime;
            int count = arrivals.Count;
            for (int i = 0; i < count; i++) {
                Arrival arrival = arrivals[i];
                TransformInfo entityTransform = transformInfos[arrival.entityId];

                float distRatio = entityTransform.DistanceToSquared(arrival.startPoint) / startDistanceSquared;
                float speed = arrivalSpeedCurve.Evaluate(Mathf.Clamp01(distRatio)) * hyperspaceEnterSpeed;
                entityTransform.position += (entityTransform.forward * speed) * deltaTime;
                float distSqr = entityTransform.DistanceToSquared(arrival.startPoint);
                if (distSqr >= arrivalThresholdSquared) {
                    Trigger(new Evt_EntityArrived(arrival.entityId));
                }

                transformInfos[arrival.entityId] = entityTransform;
            }
        }

        private void TickDepartures() {
            List<TransformInfo> transformInfos = GameData.Instance.transformInfoMap;
            float endDistanceSquared = departureHyperspaceDistance * departureHyperspaceDistance;

            float deltaTime = GameTimer.Instance.deltaTime;
            int count = departures.Count;
            for (int i = 0; i < count; i++) {
                Departure departure = departures[i];
                TransformInfo entityTransform = transformInfos[departure.entityId];

                float distRatio = 1f - (entityTransform.DistanceToSquared(departure.endPoint) / endDistanceSquared);
                // todo -- first accelerate to top speed, then add speed + this computed speed
                float speed = (departureSpeedCurve.Evaluate(Mathf.Clamp01(distRatio)) * hyperspaceExitSpeed);
                entityTransform.position += (entityTransform.forward * speed) * deltaTime;
                Vector3 toTarget = (departure.endPoint - entityTransform.position).normalized;
                if ((Vector3.Dot(toTarget, entityTransform.forward) < 0)) {
                    Trigger(new Evt_EntityDeparted(departure.entityId));
                }

                transformInfos[departure.entityId] = entityTransform;
            }
        }

        private void OnEntityArriving(Evt_EntityArriving evt) {
            TransformInfo transformInfo = GameData.Instance.transformInfoMap[evt.entityId];
            Vector3 position = transformInfo.position;
            Vector3 offset = (-transformInfo.forward * arrivalHyperspaceDistance);
            arrivals.Add(new Arrival(evt.entityId, position + offset));
            GameData.Instance.transformInfoMap[evt.entityId] = new TransformInfo(
                transformInfo.entityId,
                position + offset,
                transformInfo.rotation
            );
        }

        private void OnEntityArrived(Evt_EntityArrived evt) {
            int idx = arrivals.FindIndex((arrival) => arrival.entityId == evt.entityId);
            if (idx >= 0) arrivals.RemoveAt(idx);
            actives.Add(evt.entityId);
        }

        private void OnEntityDeparting(Evt_EntityDeparting evt) {
            int idx = actives.IndexOf(evt.entityId);
            if (idx >= 0) actives.RemoveAt(idx);

            TransformInfo transformInfo = GameData.Instance.transformInfoMap[evt.entityId];
            Vector3 offset = (transformInfo.forward * departureHyperspaceDistance);
            departures.Add(new Departure(evt.entityId, transformInfo.position + offset));
        }

        private void OnEntityDeparted(Evt_EntityDeparted evt) {
            int idx = departures.FindIndex((departure) => departure.entityId == evt.entityId);
            if (idx >= 0) departures.RemoveAt(idx);
        }

        private struct Departure {

            public readonly int entityId;
            public readonly Vector3 endPoint;

            public Departure(int entityId, Vector3 endPoint) {
                this.entityId = entityId;
                this.endPoint = endPoint;
            }

        }

        private struct Arrival {

            public readonly int entityId;
            public readonly Vector3 startPoint;

            public Arrival(int entityId, Vector3 startPoint) {
                this.entityId = entityId;
                this.startPoint = startPoint;
            }

        }

    }

}