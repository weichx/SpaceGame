using System;
using System.Collections.Generic;
using Src.Attrs;
using UnityEngine;
using Weichx.Persistence;
using EntityDefinitionList = System.Collections.Generic.List<SpaceGame.EntityDefinition>;

namespace SpaceGame.FileTypes {

    public class MissionDefinition : ScriptableObject {
        
        [ReadOnly]
        public string createdAt;

        public string serializedEntityDefinitions;

        public void SetEntityDefinitions(EntityDefinitionList entityDefinitions) {
            serializedEntityDefinitions = Snapshot<EntityDefinitionList>.Serialize(entityDefinitions);
        }
        
        public List<EntityDefinition> GetEntityDefinitions() {
            return Snapshot<EntityDefinitionList>.Deserialize(serializedEntityDefinitions);
        }
        
        public static MissionDefinition Create(string name) {
            MissionDefinition asset = CreateInstance<MissionDefinition>();
            asset.name = name;
            asset.serializedEntityDefinitions = Snapshot<EntityDefinitionList>.SerializeDefault();
            asset.createdAt = DateTime.Now.ToShortTimeString() + " on " + DateTime.Now.ToShortDateString();
            return asset;
        }

    }

}