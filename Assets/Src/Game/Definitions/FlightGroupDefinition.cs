using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Weichx.Util;

namespace SpaceGame {

    public class FlightGroupDefinition : AssetDefinition {

        [HideInInspector] public readonly List<EntityDefinition> entities;
        [HideInInspector] public int factionId;
        
        [UsedImplicitly]
        public FlightGroupDefinition() {
            this.entities = new List<EntityDefinition>(16);
        }

        public FlightGroupDefinition(string name) : base(name) {
            this.entities = new List<EntityDefinition>(16);
        }
        
        public EntityDefinition AddEntity() {
            return AddEntity(new EntityDefinition());
        }
        
        public EntityDefinition AddEntity(EntityDefinition entity) {
            this.entities.Add(entity);
            entity.factionId = factionId;
            entity.flightGroupId = id;
            return entity;
        }

        public EntityDefinition RemoveEntity(EntityDefinition entity) {
            if (entities.Remove(entity)) {
                entity.factionId = -1;
                entity.flightGroupId = -1;
                return entity;
            }
            return null;
        }

        public bool MoveEntity(EntityDefinition child, int index) {
            if (!entities.Contains(child)) return false;
            return entities.MoveToIndex(child, index);
        }

        public EntityDefinition InsertEntity(EntityDefinition entity, int index) {
            entity.factionId = factionId;
            entity.flightGroupId = id;
            int currentIndex = entities.IndexOf(entity);
            if (currentIndex == -1) {
                entities.Insert(index, entity);
            }
            else {
                entities.MoveToIndex(currentIndex, index);
            }
            
            return entity;
        }

    }

}