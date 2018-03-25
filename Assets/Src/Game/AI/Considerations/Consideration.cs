using System;
using UnityEngine;

namespace SpaceGame.AI {

    public abstract class Consideration : IContextAware {

        [SerializeField] public ResponseCurve curve;

        protected Consideration() {
            this.curve = new ResponseCurve();
        }

        public virtual float Score(DecisionContext context) {
            return 1f;
        }
        
        public abstract Type GetContextType();

    }

    public abstract class Consideration<TContext> : Consideration where TContext : DecisionContext {

        protected abstract float Score(TContext context);

        public float ScoreCurved(TContext context) {
            return curve.Evaluate(Score(context));
        }
        
        public override Type GetContextType() {
            return typeof(TContext);
        }

    }

}