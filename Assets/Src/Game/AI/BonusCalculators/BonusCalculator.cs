using UnityEngine;

namespace SpaceGame.AI {

    public abstract class BonusCalculator {}
    
    public class BonusCalculator<TContext> : BonusCalculator where TContext : DecisionContext {

        public virtual float GetBonus(TContext context) {
            Debug.Log("Generic Bonus");
            return 0;
        }

    }

}