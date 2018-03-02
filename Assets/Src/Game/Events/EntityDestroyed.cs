namespace SpaceGame.Events {

    public class Evt_EntityDestroyed : GameEvent {

        public int entityId;
        
        private static readonly Pool<Evt_EntityDestroyed> pool = new Pool<Evt_EntityDestroyed>(10);

        public static Evt_EntityDestroyed Spawn(int entityId) {
            Evt_EntityDestroyed evt = pool.Spawn();
            evt.entityId = entityId;
            return evt;
        }

    }

}