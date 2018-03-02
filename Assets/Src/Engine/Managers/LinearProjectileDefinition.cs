using UnityEngine;

namespace SpaceGame {

    [CreateAssetMenu(
        fileName = "LinearProjectile", 
        menuName = "Game/Create LinearProjectile ", 
        order = 1
    )]
    public class LinearProjectileDefinition : ScriptableObject {

        public float maxLifeTime = 5f;
        public float raycastAdvance = 2f;
        public float projectileSpeed = 20f;
        public float muzzleFlashDuration = 0.1f;

        public GameObject muzzleFlash;
        public GameObject projectile;
        public GameObject impact;
        public AudioClip fireSound;
        public AudioClip impactSound;
        
    }

}