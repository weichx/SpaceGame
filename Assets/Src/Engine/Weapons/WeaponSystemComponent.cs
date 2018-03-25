using SpaceGame.Events;
using UnityEngine;
using Weichx.Util;

namespace SpaceGame.Weapons {

    internal struct PositionNormal {

        public readonly Vector3 point;
        public readonly Vector3 normal;

        public PositionNormal(Vector3 point, Vector3 normal) {
            this.point = point;
            this.normal = normal;
        }

    }

    public class WeaponSystemComponent : MonoBehaviour {

        public LinearProjectileDefinition weaponAsset;

        public float fireRate = 0.33f;

        private int gunIndex;
        private Entity entity;
        private float lastFireTime;
        private PositionNormal[] gunpoints;

        private void Awake() {
            entity = GetComponent<Entity>();
            Transform gunpointRoot = transform.Find("Gun Points");
            gunpoints = new PositionNormal[gunpointRoot.childCount];
            for (int i = 0; i < gunpointRoot.childCount; i++) {
                Transform childGunpoint = gunpointRoot.GetChild(i);
                gunpoints[i] = new PositionNormal(childGunpoint.localPosition, childGunpoint.forward);
            }

        }

        public void Fire() {
            if (GameTimer.Instance.RealTimeElapsed(fireRate, lastFireTime)) {
                lastFireTime = GameTimer.Instance.GetRealTimestamp();
                Vector3 point = transform.position + gunpoints[gunIndex].point;
                Vector3 direction = transform.rotation * gunpoints[gunIndex].normal;
                gunIndex = (gunIndex + 1) % gunpoints.Length;
                FiringParameters parameters = new FiringParameters();
                parameters.weaponType = WeaponType.Vulcan;
                parameters.direction = direction;
                parameters.ownerId = entity.index;
                parameters.position = point;
                EventSystem.Instance.Trigger(Evt_WeaponFired.Spawn(parameters));
            }
        }

    }

}