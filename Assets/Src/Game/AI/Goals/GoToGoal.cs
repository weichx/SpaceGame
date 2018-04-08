using Weichx.Util;
using UnityEngine;

namespace SpaceGame.AI {

    public class GoToGoal : Goal {

        public override IReadonlyListX<DecisionContext> GetEvaluationContexts(Entity agent) {
            return new ListX<DecisionContext>() {
                new PointContext(Object.FindObjectOfType<WaypointSpawner>().transform, agent)
            };
        }

        public override IReadonlyListX<DecisionContext> GetExecutionContexts(Entity agent) {
            return new ListX<DecisionContext>() {
                new PointContext(Object.FindObjectOfType<WaypointSpawner>().transform, agent)
            };
        }

    }

}