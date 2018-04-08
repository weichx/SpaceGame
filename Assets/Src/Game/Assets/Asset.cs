using JetBrains.Annotations;
using UnityEngine;

namespace SpaceGame {

    public abstract class Asset {

        [HideInInspector] public readonly int id;
        public string name;

        protected Asset(int id, string name) {
            this.id = id;
            this.name = name;
        }

        [UsedImplicitly]
        protected Asset() {}

        public virtual string DisplayName {
            get { return name; }
        }

        public virtual Texture2D Icon {
            get { return null; }
        }

    }

}