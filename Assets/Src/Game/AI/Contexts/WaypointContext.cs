namespace SpaceGame.AI {

    public class WaypointContext : DecisionContext {

        public readonly WaypointPath path;

        public WaypointContext(Entity agent, WaypointPath path) : base(agent) {
            this.path = path;
        }

    }


}