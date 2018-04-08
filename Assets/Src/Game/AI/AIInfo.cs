using System.Collections.Generic;
using SpaceGame.Engine;

namespace SpaceGame.AI {

    public class AIInfo {

        public int entityId;
        public float lastDecisionTime;
        public float decisionDuration;
        public AIAction action;
        public List<Decision> decisions;

        public AIInfo(int entityId) {
            this.entityId = entityId;
            this.lastDecisionTime = 0;
            this.decisionDuration = 0;
            this.action = null;
            this.decisions = null;
        }

        public Entity Entity {
            get { return GameData.Instance.entityMap[entityId]; }
        }

    }

}