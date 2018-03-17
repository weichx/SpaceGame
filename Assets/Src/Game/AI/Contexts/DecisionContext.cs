using System;
using UnityEngine;

namespace SpaceGame.AI {

    public class DecisionContext {

        [NonSerialized] [HideInInspector] public readonly Entity agent;

        protected DecisionContext(Entity agent) {
            this.agent = agent;
        }

        public static bool IsCompatible(Type type) {
            return typeof(DecisionContext).IsAssignableFrom(type);
        }

    }

}