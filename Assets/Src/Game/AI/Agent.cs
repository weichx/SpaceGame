using System.Diagnostics;
using SpaceGame.AI.Behaviors;
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

    public class Agent {

        public ListX<Goal> goals;

        public Goal activeGoal;
        public AIAction activeAction;
        public AIBehavior activeBehavior;
        public DecisionContext activeContext;

        public ListX<AIBehavior> behaviors;
        public ListX<AIBehavior> goalRelevantBehaviors;

        public Entity entity;
        public int behaviorCompatibiltyFlags;

        public float lastGoalDecision;
        public float lastBehaviorDecision;
        
        public Agent(Entity entity) {
            this.entity = entity;
        }
        
        public void DecideGoal() {
            lastGoalDecision = GameTimer.Instance.GetFrameTimestamp();
            float maxScore = float.MinValue;
            activeGoal = null;
            for (int i = 0; i < goals.Count; i++) {
                Goal goal = goals[i];
                if ((behaviorCompatibiltyFlags & (int) goal.goalType) == 0) {
                  //  continue;
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

        public void DecideGoalBehavior() {
            lastBehaviorDecision = GameTimer.Instance.GetFrameTimestamp();
            IReadonlyListX<DecisionContext> goalContexts = activeGoal.GetExecutionContexts(entity);
            float maxScore = float.MinValue;
            activeContext = null;
            activeBehavior = null;
            for (int i = 0; i < behaviors.Count; i++) {
                AIBehavior behavior = behaviors[i];
                if (!behavior.CanSatisfyGoalType(activeGoal.goalType)) {
                //    continue;
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

        public void DecideAction() {

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

            Debug.Assert(activeAction != null, nameof(activeAction) + " != null");
            activeAction.SetContext(activeContext);
            activeAction.OnSetup();
        }

    }

}