using System.Collections.Generic;
using JetBrains.Annotations;
using SpaceGame.FileTypes;
using UnityEngine;

namespace SpaceGame {

    public class FactionDefinition : AssetDefinition {

        private Texture2D iconTexture;
        
        public AssetPointer<Texture2D> iconPointer;
        [HideInInspector] public readonly List<EntityDefinition> entities;

        [PublicAPI]
        public FactionDefinition() : this(string.Empty) { }

        public FactionDefinition(string name) : base (name) {
            this.iconPointer = new AssetPointer<Texture2D>();
            this.entities = new List<EntityDefinition>(16);
        }

        public Texture2D icon {
            get {
                if (iconTexture) {
                    return iconTexture;
                }
                return null;
            }
            set {
                iconPointer = new AssetPointer<Texture2D>(value);
                iconTexture = iconPointer.GetAsset();
            }
        }
        

    }

}