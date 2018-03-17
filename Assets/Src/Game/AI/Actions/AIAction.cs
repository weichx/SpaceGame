using System;

namespace SpaceGame.AI {

    public abstract class AIAction : IContextAware {

        public virtual void Setup() { }
        public abstract bool Tick();
        public virtual void Teardown() { }

        public abstract void SetContext(DecisionContext bestResultContext);

        public abstract Type GetContextType();

    }

    public abstract class AIAction<TContext> : AIAction where TContext : DecisionContext {

        protected TContext context;

        public override void SetContext(DecisionContext context) {
            this.context = (TContext)context;
           
        }

        public override Type GetContextType() {
            return typeof(TContext);
        }

    }

}