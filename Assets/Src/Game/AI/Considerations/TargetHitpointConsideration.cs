
using System.Diagnostics;

namespace SpaceGame.AI {

    public class TargetHitpointConsideration : Consideration<EntityContext> {

        protected override float Score(EntityContext context) {
            Debug.Assert(context.other.health.maxHitpoints >= 0, "context.other.maxHitPoints >= 0");
            return context.other.health.currentHitpoints / context.other.health.maxHitpoints;
        }
        
    }

}