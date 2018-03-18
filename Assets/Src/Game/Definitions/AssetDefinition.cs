using System;
using UnityEngine;

namespace SpaceGame {

    public abstract class AssetDefinition {

        public string name;
        [HideInInspector] public readonly string guid;
        [HideInInspector] public readonly int id;

        private static int idGenerator;

        protected AssetDefinition(string name) : this() {
            this.name = name;
        }
        
        protected AssetDefinition() {
            this.id = ++idGenerator;
            this.name = string.Empty;
            this.guid = Guid.NewGuid().ToString();
        }
        
    }

}