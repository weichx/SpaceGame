namespace SpaceGame.AI {

    public abstract class AIAction {

        public virtual void Setup() { }
        public abstract bool Tick();
        public virtual void Teardown() { }

        public abstract void SetContext(DecisionContext bestResultContext);

    }

    public abstract class AIAction<TContext> : AIAction where TContext : DecisionContext {

        protected TContext context;

        public override void SetContext(DecisionContext context) {
            this.context = (TContext)context;
           
        }

    }

}