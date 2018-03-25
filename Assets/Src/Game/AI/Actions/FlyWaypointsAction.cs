using UnityEngine;

namespace SpaceGame.AI {

    public class FlyWaypointsAction : AIAction<WaypointContext> {

        public override void OnSetup() {
            Debug.Log("Setup");
        }
        
        public override bool Tick() {
            Entity agent = context.agent;
            WaypointPath path = context.path;
            Debug.Assert(path != null, nameof(path) + " != null");

            WaypointTracker tracker = path.GetTrackerForEntity(context.agent.index);
            int completedLaps = tracker.CompletedLaps;

            Vector3 waypoint = tracker.CurrentWaypoint;

            float distSquared = agent.transformInfo.DistanceToSquared(waypoint);

            if (distSquared < 20 * 20) { // todo replace with a radius per waypoint
                tracker.Progress();
            }

            waypoint = tracker.CurrentWaypoint;
            ApproachType approachType = tracker.IsFinalWaypoint ? ApproachType.Arrive : ApproachType.Normal;
            agent.FlightSystem.SetTargetPosition(waypoint, approachType);
            return tracker.CompletedLaps != completedLaps;
        }

        public override void Teardown() {
            Debug.Log("Teardown");
        }

    }

}