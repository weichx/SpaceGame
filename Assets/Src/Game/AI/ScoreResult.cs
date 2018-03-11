namespace SpaceGame.AI {

    internal struct ScoreResult {

        public float score;
        public AIAction action;
        public DecisionContext context;

        public ScoreResult(float score, DecisionContext context, AIAction action) {
            this.score = score;
            this.context = context;
            this.action = action;
        }

    }

}