using UnityEngine;

namespace SpaceGame {

    [System.Serializable]
    public struct DispositionRating {

        public float rating;
        public Disposition type;

        public DispositionRating(Disposition type = Disposition.Neutral, float rating = 1f) {
            this.type = type;
            this.rating = Mathf.Clamp01(rating);
        }

    }
}