namespace SpaceGame.AI {

    public class AIInfo {

        public int entityId;
        public float lastDecisionTime;
        public float decisionDuration;
        public AIAction<DecisionContext> action;
        public Decision<DecisionContext>[] decisions;

        // todo add an idle behavior as baseline
        public AIInfo(int entityId) {
            this.entityId = entityId;
            this.lastDecisionTime = 0;
            this.decisionDuration = 0;
            this.action = null;
            this.decisions = null;
        }
        
        // todo make this not suck
        // todo make Decisions a template that can be cloned
        // todo give Decisions a counter so multiple behavior sets can add and remove 
        // todo give Decisions a unique id so multiple behavior sets can add and remove 
        public void AddBehaviors(BehaviorSet behaviorSet) {
            decisions = new Decision<DecisionContext>[behaviorSet.decisions.Length];

            for (int i = 0; i < decisions.Length; i++) {
                decisions[i] = behaviorSet.decisions[i];
            }
        }

         
        // todo use this for decision debugging 
        public class LoggedAction {

            public int entityId;
            public float duration;
            public DecisionContext context;
            public AIAction<DecisionContext> action;

        }
    }


}