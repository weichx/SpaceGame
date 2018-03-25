using UnityEngine;

namespace SpaceGame.AI {

    public class MyCurrentHitpoints : Consideration<SelfContext> {

        protected override float Score(SelfContext context) {
            Debug.Assert(context.agent.health.currentHitpoints >= 0, "context.agent.maxHitPoints >= 0");
            return context.agent.health.currentHitpoints / context.agent.health.maxHitpoints;
        }


    }

}