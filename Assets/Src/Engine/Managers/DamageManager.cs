using System.Collections.Generic;
using SpaceGame.Events;
using UnityEngine;

namespace SpaceGame.Engine {

    public struct DamageParameters {

        public int entityId;
        public float damagePoints;

    }
    
    public class DamageManager : MonoBehaviour {

        private List<DamageParameters> damages;
        
        public void Initialize() {
            damages = new List<DamageParameters>();
            EventSystem.Instance.AddListener<Evt_WeaponImpact>(OnWeaponImpact);
        }

        public void Tick() {
//            for (int i = 0; i < damages.Count; i++) {
//                DamageParameters damage = damages[i];
//                Entity target = EntityDatabase.GetEntity(damage.entityId);
//                
//                if(target == null) continue;
//                
//                // this is super simple for now
//                if (target.hitPoints > 0) {
//                    target.hitPoints -= damage.damagePoints;
//                }
//
//                if (target.hitPoints <= 0) {
//                    EventSystem.Instance.Trigger(Evt_EntityDestroyed.Spawn(target.id));
//                }
//            }
            damages.Clear();
        }
        
        private void OnWeaponImpact(Evt_WeaponImpact evt) {
            DamageParameters parameters;
            parameters.entityId = evt.shooterId;
            parameters.damagePoints = 10f;
            damages.Add(parameters);
        }

        private void OnDestroy() {
            EventSystem.Instance.RemoveListener<Evt_WeaponImpact>(OnWeaponImpact);
        }

    }

}