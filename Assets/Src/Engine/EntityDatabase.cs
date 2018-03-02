using System.Collections.Generic;
using UnityEngine;

namespace SpaceGame {

    public static class EntityDatabase {

        private static readonly List<Entity> entities = new List<Entity>();

        public static void AddEntity(Entity entity) {
            if (!entities.Contains(entity)) {
                entities.Add(entity);
            }
        }

        public static Entity GetEntity(int id) {
            return entities.Find((e) => e.id == id);
        }
        
        public static Entity GetEntityById(int id) {
            return entities.Find((e) => e.id == id);
        }
        
        public static void RemoveEntity(Entity entity) {
            entities.Remove(entity);
        }

        public static List<Entity> FindHostilesInRange(Faction faction, Vector3 position, float range) {
            // todo -- accept list as out parameter
            List<Entity> retn = new List<Entity>();
            range *= range;

            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];

                if (faction.IsHostile(entity.faction)) {
                    if ((entity.transform.position - position).sqrMagnitude < range) {
                        retn.Add(entity);
                    }
                }
            }

            return retn;
        }

    }

}