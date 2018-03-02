using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceGame {

    /*
     * Gather all Entity Components in immediate children and treat them as a group.
     */
    public class EntityGroup : MonoBehaviour {

        [SerializeField] public List<Entity> entities;

        public void Awake() {
            
            entities = new List<Entity>(GetComponentsInChildren<Entity>());
            // if entity starts disabled, we still want to register it with the entity database
            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];
                EntityDatabase.AddEntity(entity);
            }
        }
        
        public Entity GetEntity(string name) {
            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];
                if (entity.name == name) {
                    return entity;
                }
            }

            return null;
        }

    }

}