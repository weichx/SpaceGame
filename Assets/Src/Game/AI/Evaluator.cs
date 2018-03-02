using System.Collections.Generic;
using SpaceGame.Util;

namespace SpaceGame.AI {

    public sealed class Evaluator<TContext> where TContext : DecisionContext {

        public string name;
        public float weight;
        public BonusCalculator<TContext> bonusCalculator;
        public Consideration<TContext>[] considerations;

        private List<TContext> contextList;

        public Evaluator() {
            this.contextList = new List<TContext>(16);
        }

        internal ScoreResult<TContext> Score(Decision<TContext> decision, float cutoff) {
            contextList.Clear();
            decision.contextCreator.CreateContexts(contextList);
            int count = contextList.Count;
            float modFactor = 1f - (1f / considerations.Length);

            for (int i = 0; i < count; i++) {
                TContext context = contextList[i];
                float bonus = bonusCalculator.GetBonus(context);
                float finalScore = 1 + bonus;

                for (int j = 0; j < considerations.Length; j++) {

                    if (finalScore < 0 || finalScore < cutoff) {
                        break;
                    }

                    Consideration<TContext> consideration = considerations[j];
                    float score = consideration.ScoreCurved(context);
                    float makeUpValue = (1f - score) * modFactor;
                    float total = score + (makeUpValue * score);
                    finalScore *= MathUtil.Clamp01(total);
                }
            }

            return new ScoreResult<TContext>();
        }

    }

}