using Src.Engine;
using System.Collections.Generic;
using SpaceGame.AI;
using SpaceGame.Events;
using Weichx.Util;
using UnityEngine;

namespace SpaceGame.Engine {

    public class AIManager : ManagerBase {

        public List<AIInfo> agents;
        public List<AIAction> actionsToSetup;
        public List<AIAction> actionsToTick;
        public List<AIAction> actionsToTeardown;

        public void Initialize() {
            agents = new List<AIInfo>(16);
            actionsToTick = new List<AIAction>(16);
            actionsToSetup = new List<AIAction>(16);
            actionsToTeardown = new List<AIAction>(16);

            AddListener<Evt_EntityBehaviorChanged>(OnEntityBehaviorChanged);
            AddListener<Evt_EntityDeparting>(OnEntityDeparting);
        }

        private void OnEntityBehaviorChanged(Evt_EntityBehaviorChanged evt) {
            AIInfo aiInfo = GameData.Instance.aiInfoMap[evt.entityId];

            if (aiInfo.decisions != null && aiInfo.decisions.Count > 0) {
                agents.Add(aiInfo);
            }

        }

        private void OnEntityDeparting(Evt_EntityDeparting evt) {
            agents.Remove(GameData.Instance.aiInfoMap[evt.entityId]);
        }

        public AIAction Decide(AIInfo aiInfo) {
            float cutoff = float.MinValue;
            aiInfo.lastDecisionTime = GameTimer.Instance.GetFrameTimestamp();

            ScoreResult bestResult = new ScoreResult();
            List<Decision> decisions = aiInfo.decisions;

            for (int i = 0; i < decisions.Count; i++) {
                Decision decision = decisions[i];
                ScoreResult result = decision.evaluator.Score(aiInfo.Entity, decision, cutoff);

                if (result.score > cutoff) {
                    cutoff = result.score;
                    bestResult = result;
                }
            }

            bestResult.action.SetContext(bestResult.context);
            aiInfo.action = bestResult.action;
            return bestResult.action;
        }

        public void Tick() {
            int count = agents.Count;
    //todo need to 'instantiate' decisions. Right now they are all the same references
            for (int i = 0; i < count; i++) {
                AIInfo agent = agents[i];
                agent.decisionDuration = 1f;

                if (GameTimer.Instance.FrameTimeElapsed(agent.decisionDuration, agent.lastDecisionTime)) {
                    if (agent.action != null) {
                        actionsToSetup.Remove(agent.action);
                        actionsToTeardown.Remove(agent.action);
                        actionsToTick.Remove(agent.action);
                        agent.action.Teardown();
                    }

                    AIAction action = Decide(agent);
                    Debug.Assert(action != null, "action != null");
                    actionsToSetup.Add(action);
                }
            }

            count = actionsToSetup.Count;

            for (int i = 0; i < count; i++) {
                actionsToSetup[i].Setup();
                actionsToTick.Add(actionsToSetup[i]);
            }

            actionsToSetup.Clear();
            count = actionsToTick.Count;

            for (int i = 0; i < count; i++) {
                AIAction action = actionsToTick[i];

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