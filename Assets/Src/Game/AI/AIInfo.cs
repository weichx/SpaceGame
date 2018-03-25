using System.Collections.Generic;
using SpaceGame.Engine;

namespace SpaceGame.AI {

    public class AIInfo {

        public int entityId;
        public float lastDecisionTime;
        public float decisionDuration;
        public AIAction action;
        public List<Decision> decisions;

        // todo add an idle behavior as baseline
        public AIInfo(int entityId) {
            this.entityId = entityId;
            this.lastDecisionTime = 0;
            this.decisionDuration = 0;
            this.action = null;
            this.decisions = null;
        }

        // todo use this for decision debugging 
        public class LoggedAction {

            public int entityId;
            public float duration;
            public DecisionContext context;
            public AIAction<DecisionContext> action;

        }

        public Entity Entity {
            get { return GameData.Instance.entityMap[entityId]; }
        }

    }

}