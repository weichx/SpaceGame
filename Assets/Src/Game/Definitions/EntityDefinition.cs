using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SpaceGame.AI;
using UnityEngine;
using Weichx.ReflectionAttributes;
using Weichx.ReflectionAttributes.Markers;

namespace SpaceGame {

    //this allows type safety and a custom inspector
    public struct FactionReference {

        public readonly int factionId;

        public FactionReference(int factionId) {
            this.factionId = factionId;
        }

    }

    [Serializable]
    public class EntityDefinition : MissionAsset {

        [UsePropertyDrawer(typeof(SceneEntitySelector))]
        public int sceneEntityId;

        [HideInInspector] public int factionId;
        [HideInInspector] public int flightGroupId;
        [DefaultExpanded, CreateOnReflect] public readonly List<Goal> goals;

        [UsedImplicitly]
        public bool isTemplate; // don't show errors about missing sceneEntityId

        [UsedImplicitly]
        [UsePropertyDrawer(typeof(ShipTypeSelector))]
        public int shipTypeId;

        [UsedImplicitly]
        private EntityDefinition() { }

        public EntityDefinition(int id) : base(id) {
            this.name = $"Entity {id}";
            this.goals = new List<Goal>(4);
        }

        public override string DisplayName {
            get {
                if (!isTemplate && sceneEntityId <= 0) {
                    return $"[Not In Scene] {name}";
                }
                return name;
            }
        }

    }

}