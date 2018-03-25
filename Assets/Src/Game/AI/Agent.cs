using SpaceGame.Weapons;
using Src.Engine;
using UnityEngine;
using Weichx.Persistence;
using Weichx.Util;

namespace SpaceGame.AI {

    public class Agent : MonoBehaviour {

        public string serializedData;
        public AgentData agentData;

        public Transform target;
        private FlightController flightController;

        private void Start() {
            agentData = Snapshot<AgentData>.Deserialize(serializedData);
            flightController = GetComponent<FlightController>();
        }

        // select a target
        // select approach direction
        // select a speed
        // select a weapon
        // fire the weapon

        private void SelectDirection() {
            Vector3 dot = Vector3.back;
            float distance = 100f;
            int targetType;
            Vector3 targetForwardVector3;
            int directApproach;
            int leadApproach;
            int getSpace;
        }
        
        private void Update() {
//            float deltaTime = GameTimer.Instance.deltaTime;
//
//            Vector3 direction = transform.position.DirectionTo(target.transform.position);
//            flightController.SetTargetDirection(direction);
//
//            Quaternion rotation = PropulsionUtil.RotateTowardsDirection(
//                transform.rotation,
//                transform.position.DirectionTo(flightController.targetPosition),
//                flightController.turnRate,
//                deltaTime
//            );
//
//            transform.position += rotation.GetForward() * flightController.currentSpeed * deltaTime;
//            transform.rotation = rotation;
//            GetComponent<WeaponSystemComponent>().Fire();
        }

    }

    public class AgentData {

        public float obedience;
        public float aggression;

        public Decision[] decisions;

    }

}