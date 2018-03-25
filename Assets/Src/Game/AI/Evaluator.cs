using System.Collections.Generic;
using System.Diagnostics;
using Weichx.ReflectionAttributes;
using Weichx.Util;

namespace SpaceGame.AI {

    public class Evaluator {


        public string name;
        [UnityEngine.Range(0.1f, 10f)] public float weight;

        [UsePropertyDrawer(typeof(ConstructableSubclass))]
        public BonusCalculator bonusCalculator;

        public Consideration[] considerations;

        protected Evaluator() {
            considerations = new Consideration[1];
        }

        internal virtual ScoreResult Score(Entity agent, Decision decision, float cutoff) {
            return default(ScoreResult);
        }

    }

    public sealed class Evaluator<TContext> : Evaluator where TContext : DecisionContext {

//        public BonusCalculator<TContext> bonusCalculator;
//        public Consideration<TContext>[] considerations;

        private List<TContext> contextList;

        public Evaluator() {
            this.contextList = new List<TContext>(16);
        }

        internal override ScoreResult Score(Entity agent, Decision decision, float cutoff) {
            contextList.Clear();
            ContextCreator<TContext> creator = decision.contextCreator as ContextCreator<TContext>;
            BonusCalculator<TContext> bonusCalc = bonusCalculator as BonusCalculator<TContext>;
            
            Debug.Assert(creator != null, nameof(creator) + " != null");
            Debug.Assert(bonusCalc != null, nameof(bonusCalc) + " != null");
            Debug.Assert(agent != null, nameof(agent) + " != null");

            creator.CreateContexts(agent, contextList);
            int count = contextList.Count;
            float modFactor = 1f - (1f / considerations.Length);
            ScoreResult scoreResult = new ScoreResult(0, null, null);
            float maxScore = float.MinValue;
            
            for (int i = 0; i < count; i++) {
                TContext context = contextList[i];
                float bonus = bonusCalc.GetBonus(context);
                float finalScore = 1 + bonus;

                for (int j = 0; j < considerations.Length; j++) {

                    if (finalScore < 0 || finalScore < cutoff) {
                        break;
                    }

                    //todo -- hold a second array of pre-cast considerations?
                    Consideration<TContext> consideration = considerations[j] as Consideration<TContext>;
                    Debug.Assert(consideration != null, nameof(consideration) + " != null");
                    float score = consideration.ScoreCurved(context);
                    float makeUpValue = (1f - score) * modFactor;
                    float total = score + (makeUpValue * score);
                    finalScore *= MathUtil.Clamp01(total);
                }

                if (finalScore > maxScore) {
                    maxScore = finalScore;
                    scoreResult = new ScoreResult(maxScore, context, decision.action);
                }
            }

            return scoreResult;
        }

    }

}