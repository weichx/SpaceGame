using System.Collections.Generic;
using JetBrains.Annotations;
using SpaceGame.AI;
using UnityEngine;
using Weichx.ReflectionAttributes;

namespace SpaceGame {

    public class FlightGroupDefinition : MissionAsset, IParentAsset, INestedAsset<FactionDefinition> {

        [HideInInspector] public int factionId;

        [DefaultExpanded, CreateOnReflect] public readonly List<Goal> goals;
        [DefaultExpanded, CreateOnReflect] public readonly List<EntityDefinition> entities;

        [UsedImplicitly]
        public FlightGroupDefinition() { }

        public FlightGroupDefinition(int id, string name) : base(id, name) {
            this.entities = new List<EntityDefinition>();
            this.goals = new List<Goal>();
        }

        public void AddEntity(EntityDefinition entity, int index = -1) {
            entity.factionId = factionId;
            entity.flightGroupId = id;
            if (index == -1) {
                entities.Add(entity);
            }
            else {
                entities.Insert(index, entity);
            }
        }

        public void RemoveEntity(EntityDefinition entity) {
            if (entities.Remove(entity)) {
                entity.factionId = -1;
                entity.flightGroupId = -1;
            }
        }

        public void SetParentAsset(FactionDefinition asset, int siblingIndex = -1) {
            throw new System.NotImplementedException();
        }

        public void SetSiblingIndex(int index) {
            throw new System.NotImplementedException();
        }

    }

    

}