using System;
using System.Collections.Generic;
using SpaceGame.Events;
using SpaceGame.Weapons;
using UnityEngine;

namespace SpaceGame.Engine {

    public class WeaponManager : MonoBehaviour {

        private List<FiringParameters>[] weaponLists;
        private WeaponHandler[] handlers;

        public LinearProjectileDefinition vulcanData;
        public LinearProjectileDefinition pulseLaserData;
        
        public void Initialize() {
            EventSystem.Instance.AddListener<Evt_WeaponFired>(OnWeaponFired);

            int weaponCount = Enum.GetNames(typeof(WeaponType)).Length;
            handlers = new WeaponHandler[weaponCount];
            weaponLists = new List<FiringParameters>[weaponCount];
            
            for (int i = 0; i < weaponLists.Length; i++) {
                weaponLists[i] = new List<FiringParameters>();
            }
            
            handlers[(int)WeaponType.Vulcan] = new LinearProjectileHandler(transform, vulcanData);
           // handlers[(int)WeaponType.PulseLaser] = new LinearProjectileHandler(transform, pulseLaserData);
            
        }

        public void Tick() {
            for (int i = 0; i < weaponLists.Length; i++) {
                List<FiringParameters> list = weaponLists[i];
                if (list.Count > 0) {
                    handlers[(int)list[0].weaponType].Spawn(list);
                    list.Clear();
                }
            }
            for (int i = 0; i < handlers.Length; i++) {
                if (handlers[i] != null) {
                    handlers[i].Tick();
                }
            }
        }

        private void OnWeaponFired(Evt_WeaponFired evt) {
            weaponLists[(int)evt.parameters.weaponType].Add(evt.parameters);
        }

        public void Dispose() {
            EventSystem.Instance.RemoveListener<Evt_WeaponFired>(OnWeaponFired);
        }

    }

}