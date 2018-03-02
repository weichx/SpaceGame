namespace SpaceGame.AI {

    public class BonusCalculator<TContext> where TContext : DecisionContext {

        public virtual float GetBonus(TContext context) {
            return 0;
        }

    }

}