using System.Collections.Generic;
using SpaceGame.Weapons;

namespace SpaceGame {

    public abstract class WeaponHandler {

        public abstract void Spawn(List<FiringParameters> shots);

        public abstract void Tick();

    }

}