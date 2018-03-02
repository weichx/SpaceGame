namespace SpaceGame.AI {

    public abstract class Requirement<TContext> where TContext : DecisionContext {

        public string name;
        public string description;

        public abstract bool Check(TContext context);

    }

}