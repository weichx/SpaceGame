using UnityEngine;

namespace SpaceGame.Events {

    public class Evt_WeaponImpact : GameEvent {

        public readonly int shooterId;
        public readonly RaycastHit raycastHit;
        
        public Evt_WeaponImpact(int shooterId, RaycastHit raycastHit) {
            this.shooterId = shooterId;
            this.raycastHit = raycastHit;
        }

    }

}