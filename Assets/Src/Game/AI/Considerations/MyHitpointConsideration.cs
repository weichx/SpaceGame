using UnityEngine;

namespace SpaceGame.AI {

    public class MyHitpointConsideration : Consideration<SelfContext> {

        public override float Score(SelfContext context) {
            Debug.Assert(context.agent.maxHitPoints >= 0, "context.agent.maxHitPoints >= 0");
            return context.agent.hitPoints / context.agent.maxHitPoints;
        }


    }

}