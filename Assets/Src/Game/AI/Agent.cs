using System;
using System.Collections;
using System.Collections.Generic;
using Lib.Util;
using SpaceGame.AI.Behaviors;
using SpaceGame.Weapons;
using Src.Engine;
using UnityEngine;
using Weichx.Persistence;
using Weichx.Util;

/*
    Goals get prioritized based on mission status

// High level -> Refreshed when goal no longer valid (ie completed, failed, target destroyed, whatever)
    Or after some time period (10 - 30 seconds)
    important events such as entity destroyed, entity arrived, entity docked, missile launch have a chance of refreshing
    goals can specifiy which events they care about. Goals can have a tick function that demands interrupts?

    for each Goal =>
        if agent has behavior that can satisfy that goal (this is a simple boolean: attack goal -> attack behavior good, dock goal -> no dock behavior, bad)
            prioritize the goal (score it)

//Still High level
    for each scored goal =>
        apply behavioral weights

    pick the highest
    HighLevelOutput = Goal (singular)

//Mid Level - Refreshed occasionally, every couple seconds maybe
    for each behavior that can satisfy the chosen goal
    score it vs every context in goal.getcontext()
    intermediate output is Tuple<Score, Behavior, Context>

    pick the highest
    MidLevel output = Pair<Behavior, Context>

//Low level - Every Time an action completes
    for each action in the selected behavior
    score it against the context

    pick the highest

    run it.*/
namespace SpaceGame.AI {

    public class Agent : MonoBehaviour {

        public string serializedData;
        public AgentData agentData;

        public ListX<Goal> goals;
        public Transform target;
        private FlightController flightController;

        private Goal activeGoal;
        private AIAction activeAction;
        private AIBehavior activeBehavior;
        private DecisionContext activeContext;

        private void Start() {
            agentData = Snapshot<AgentData>.Deserialize(serializedData);
            flightController = GetComponent<FlightController>();
        }

        private Entity entity;
        public ListX<AIBehavior> behaviors;

        private int behaviorCompatibiltyFlags;

        private static GoalType[] s_goalTypes;

        private static GoalType[] GetGoalTypes() {
            if (s_goalTypes != null) return s_goalTypes;
            IList goalTypesRaw = Enum.GetValues(typeof(GoalType));
            s_goalTypes = new GoalType[goalTypesRaw.Count];
            for (int i = 0; i < goalTypesRaw.Count; i++) {
                s_goalTypes[i] = (GoalType) goalTypesRaw[i];
            }
            return s_goalTypes;
        }

        private void UpdateCompatFlags() {
            behaviorCompatibiltyFlags = 0;
            for (int i = 0; i < behaviors.Count; i++) {
                for (int j = 0; j < s_goalTypes.Length; j++) {
                    bool canSatisfy = behaviors[i].CanSatisfyGoalType(s_goalTypes[j]);
                    behaviorCompatibiltyFlags |= canSatisfy ? (int) s_goalTypes[j] : 0;
                }
            }
        }

        public void HighLevelUpdate() {
            float maxScore = float.MinValue;
            activeGoal = null;
            for (int i = 0; i < goals.Count; i++) {
                Goal goal = goals[i];
                if ((behaviorCompatibiltyFlags & (int) goal.goalType) == 0) {
                    continue;
                }
                DecisionContext unused_bestContext;
                IReadonlyListX<DecisionContext> contexts = goal.GetEvaluationContexts(entity);
                float score = AIUtil.Score(contexts, goal.considerations, maxScore, out unused_bestContext);
                // todo personality skew the score
                if (score > maxScore) {
                    maxScore = score;
                    activeGoal = goal;
                }

            }

        }

        public void MidLevelUpdate() {
            IReadonlyListX<DecisionContext> goalContexts = activeGoal.GetExecutionContexts(entity);
            float maxScore = float.MinValue;
            activeContext = null;
            activeBehavior = null;
            for (int i = 0; i < behaviors.Count; i++) {
                AIBehavior behavior = behaviors[i];
                if (!behavior.CanSatisfyGoalType(activeGoal.goalType)) {
                    continue;
                }
                DecisionContext context;
                float score = AIUtil.Score(goalContexts, behavior.considerations, maxScore, out context);
                if (score > maxScore) {
                    maxScore = score;
                    activeBehavior = behavior;
                    activeContext = context;
                }

            }
        }

        private void LowLevelUpdate() {

            //todo -- if time elapsed || activeAction.IsComplete;

            AIAction[] actions = activeBehavior.actions;

            activeAction = null;
            float maxScore = float.MinValue;
            int length = actions.Length;
            for (int i = 0; i < length; i++) {
                AIAction action = actions[i];
                float score = action.Score(activeContext, maxScore);
                if (score > maxScore) {
                    maxScore = score;
                    activeAction = action;
                }
            }

        }

        private void Update() {
//            float deltaTime = GameTimer.Instance.deltaTime;
//
//            Vector3 direction = transform.position.DirectionTo(target.transform.position);
//            flightController.SetTargetDirection(direction);
//
//            Quaternion rotation = PropulsionUtil.RotateTowardsDirection(
//                transform.rotation,
//                transform.position.DirectionTo(flightController.targetPosition),
//                flightController.turnRate,
//                deltaTime
//            );
//
//            transform.position += rotation.GetForward() * flightController.currentSpeed * deltaTime;
//            transform.rotation = rotation;
//            GetComponent<WeaponSystemComponent>().Fire();
        }

    }

    public class AgentData {

        public float obedience;
        public float aggression;

        public Decision[] decisions;

    }

}