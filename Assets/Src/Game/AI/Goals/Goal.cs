using System.Collections.Generic;
using JetBrains.Annotations;
using Lib.Util;
using UnityEngine;
using Weichx.ReflectionAttributes;

namespace SpaceGame.AI {

    public abstract class Goal {

        [HideInInspector] public int id;
        [HideInInspector] public string name;
        [HideInInspector] public GoalLevel goalLevel;
        [HideInInspector] public GoalType goalType;

        public float priority = 1;

        [UsedImplicitly] [CreateOnReflect] [DefaultExpanded]
        public Consideration[] considerations;

        [UsedImplicitly]
        protected Goal() { }

        //contexts in which this goal is prioritized against other goals
        public abstract IReadonlyListX<DecisionContext> GetEvaluationContexts(Entity agent);

        //contexts in which this goal is actually executed by behaviors
        public abstract IReadonlyListX<DecisionContext> GetExecutionContexts(Entity agent);
        
    }

}