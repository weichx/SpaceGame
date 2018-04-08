using System;
using SpaceGame;
using SpaceGame.AI;
using UnityEngine;

namespace Game.AI.Actions {

    public class GoToAction : AIAction {

        private PointContext context;
        
        public override bool Tick() {
//            Debug.Log("Go to Tick");
            context.agent.FlightSystem.SetTargetPosition(context.target.position, ApproachType.Attack);
            return false;
        }

        public override void SetContext(DecisionContext context) {
            this.context = context as PointContext;
        }

        public override Type GetContextType() {
            return typeof(EntityContext);
        }

    }

}