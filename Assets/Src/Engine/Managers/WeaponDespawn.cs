using UnityEngine;

namespace SpaceGame {

    public struct WeaponDespawn {

        public int ownerId;
        public WeaponDespawnType weaponDespawnType;
        public RaycastHit raycastHit;
        public Transform transform;

    }

}