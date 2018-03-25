using JetBrains.Annotations;
using UnityEngine;

namespace SpaceGame {

    public abstract class AssetDefinition {

        public string name;

        [HideInInspector] public int id;

        protected AssetDefinition() {
            this.id = 0;
            this.name = string.Empty;
        }

        protected AssetDefinition(int id, string name) : this(id) {
            this.name = name;
        }

        [UsedImplicitly]
        public AssetDefinition(int id) {
            this.id = id;
        }

    }

}