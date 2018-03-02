using SpaceGame.Weapons;

namespace SpaceGame.Events {

    public class Evt_WeaponFired : GameEvent {

        private static readonly Pool<Evt_WeaponFired> pool = new Pool<Evt_WeaponFired>(100);

        public FiringParameters parameters;
        
        public static Evt_WeaponFired Spawn(FiringParameters parameters) {
            Evt_WeaponFired evt = pool.Spawn();
            evt.parameters = parameters;
            return evt;
        }

    }

}