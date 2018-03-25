using UnityEngine;

namespace SpaceGame.AI {

    public class TargetForwardFacingMeConsideration : Consideration<EntityContext> {

        protected override float Score(EntityContext context) {
            
            TransformInfo info = context.agent.transformInfo;
            TransformInfo otherInfo = context.other.transformInfo;

            Vector3 toMe = otherInfo.DirectionTo(info);
            return Vector3.Dot(otherInfo.forward, toMe);
            
        }

    }

}