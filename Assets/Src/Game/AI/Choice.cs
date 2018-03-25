namespace SpaceGame.AI {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public interface IRequirement<T> {

        bool Check(T t);

    }

    public interface IConsideration<T> {

        float Score(T t);

    }
    
    public class Choice<T> {

        private int requirementCount;
        private int considerationCount;

        private IRequirement<T>[] requirements;
        private IConsideration<T>[] considerations;

        public Choice(IList<IConsideration<T>> considerations, IList<IRequirement<T>> requirements = null) {
            this.considerations = considerations?.ToArray() ?? new IConsideration<T>[0];
            this.requirements = requirements?.ToArray() ?? new IRequirement<T>[0];

            requirementCount = this.requirements.Length;
            considerationCount = this.considerations.Length;
        }

        public void AddRequirement(IRequirement<T> requirement) {
            if (requirementCount >= requirements.Length) {
                Array.Resize(ref requirements, requirementCount * 2);
            }

            requirements[requirementCount] = requirement;
            requirementCount++;
        }

        public void AddConsideration(IConsideration<T> consideration) {
            if (considerationCount >= considerations.Length) {
                Array.Resize(ref considerations, considerationCount * 2);
            }

            considerations[considerationCount] = consideration;
            considerationCount++;
        }

        public T Choose(T[] options) {
            int result = MakeChoice(options);
            return result < 0 ? default(T) : options[result];
        }

        public int Choose(T[] options, out T selected) {
            int result = MakeChoice(options);
            selected = result < 0 ? default(T) : options[result];
            return result;
        }

        private int MakeChoice(T[] options) {
            
            int maxIndex = -1;
            float maxScore = float.MinValue;
            float modFactor = 1f - (1f / considerationCount);

            for (int i = 0; i < options.Length; i++) {
                T option = options[i];
                bool canConsider = true;
                float finalScore = 1;

                for (int j = 0; j < requirementCount; j++) {
                    if (!requirements[j].Check(option)) {
                        canConsider = false;
                        break;
                    }
                }

                if (!canConsider) continue;

                for (int j = 0; j < considerationCount; j++) {
                    float score = considerations[i].Score(option);
                    float makeUpValue = (1 - score) * modFactor;
                    float total = score + (makeUpValue * score);
                    finalScore *= Mathf.Clamp01(total);
                }

                if (finalScore > maxScore) {
                    maxIndex = i;
                    maxScore = finalScore;
                }
            }

            return maxIndex;
        }

    }

}