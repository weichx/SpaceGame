namespace SpaceGame.AI {

    internal struct ScoreResult<TContext> where TContext : DecisionContext {

        public float score;
        public TContext context;
        public AIAction<TContext> action;

        public ScoreResult(float score, TContext context, AIAction<TContext> action) {
            this.score = score;
            this.context = context;
            this.action = action;
        }

    }

}