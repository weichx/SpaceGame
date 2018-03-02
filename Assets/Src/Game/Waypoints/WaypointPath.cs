using System.Collections.Generic;
using SpaceGame.Util;
using UnityEngine;

namespace SpaceGame {

    [ExecuteInEditMode]
    public class WaypointPath : MonoBehaviour {

        [SerializeField] [HideInInspector] public int tagColorIndex = -1;

        public int id;
        public Vector3[] waypoints;

        private List<WaypointTracker> trackers;

        public void Initialize() {
            trackers = new List<WaypointTracker>();
            waypoints = transform.MapChildren((child) => child.position);
            // todo -- maybe destroy children in production?
        }

        public WaypointTracker GetTrackerForEntity(int entityId) {
            for (int i = 0; i < trackers.Count; i++) {
                if (trackers[i].entityId == entityId) return trackers[i];
            }

            WaypointTracker tracker = new WaypointTracker(entityId, this);
            trackers.Add(tracker);
            return tracker;
        }

        public Vector3 this[int key] => waypoints[key];

    }

}