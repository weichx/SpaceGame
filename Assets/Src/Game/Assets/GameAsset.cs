using System;
using UnityEngine;
using Weichx.ReflectionAttributes;

namespace SpaceGame.Assets {

    public abstract class GameAsset {

        public string name;
        [HideInInspector] public readonly int id;
        [ReadOnly]public readonly string createdAt;

        protected GameAsset() { }

        protected GameAsset(int id, string name) {
            this.id = id;
            this.name = name;
            this.createdAt = $"{DateTime.Now.ToShortTimeString()} on {DateTime.Now.ToShortDateString()}";

        }

    }

}