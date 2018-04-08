using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SpaceGame.AI;
using SpaceGame.AI.Behaviors;
using SpaceGame.Assets;
using UnityEngine;
using Weichx.ReflectionAttributes;
using Weichx.ReflectionAttributes.Markers;
using Weichx.Util;

namespace SpaceGame {

    //this allows type safety and a custom inspector
    public struct FactionReference {

        public readonly int factionId;

        public FactionReference(int factionId) {
            this.factionId = factionId;
        }

    }

    [Serializable]
    public class EntityDefinition : MissionAsset, INestedAsset<FlightGroupDefinition> {

        public bool prependFactionName;
        public bool prependFlightGroupName;
        public bool inheritFlightGroupNameGenerator;
        
        [UsePropertyDrawer(typeof(SceneEntitySelector))]
        public int sceneEntityId;

        [HideInInspector] public int factionId;
        [HideInInspector] public int flightGroupId;
        [DefaultExpanded, CreateOnReflect] public readonly List<Goal> goals;

        [UsedImplicitly]
        public bool isTemplate; // don't show errors about missing sceneEntityId

        [UsedImplicitly] [UsePropertyDrawer(typeof(ShipTypeSelector))]
        public string chassisGuid;

        [DefaultExpanded, CreateOnReflect]
        public ListX<BehaviorSet> behaviorSets;

        [DefaultExpanded, CreateOnReflect] public ListX<AIBehavior> standaloneBehaviors;
        
        [UsedImplicitly]
        private EntityDefinition() { }

        private EntityDefinition(int id, string name) : base(id, name) {
            this.goals = new List<Goal>(4);
            behaviorSets = new ListX<BehaviorSet>();
            standaloneBehaviors = new ListX<AIBehavior>();
        }

        public bool IsLinked => sceneEntityId > 0;
        
        public override string DisplayName {
            get {
                if (!isTemplate && sceneEntityId <= 0) {
                    return $"[Unlinked] {name}";
                }
                return name;
            }
        }

        public void SetParentAsset(FlightGroupDefinition asset, int index = -1) {
            asset.AddEntity(this, index);
        }

        public void SetSiblingIndex(int index) {
            FlightGroupDefinition parent = GameDatabase.ActiveInstance.FindAsset<FlightGroupDefinition>(flightGroupId);
            parent.entities.MoveToIndex(this, index);
        }

    }

}