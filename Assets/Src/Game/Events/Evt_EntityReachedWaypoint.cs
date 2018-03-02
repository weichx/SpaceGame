namespace SpaceGame.Events {

    public class Evt_EntityReachedWaypoint : EntityEvent {

        public readonly int waypointIndex;
        public readonly int waypointPathId;

        public Evt_EntityReachedWaypoint(int entityId, int waypointIndex, int waypointPathId) : base(entityId) {
            this.waypointIndex = waypointIndex;
            this.waypointPathId = waypointPathId;
        }

    }

}