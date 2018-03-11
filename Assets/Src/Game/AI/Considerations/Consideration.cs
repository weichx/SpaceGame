namespace SpaceGame.AI {

    public class Consideration {

        public ResponseCurve curve = new ResponseCurve();
        
        public bool requiresMainThread;

        public virtual float Score(DecisionContext context) {
            return 1f;
        }

    }

    public class Consideration<TContext> : Consideration where TContext : DecisionContext {

        public virtual float Score(TContext context) {
            return 1f;
        }

        public float ScoreCurved(TContext context) {
            return curve.Evaluate(Score(context));
        }

    }

}