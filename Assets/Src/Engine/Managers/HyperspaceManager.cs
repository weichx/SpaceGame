using System.Collections.Generic;
using SpaceGame.AI;
using SpaceGame.Events;
using UnityEngine;
using Weichx.Util;

namespace SpaceGame.Engine {

    public class HyperspaceManager : ManagerBase {

        public ResponseCurve speedCurve; // exp = 0.42, invert = true;
        public float hyperspaceDistance = 3000f;
        public float hyperspaceEnterSpeed = 1500f;
        public float arrivedThreshold = 2800f;
        private List<Arrival> arrivals;

        private struct Arrival {

            public readonly int entityId;
            public readonly Vector3 startPoint;
            public readonly Transform transform;
            
            public Arrival(Entity entity, Vector3 startPoint) {
                this.entityId = entity.index;
                this.startPoint = startPoint;
                this.transform = entity.transform;
            }
            
        }

        public void Initialize() {
            arrivals = new List<Arrival>();
            AddListener<Evt_EntityArriving>(OnEntityArrivalStart);
            AddListener<Evt_EntityArrived>(OnEntityArrived);
        }

        public void Tick() {
            float startDistanceSquared = hyperspaceDistance * hyperspaceDistance;
            for (int i = 0; i < arrivals.Count; i++) {
                Arrival arrival = arrivals[i];
                Transform entityTransform = arrival.transform;
                float distRatio = entityTransform.DistanceToSquared(arrival.startPoint) / startDistanceSquared;
                float speed = speedCurve.Evaluate(Mathf.Clamp01(distRatio));
                entityTransform.position += (entityTransform.forward * hyperspaceEnterSpeed * speed) * GameTimer.Instance.deltaTime;
                float distSqr = entityTransform.DistanceToSquared(arrival.startPoint);
                if (distSqr >= arrivedThreshold * arrivedThreshold) {
                    Trigger(new Evt_EntityArrived(arrival.entityId));
                }
            }
        }
        
        private void OnEntityArrivalStart(Evt_EntityArriving evt) {
            Entity entity = EntityDatabase.GetEntityById(evt.entityId);
            Transform entityTransform = entity.transform;
            Vector3 startPoint = entityTransform.position + (entityTransform.forward * -hyperspaceDistance);
            entityTransform.position = startPoint;
            arrivals.Add(new Arrival(entity, startPoint));
        }

        private void OnEntityArrived(Evt_EntityArrived evt) {
            Debug.Log("Entity Arrived");
            int idx = arrivals.FindIndex((a) => a.entityId == evt.entityId);
            if (idx >= 0) {
                arrivals.RemoveAt(idx);
            }
        }
    }

}