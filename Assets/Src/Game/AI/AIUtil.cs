using Lib.Util;
using Weichx.Util;

namespace SpaceGame.AI {

    public static class AIUtil {

        public static float Score(IReadonlyListX<DecisionContext> contexts, Consideration[] considerations, float cutoff, out DecisionContext outputContext) {
            int length = contexts.Count;
            int considerationCount = considerations.Length;

            float modFactor = 1f - (1f / considerationCount);
            float maxScore = float.MinValue;
            outputContext = null;
            DecisionContext[] rawList = contexts.RawArray;
            for (int i = 0; i < length; i++) {
                DecisionContext context = rawList[i];

                float finalScore = 1 + 0; //todo -- bonus / personality influence

                for (int j = 0; j < considerationCount; j++) {

                    if (finalScore < 0 || finalScore < cutoff) {
                        break;
                    }

                    Consideration consideration = considerations[j];
                    UnityEngine.Debug.Assert(consideration != null, nameof(consideration) + " != null");
                    float score = consideration.curve.Evaluate(consideration.Score(context));
                    float makeUpValue = (1f - score) * modFactor;
                    float total = score + (makeUpValue * score);
                    finalScore *= MathUtil.Clamp01(total);
                }

                if (finalScore > maxScore) {
                    maxScore = finalScore;
                    outputContext = context;
                }
            }
            return maxScore;
        }

    }

}