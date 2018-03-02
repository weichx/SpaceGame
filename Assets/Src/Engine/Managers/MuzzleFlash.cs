using UnityEngine;

namespace SpaceGame {

    public partial class LinearProjectileHandler {

        private struct MuzzleFlash {

            public float lifeTime;
            public GameObject gameObject;

            public MuzzleFlash(GameObject gameObject) {
                this.lifeTime = 0;
                this.gameObject = gameObject;
            }

        }

    }

}