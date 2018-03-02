using UnityEngine;

namespace SpaceGame {

    [CreateAssetMenu(fileName = "WeaponType", menuName = "Game/Create New Weapon Type", order = 1)]
    public class WeaponAssetGroup : ScriptableObject {
        
        public GameObject muzzleFlash;
        public GameObject projectile;
        public GameObject impact;
        public AudioClip fireSound;
        public AudioClip impactSound;

    }

}