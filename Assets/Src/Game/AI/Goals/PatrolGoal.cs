using JetBrains.Annotations;
using Weichx.Util;

namespace SpaceGame.AI {

    public class PatrolGoal : Goal {

        [UsedImplicitly]
        public PatrolGoal() { }

        public override IReadonlyListX<DecisionContext> GetEvaluationContexts(Entity agent) {
            throw new System.NotImplementedException();
        }

        public override IReadonlyListX<DecisionContext> GetExecutionContexts(Entity agent) {
            throw new System.NotImplementedException();
        }

    }

}