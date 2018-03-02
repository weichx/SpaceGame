namespace SpaceGame.AI {

    public abstract class AIAction<TContext> where TContext : DecisionContext {

        protected TContext context;

        public void SetContext(TContext context) {
            this.context = context;
        }

        public virtual void Setup() { }
        public abstract bool Tick();
        public virtual void Teardown() { }

    }
}