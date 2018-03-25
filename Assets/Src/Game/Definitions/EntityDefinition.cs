using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SpaceGame.AI;
using UnityEngine;
using Weichx.ReflectionAttributes;

namespace SpaceGame {
    
    //this allows type safety and a custom inspector
    public struct FactionReference {

        public readonly int factionId;

        public FactionReference(int factionId) {
            this.factionId = factionId;
        }

    }

    [Serializable]
    public class EntityDefinition : AssetDefinition {

        [HideInInspector] public int factionId;
        [HideInInspector] public int flightGroupId;
        [DefaultExpanded, CreateOnReflect] public readonly List<Goal> goals;

        public string callsign;
        public string shipType;

        [UsedImplicitly] private EntityDefinition() { }

        public EntityDefinition(int id) : base(id) {
            this.name = $"Entity {id}";
            this.goals = new List<Goal>(4);
        }

    }

}