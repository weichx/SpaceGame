using JetBrains.Annotations;
using UnityEngine;

namespace SpaceGame {

    public abstract class MissionAsset {

        public string name;

        [HideInInspector] public int id;

        protected MissionAsset() {
            this.id = 0;
            this.name = string.Empty;
        }

        protected MissionAsset(int id, string name) : this(id) {
            this.name = name;
        }

        [UsedImplicitly]
        protected MissionAsset(int id) {
            this.id = id;
        }

    }

}