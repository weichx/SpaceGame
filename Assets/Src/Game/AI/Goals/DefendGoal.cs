using System.Collections.Generic;
using JetBrains.Annotations;
using Weichx.Util;

namespace SpaceGame.AI {

    public class DefendGoal : Goal {

        [UsedImplicitly]
        public DefendGoal() { }

        public override IReadonlyListX<DecisionContext> GetEvaluationContexts(Entity agent) {
            throw new System.NotImplementedException();
        }

        public override IReadonlyListX<DecisionContext> GetExecutionContexts(Entity agent) {
            throw new System.NotImplementedException();
        }

    }

}