namespace SpaceGame.AI {

    public class Consideration<TContext> {

        public ResponseCurve curve;

        public bool requiresMainThread;
        
        public virtual float Score(TContext context) {
            return 1f;
        }

        public float ScoreCurved(TContext context) {
            return curve.Evaluate(Score(context));
        }

    }

}