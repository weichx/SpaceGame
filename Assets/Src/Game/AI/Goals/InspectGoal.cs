using System.Collections.Generic;
using JetBrains.Annotations;
using Lib.Util;

namespace SpaceGame.AI {

    public class InspectGoal : Goal {

        [UsedImplicitly]
        public InspectGoal() { }

        public override IReadonlyListX<DecisionContext> GetEvaluationContexts(Entity agent) {
            throw new System.NotImplementedException();
        }

        public override IReadonlyListX<DecisionContext> GetExecutionContexts(Entity agent) {
            throw new System.NotImplementedException();
        }

    }

}