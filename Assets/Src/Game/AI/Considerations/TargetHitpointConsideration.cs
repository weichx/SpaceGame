
using System.Diagnostics;

namespace SpaceGame.AI {

    public class TargetHitpointConsideration : Consideration<EntityContext> {

        public override float Score(EntityContext context) {
            Debug.Assert(context.other.maxHitPoints >= 0, "context.other.maxHitPoints >= 0");
            return context.other.hitPoints / context.other.maxHitPoints;
        }
        
    }

}