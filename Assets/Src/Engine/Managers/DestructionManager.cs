using System.Collections.Generic;
using SpaceGame.Events;
using UnityEngine;

namespace SpaceGame.Engine {

    public class DestructionManager : MonoBehaviour {

        public GameObject explosion;
        private List<int> destroyedEntities;
        
        public void Initialize() {
            destroyedEntities = new List<int>();
            EventSystem.Instance.AddListener<Evt_EntityDestroyed>(OnEntityDestroyed);
        }
        
        public void Tick() {
            for (int i = 0; i < destroyedEntities.Count; i++) {
                Entity entity = EntityDatabase.GetEntity(destroyedEntities[i]);
                if (entity != null) {
                    Instantiate(explosion, entity.transform.position, Quaternion.identity);
                    Destroy(entity.gameObject);
                }
            }
        }
        
        private void OnEntityDestroyed(Evt_EntityDestroyed evt) {
            Debug.Log("ENTITY DESTROYED");
            destroyedEntities.Add(evt.entityId);
        }

        private void OnDestroy() {
            EventSystem.Instance.RemoveListener<Evt_EntityDestroyed>(OnEntityDestroyed);
        }

    }

}