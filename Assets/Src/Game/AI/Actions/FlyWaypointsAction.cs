using UnityEngine;

namespace SpaceGame.AI {

    public class FlyWaypointsAction : AIAction<WaypointContext> {

        public override bool Tick() {
            Entity agent = context.agent;
            WaypointPath path = context.path;
            Debug.Assert(path != null, nameof(path) + " != null");

            WaypointTracker tracker = path.GetTrackerForEntity(context.agent.id);
            int completedLaps = tracker.CompletedLaps;

            Vector3 waypoint = tracker.CurrentWaypoint;

            float distSquared = agent.transformInfo.DistanceToSquared(waypoint);

            if (distSquared < 20 * 20) {
                tracker.Progress();
            }

            waypoint = tracker.CurrentWaypoint;
            ApproachType approachType = tracker.IsFinalWaypoint ? ApproachType.Arrive : ApproachType.Normal;
            agent.SetTargetPosition(waypoint, approachType);

            return tracker.CompletedLaps != completedLaps;
        }

    }

}