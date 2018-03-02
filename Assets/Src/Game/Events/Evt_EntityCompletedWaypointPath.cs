namespace SpaceGame.Events {

    public class Evt_EntityCompletedWaypointPath : EntityEvent {

        public readonly int waypointPathId;
        
        public Evt_EntityCompletedWaypointPath(int entityId, int waypointPathId) : base(entityId) {
            this.waypointPathId = waypointPathId;
        }

    }

}