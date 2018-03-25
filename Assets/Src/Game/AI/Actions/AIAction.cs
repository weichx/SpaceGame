using System;

namespace SpaceGame.AI {

    public enum AIActionStatus {

        None, Running, Succeeded, Failed, Interrupted

    }
    public abstract class AIAction : IContextAware {

        private bool isSetup;
        
        public void Setup() {
            
        }
        
        public virtual void OnSetup() { }
        public abstract bool Tick();
        public virtual void Teardown() { }

        public abstract void SetContext(DecisionContext bestResultContext);

        public abstract Type GetContextType();

        public Consideration[] considerations;

        public float Score(DecisionContext context, float maxScore) {
            return 1f;
        }

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