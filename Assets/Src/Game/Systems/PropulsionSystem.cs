using UnityEngine;
using Serialize = System.SerializableAttribute;

namespace SpaceGame.Systems {

    [Serialize]
    public class PropulsionSystemData {

        public float maxSpeed;
        public float minSpeed;

        public float breakingRate;
        public float accelerationRate;
        public float turnRate;
        public float responsiveness;
        public float slideRate;
        public float afterburnerPower;
        public float afterburnerPowerConsumption;
        public float maxAfterburnerSpeed;
        public float maxAfterburnerDuration;
        public float afterburnerCooldown;

    }

    [Serialize]
    public class PhysicsData {

        public int id;
        public Vector3 direction;
        public Vector3 position;
        public Vector3 desiredDirection;
        public Vector3 adjustedDirection;
        public Vector3 velocity;
        public float speed;
        public float desiredSpeed;
        public float velocityTrend;

    }

    [RequireComponent(typeof(Rigidbody))]
    public class PropulsionSystem : SpaceGame.Systems.System {

        public PhysicsData physicsData;
        public PropulsionSystemData propulsionData;
        
        public new Rigidbody rigidbody;

        public void Awake() {
            rigidbody = GetComponent<Rigidbody>();
        }

        public void TickPhysics() {
            
        }

        public void OrientToDirectionWithoutPhysics(Vector3 direction) {
            Vector3 localTarget = transform.InverseTransformDirection(direction);
            float radiansToTargetYaw = Mathf.Atan2(localTarget.x, localTarget.z);
            float radiansToTargetPitch = -Mathf.Atan2(localTarget.y, localTarget.z);
            float radiansToTargetRoll = -radiansToTargetYaw + GetRollAngle();
            float turnRateRadians = propulsionData.turnRate * Mathf.Deg2Rad;
            float pitch = Mathf.Clamp(radiansToTargetPitch, -turnRateRadians, turnRateRadians) * Mathf.Rad2Deg;
            float yaw = Mathf.Clamp(radiansToTargetYaw, -turnRateRadians, turnRateRadians) * Mathf.Rad2Deg;
            float roll = Mathf.Clamp(radiansToTargetRoll, -turnRateRadians, turnRateRadians) * Mathf.Rad2Deg;
            Quaternion.LookRotation(transform.forward, transform.up);
            Vector3 eulerAngles = new Vector3(pitch, yaw, roll) * Time.fixedDeltaTime;
            Quaternion quaternion = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
            rigidbody.rotation = rigidbody.rotation * quaternion;
        }

        public void OrientToDirection(Vector3 direction) {
            Vector3 localAV = transform.InverseTransformDirection(rigidbody.angularVelocity);
            Vector3 localTarget = transform.InverseTransformDirection(direction);
            float radiansToTargetYaw = Mathf.Atan2(localTarget.x, localTarget.z);
            float radiansToTargetPitch = -Mathf.Atan2(localTarget.y, localTarget.z);
            float radiansToTargetRoll = -radiansToTargetYaw + GetRollAngle();
            float turnRateRadians = propulsionData.turnRate * Mathf.Deg2Rad;

            localAV.x = ComputeLocalAV(radiansToTargetPitch, localAV.x, turnRateRadians, 1f);
            localAV.y = ComputeLocalAV(radiansToTargetYaw, localAV.y, turnRateRadians, 1f);
            localAV.z = ComputeLocalAV(radiansToTargetRoll, localAV.z, turnRateRadians, 1f);

            localAV.x = Mathf.Clamp(localAV.x, -turnRateRadians, turnRateRadians);
            localAV.y = Mathf.Clamp(localAV.y, -turnRateRadians, turnRateRadians);
            localAV.z = Mathf.Clamp(localAV.z, -turnRateRadians, turnRateRadians);
            rigidbody.angularVelocity = transform.TransformDirection(localAV);
        }

        private float GetRollAngle() {
            Vector3 flatForward = transform.forward;
            float rollAngle = 0f;
            flatForward.y = 0;

            if (flatForward.sqrMagnitude > 0) {
                flatForward.Normalize();
                Vector3 flatRight = Vector3.Cross(Vector3.up, flatForward);
                Vector3 localFlatRight = transform.InverseTransformDirection(flatRight);
                rollAngle = Mathf.Atan2(localFlatRight.y, localFlatRight.x);
            }

            return rollAngle;
        }

        //calculates force needed across a single local angular velocity axis to align with a target. Away handles current AV direction being away from desired AV
        private float Away(float inputVelocity, float maxVelocity, float radiansToGoal, float maxAcceleration, float deltaTime, bool noOvershoot) {
            if ((-inputVelocity < 1e-5) && (radiansToGoal < 1e-5)) {
                return 0f;
            }

            if (maxAcceleration == 0) {
                return inputVelocity;
            }

            float t0 = -inputVelocity / maxAcceleration; //time until velocity is zero

            if (t0 > deltaTime) { // no reversal in this time interval
                return inputVelocity + maxAcceleration * deltaTime;
            }

            // use time remaining after v = 0
            radiansToGoal -= 0.5f * inputVelocity * t0; //will be negative
            return Approach(0.0f, maxVelocity, radiansToGoal, maxAcceleration, deltaTime - t0, noOvershoot);
        }

