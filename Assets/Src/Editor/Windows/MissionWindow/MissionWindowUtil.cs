using SpaceGame.Util;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Editor.MissionWindow {

    public class MissionWindowUtil {

        public static List<EntityDefinition> ResolveEntityDefinitions(List<EntityDefinition> entityDefinitions, List<Entity> sceneEntities) {

            List<EntityDefinition> missingDefinitions = new List<EntityDefinition>(32);
            List<Entity> missingEntities = new List<Entity>(32);

            for (int i = 0; i < sceneEntities.Count; i++) {
                if(entityDefinitions.FindByIndex(sceneEntities[i].guid, (entDef, guid) => entDef.guid == guid) == -1) {
                    //no definition found for this entity, create one
                    missingDefinitions.Add(new EntityDefinition(sceneEntities[i]));
                }
            }

            for (int i = 0; i < entityDefinitions.Count; i++) {
                if(sceneEntities.FindByIndex(sceneEntities[i].guid, (entity, s) => entity.guid == s) == -1) {
                    //no scene entity found for this guid, create it
//                    missingEntities.Add();
                }
            }

            return entityDefinitions.Concat(missingDefinitions).ToList();
        }

    }

}