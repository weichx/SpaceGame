using System;
using System.Collections;
using SpaceGame.AI.Behaviors;
using SpaceGame.Engine;
using UnityEngine;
using Weichx.Util;

namespace SpaceGame.AI {

    public class IntelligenceSystem {

        public GameData gd;

        public ListX<Agent> agents;
        public ListX<Agent> agentsNeedingGoal;
        public ListX<Agent> agentsNeedingAction;
        public ListX<Agent> agentsNeedingBehavior;

        private GoalType[] goalTypes;

        public void Initialize() {
            InitGoalTypes();
            agents = new ListX<Agent>();
            agentsNeedingGoal = new ListX<Agent>();
            agentsNeedingAction = new ListX<Agent>();
            agentsNeedingBehavior = new ListX<Agent>();
        }

        private void InitGoalTypes() {
            IList goalTypesRaw = Enum.GetValues(typeof(GoalType));
            goalTypes = new GoalType[goalTypesRaw.Count];
            for (int i = 0; i < goalTypesRaw.Count; i++) {
                goalTypes[i] = (GoalType) goalTypesRaw[i];
            }
        }

        public void CreateAgent(Entity entity, EntityDefinition entityDefinition) {
            Agent agent = new Agent(entity);
            agent.goals = new ListX<Goal>(entityDefinition.goals);
            agent.behaviors = entityDefinition.standaloneBehaviors ?? new ListX<AIBehavior>();
            agent.behaviorCompatibiltyFlags = -1;
            agents.Add(agent);
            agentsNeedingGoal.Add(agent);
        }

        private int UpdateCompatFlags(ListX<AIBehavior> behaviors) {
            int behaviorCompatibiltyFlags = 0;
            for (int i = 0; i < behaviors.Count; i++) {
                for (int j = 0; j < goalTypes.Length; j++) {
                    bool canSatisfy = behaviors[i].CanSatisfyGoalType(goalTypes[j]);
                    behaviorCompatibiltyFlags |= canSatisfy ? (int) goalTypes[j] : 0;
                }
            }
            return behaviorCompatibiltyFlags;
        }

        public void Tick() {

            int highLevelCount = agentsNeedingGoal.Count;
            Agent[] highLevelRaw = agentsNeedingGoal.RawArray;

            for (int i = 0; i < highLevelCount; i++) {
                Agent agent = highLevelRaw[i];
                agent.DecideGoal();
                agentsNeedingBehavior.Add(agent);
            }

            agentsNeedingGoal.Clear();

            int midLevelCount = agentsNeedingBehavior.Count;
            Agent[] midLevelRaw = agentsNeedingBehavior.RawArray;
            for (int i = 0; i < midLevelCount; i++) {
                midLevelRaw[i].DecideGoalBehavior();
                agentsNeedingAction.Add(midLevelRaw[i]);
            }

            agentsNeedingBehavior.Clear();

            int lowLevelCount = agentsNeedingAction.Count;
            Agent[] lowLevelRaw = agentsNeedingAction.RawArray;
            for (int i = 0; i < lowLevelCount; i++) {
                lowLevelRaw[i].DecideAction();
            }

            agentsNeedingAction.Clear();

            for (int i = 0; i < agents.Count; i++) {
                Agent agent = agents[i];

                bool isCompleted = agent.activeAction.Tick();

                if (GameTimer.Instance.FrameTimeElapsed(agent.lastGoalDecision, 5f)) {
                    agentsNeedingGoal.Add(agent);
                    agent.activeAction.Teardown();
                }
                else if (GameTimer.Instance.FrameTimeElapsed(agent.lastBehaviorDecision, 2.5f)) {
                    agentsNeedingBehavior.Add(agent);
                    agent.activeAction.Teardown();
                }
                else if (isCompleted) {
                    agentsNeedingAction.Add(agent);
                    agent.activeAction.Teardown();
                }
            }
        }

    }

}