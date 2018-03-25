using UnityEngine;

namespace SpaceGame.AI.Actions {

    public class BasicAttackAction : AIAction<EntityContext> {

        public override void Setup() { }

        public override bool Tick() {
            TransformInfo trasformInfo = context.agent.transformInfo;
            TransformInfo targetTransformInfo = context.other.transformInfo;

            Vector3 toTarget = trasformInfo.DirectionTo(targetTransformInfo);

            context.agent.FlightSystem.SetTargetDirection(toTarget, ApproachType.Attack);
            context.agent.WeaponSystem.Fire();

            return false;
        }

    }

}