        //calculates force needed across a single local angular velocity axis to align with a target, optionally allow overshooting
        private float Approach(float inputVelocity, float maxVelocity, float radiansToGoal, float maxAcceleration, float deltaTime, bool noOvershoot) {
            if (maxAcceleration == 0) {
                return inputVelocity;
            }

            if (noOvershoot && (inputVelocity * inputVelocity > 2.0f * 1.05f * maxAcceleration * radiansToGoal)) {
                inputVelocity = Mathf.Sqrt(2.0f * maxAcceleration * radiansToGoal);
            }

            if (inputVelocity * inputVelocity > 2.0f * 1.05f * maxAcceleration * radiansToGoal) { // overshoot condition
                float effectiveAngularAcceleration = 1.05f * maxAcceleration;
                float deltaRadiansToGoal = inputVelocity * deltaTime - 0.5f * effectiveAngularAcceleration * deltaTime * deltaTime; // amount rotated during time delta_t

                if (deltaRadiansToGoal > radiansToGoal) { // pass goal during this frame
                    float timeToGoal = (-inputVelocity + Mathf.Sqrt(inputVelocity * inputVelocity + 2.0f * effectiveAngularAcceleration * radiansToGoal)) / effectiveAngularAcceleration;
                    // get time to theta_goal and away
                    inputVelocity -= effectiveAngularAcceleration * timeToGoal;
                    return -Away(-inputVelocity, maxVelocity, 0.0f, maxAcceleration, deltaTime - timeToGoal, noOvershoot);
                }
                else {
                    if (deltaRadiansToGoal < 0) {
                        // pass goal and return this frame
                        return 0.0f;
                    }
                    else {
                        // do not pass goal this frame
                        return inputVelocity - effectiveAngularAcceleration * deltaTime;
                    }
                }
            }
            else if (inputVelocity * inputVelocity < 2.0f * 0.95f * maxAcceleration * radiansToGoal) { // undershoot condition
                // find peak angular velocity
                float peakVelocitySqr = Mathf.Sqrt(maxAcceleration * radiansToGoal + 0.5f * inputVelocity * inputVelocity);

                if (peakVelocitySqr > maxVelocity * maxVelocity) {
                    float timeToMaxVelocity = (maxVelocity - inputVelocity) / maxAcceleration;

                    if (timeToMaxVelocity < 0) {
                        // speed already too high
                        // TODO: consider possible ramp down to below w_max
                        float outputVelocity = inputVelocity - maxAcceleration * deltaTime;

                        if (outputVelocity < 0) {
                            outputVelocity = 0.0f;
                        }

                        return outputVelocity;
                    }
                    else if (timeToMaxVelocity > deltaTime) {
                        // does not reach w_max this frame
                        return inputVelocity + maxAcceleration * deltaTime;
                    }
                    else {
                        // reaches w_max this frame
                        // TODO: consider when to ramp down from w_max
                        return maxVelocity;
                    }
                }
                else { // wp < w_max
                    if (peakVelocitySqr > (inputVelocity + maxAcceleration * deltaTime) * (inputVelocity + maxAcceleration * deltaTime)) {
                        // does not reach wp this frame
                        return inputVelocity + maxAcceleration * deltaTime;
                    }
                    else {
                        // reaches wp this frame
                        float wp = Mathf.Sqrt(peakVelocitySqr);
                        float timeToPeakVelocity = (wp - inputVelocity) / maxAcceleration;

                        // accel
                        float outputVelocity = wp;
                        // decel
                        float timeRemaining = deltaTime - timeToPeakVelocity;
                        outputVelocity -= maxAcceleration * timeRemaining;

                        if (outputVelocity < 0) { // reached goal
                            outputVelocity = 0.0f;
                        }

                        return outputVelocity;
                    }
                }
            }
            else { // on target
                // reach goal this frame
                if (inputVelocity - maxAcceleration * deltaTime < 0) {
                    // reach goal this frame

                    return 0f;
                }
                else {
                    // move toward goal
                    return inputVelocity - maxAcceleration * deltaTime;
                }
            }
        }

        private float ComputeLocalAV(float targetAngle, float localAV, float maxVel, float acceleration) {
            if (targetAngle > 0) {
                if (localAV >= 0) {
                    return Approach(localAV, maxVel, targetAngle, acceleration, Time.fixedDeltaTime, false);
                }
                else {
                    return Away(localAV, maxVel, targetAngle, acceleration, Time.fixedDeltaTime, false);
                }
            }
            else if (targetAngle < 0) {
                if (localAV <= 0) {
                    return -Approach(-localAV, maxVel, -targetAngle, acceleration, Time.fixedDeltaTime, false);
                }
                else {
                    return -Away(-localAV, maxVel, -targetAngle, acceleration, Time.fixedDeltaTime, false);
                }
            }
            else {
                return 0;
            }
        }

    }

}