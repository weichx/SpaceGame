using System;
using UnityEngine;

namespace SpaceGame.AI {

    public class WaypointContext : DecisionContext {

        [HideInInspector] [NonSerialized] public readonly WaypointPath path;

        public WaypointContext(Entity agent, WaypointPath path) : base(agent) {
            this.path = path;
        }

    }


}