using System;

namespace SpaceGame {

    [Flags]
    public enum PhysicsLayer {

        Entity,
        Ambient,
        Weapon,
        WeaponBroadPhase

    }

}