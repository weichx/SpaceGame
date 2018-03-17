using System;
using SpaceGame.Events;
using Weichx.Util;
using SpaceGame.Weapons;
using UnityEngine;

namespace SpaceGame.Systems {

    [Flags]
    public enum LinkCapability { }

    public enum FireMode {

        Single, Linked, Burst

    }

    public class WeaponData {

        public float raycastMultiplier;
        public float aspectLockTime;
        public float aspectLockTolerance;
        public float hullSpashDamage;
        public float shieldSplashDamage;
        public float systemSplashDamage;
        public float systemDamage;
        public float hullDamage;
        public float shieldDamage;
        public float accelerationRate;
        public float turnRate;
        public float startSpeed;
        public float maxSpeed;
        public float range;
        public float lifeTime;

    }

    public class WeaponGroup {

        public int activeGunIndex;
        public float lastFiredTimestamp;
        public float timeToNextShot;
        public FireMode fireMode;
        public WeaponType weaponType;
        public int ammunition;
        public PositionDirection[] gunpoints;

    }
    
    public class WeaponSystem : System {

        public WeaponGroup[] weaponGroups;
        public WeaponAssetGroup weaponAssetGroup;
        
        // Loadout
        // Ammunition
        // Linked Mode
        // Fire Functionality
        // Fire Capability

        private void Fire() {
//            if (Timer.GetTimeStamp() - lastFireTime >= fireRate) {
//                lastFireTime = Timer.GetTimeStamp();
//                Vector3 point = transform.position + gunpoints[gunIndex].point;
//                Vector3 direction = transform.rotation * gunpoints[gunIndex].normal;
//                gunIndex = (gunIndex + 1) % gunpoints.Length;
//                FiringParameters parameters = new FiringParameters();
//                parameters.weaponType = WeaponType.Vulcan;
//                parameters.direction = direction;
//                parameters.ownerId = entity.id;
//                parameters.position = point;
//                EventSystem.Instance.Trigger(Evt_WeaponFired.Spawn(parameters));
//            }
        }
    }

}