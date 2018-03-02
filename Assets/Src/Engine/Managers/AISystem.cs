using System;
using Src.Engine;
using System.Collections.Generic;
using SpaceGame.AI;
using SpaceGame.Events;
using SpaceGame.Util;

namespace SpaceGame.Engine {

    public class AIManager : ManagerBase {

        public List<AIInfo> agents;
        public List<AIAction<DecisionContext>> actionsToSetup;
        public List<AIAction<DecisionContext>> actionsToTick;
        public List<AIAction<DecisionContext>> actionsToTeardown;

        public void Initialize() {
            agents = new List<AIInfo>(16);
            actionsToTick = new List<AIAction<DecisionContext>>(16);
            actionsToSetup = new List<AIAction<DecisionContext>>(16);
            actionsToTeardown = new List<AIAction<DecisionContext>>(16);

            AddListener<Evt_EntityArrived>(OnEntityArrived);
            AddListener<Evt_EntityDeparting>(OnEntityDeparting);
        }

        private void OnEntityArrived(Evt_EntityArrived evt) {
            AIInfo aiInfo = GameData.Instance.aiInfoMap[evt.entityId];

            if (aiInfo.decisions != null && aiInfo.decisions.Length > 0) {
                agents.Add(aiInfo);
            }

        }

        private void OnEntityDeparting(Evt_EntityDeparting evt) {
            agents.Remove(GameData.Instance.aiInfoMap[evt.entityId]);
        }

        public AIAction<DecisionContext> Decide(AIInfo aiInfo) {
            float cutoff = float.MinValue;
            aiInfo.lastDecisionTime = GameTimer.Instance.GetFrameTimestamp();

            ScoreResult<DecisionContext> bestResult = new ScoreResult<DecisionContext>();
            Decision<DecisionContext>[] decisions = aiInfo.decisions;

            for (int i = 0; i < decisions.Length; i++) {
                Decision<DecisionContext> decision = decisions[i];
                ScoreResult<DecisionContext> result = decision.evaluator.Score(decision, cutoff);

                if (result.score > cutoff) {
                    cutoff = result.score;
                    bestResult = result;
                }
            }

            return bestResult.action;
        }

        public void Tick() {
            int count = agents.Count;

            for (int i = 0; i < count; i++) {
                AIInfo agent = agents[i];

                if (GameTimer.Instance.FrameTimeElapsed(agent.decisionDuration, agent.lastDecisionTime)) {
                    actionsToSetup.Add(Decide(agent));
                }
            }

            count = actionsToSetup.Count;

            for (int i = 0; i < count; i++) {
                actionsToSetup[i].Setup();
                actionsToTick.Add(actionsToTick[i]);
            }

            actionsToSetup.Clear();
            count = actionsToTick.Count;

            for (int i = 0; i < count; i++) {
                AIAction<DecisionContext> action = actionsToTick[i];

                if (action.Tick()) {
                    // todo use double buffer instead of remove
                    actionsToTick.RemoveAt(i);
                    actionsToTeardown.Add(action);
                    i--;
                }
            }

            count = actionsToTeardown.Count;

            for (int i = 0; i < count; i++) {
                actionsToTeardown[i].Teardown();
            }

            actionsToTeardown.Clear();

        }

    }

}