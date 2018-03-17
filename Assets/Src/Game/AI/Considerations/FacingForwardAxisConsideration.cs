using UnityEngine;

namespace SpaceGame.AI {

    public class FacingForwardAxisConsideration : Consideration<EntityContext> {

        public override float Score(EntityContext context) {
            TransformInfo info = context.agent.transformInfo;
            TransformInfo otherInfo = context.other.transformInfo;

            Vector3 toOther = info.DirectionTo(otherInfo);
            return Vector3.Dot(info.forward, toOther);

        }

    }

}