using System.Collections.Generic;
using JetBrains.Annotations;
using Weichx.Util;
using Weichx.ReflectionAttributes;
using Weichx.ReflectionAttributes.Markers;

namespace SpaceGame.AI {

    public class AttackGoal : Goal {

        [UsePropertyDrawer(typeof(EntitySelector))]
        public int entityId;

        [UsePropertyDrawer(typeof(EntitySelector))]
        public int flightGroupId;

        private ListX<DecisionContext> decisionContexts;
        
        [UsedImplicitly]
        public AttackGoal() {
            this.decisionContexts = new ListX<DecisionContext>();    
        }

        public override IReadonlyListX<DecisionContext> GetEvaluationContexts(Entity agent) {
            return this.decisionContexts;
        }

        public override IReadonlyListX<DecisionContext> GetExecutionContexts(Entity agent) {
            return this.decisionContexts;
        }

    }

}