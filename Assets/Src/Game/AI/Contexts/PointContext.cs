using UnityEngine;

namespace SpaceGame.AI {

    public class PointContext : DecisionContext {

        public readonly Transform target;
        
        public PointContext(Transform target, Entity agent) : base(agent) {
            this.target = target;
        }

    }

}