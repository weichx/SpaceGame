using SpaceGame.Events;
using UnityEngine;

namespace SpaceGame {

    public class WaypointTracker {

        public readonly int entityId;
        private int currentWaypointIndex;
        public readonly WaypointPath path;

        public WaypointTracker(int entityId, WaypointPath path) {
            this.entityId = entityId;
            this.path = path;
            this.CompletedLaps = 0;
            this.currentWaypointIndex = 0;
        }

        public void Progress() {
            EventSystem.Instance.Trigger(new Evt_EntityReachedWaypoint(entityId, currentWaypointIndex, path.id));
            currentWaypointIndex = (currentWaypointIndex + 1) % path.waypoints.Length;
            ;
            if (currentWaypointIndex == 0) {
                EventSystem.Instance.Trigger(new Evt_EntityCompletedWaypointPath(entityId, path.id));
                CompletedLaps++;
            }
        }

        public float CompletedLapPercentage => ((float)currentWaypointIndex / path.waypoints.Length);

        public Vector3 CurrentWaypoint => path[currentWaypointIndex];

        public int CompletedLaps { get; private set; }

        public bool IsFinalWaypoint => currentWaypointIndex == path.waypoints.Length;

    }

}