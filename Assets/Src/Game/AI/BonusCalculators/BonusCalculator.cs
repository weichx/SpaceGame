using System;
using UnityEngine;

namespace SpaceGame.AI {

    public abstract class BonusCalculator : IContextAware {

        public abstract Type GetContextType();

    }
    
    public class BonusCalculator<TContext> : BonusCalculator where TContext : DecisionContext {

        public virtual float GetBonus(TContext context) {
            return 0;
        }

        public override Type GetContextType() {
            return typeof(TContext);
        }

    }

}