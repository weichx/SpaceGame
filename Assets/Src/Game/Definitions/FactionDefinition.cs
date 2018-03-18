using System.Collections.Generic;
using UnityEngine;

namespace SpaceGame {

    public class FactionDescription {

        public Texture2D icon;
        public string name;
        public int factionId;

        public AssetPointer<Texture2D> iconPointer;
        public List<EntityDefinition> entities;

        public FactionDescription(Texture2D icon, string name, int id) {
            this.icon = icon;
            this.name = name;
            this.factionId = id;
            this.entities = new List<EntityDefinition>();
            entities.Add(new EntityDefinition());
            entities.Add(new EntityDefinition());
            entities.Add(new EntityDefinition());
        }

    }

}