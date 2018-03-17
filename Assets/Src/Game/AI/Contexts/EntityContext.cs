﻿using System;
using UnityEngine;

namespace SpaceGame.AI {

    public class EntityContext : DecisionContext {

        [HideInInspector][NonSerialized] public readonly Entity other;

        protected EntityContext(Entity agent, Entity other) : base(agent) {
            this.other = other;
        }

    }

}