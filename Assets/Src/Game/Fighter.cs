//using System;
//using System.Collections.Generic;
//using SpaceGame.Events;
//using Weichx.Util;
//using SpaceGame.Weapons;
//using UnityEngine;
//using Util;
//
//namespace SpaceGame {
//
//    struct GunPoint {
//
//        public readonly Vector3 point;
//        public readonly Vector3 normal;
//
//        public GunPoint(Vector3 point, Vector3 normal) {
//            this.point = point;
//            this.normal = normal;
//        }
//
//    }
//
//    class Evt_SetCombatTarget : StateChartEvent {
//
//        public readonly Entity target;
//
//        public Evt_SetCombatTarget(Entity target) {
//            this.target = target;
//        }
//
//    }
//
//    class Evt_CombatEnd : StateChartEvent { }
//
//    class Evt_CombatEnter : StateChartEvent { }
//
//    [DisallowMultipleComponent]
//    [RequireComponent(typeof(Entity))]
//    public class Fighter : MonoBehaviour {
//
//        public float radius = 4;
//        public float turnRate = 90;
//        public float fireRate = 1f;
//        public float lastFireTime;
//        public float speed = 1;
//        private int gunIndex;
//        private new Rigidbody rigidbody;
//        private GunPoint[] gunpoints;
//        private StateChart chart;
//        public float arrivalThreshold = 2f;
//        private Entity entity;
//        public Entity target;
//        public Vector3 targetDirection;
//
//        public float MaxSpeed;
//        
//        public void Start() {
//            entity = GetComponent<Entity>();
//            rigidbody = GetComponent<Rigidbody>();
//            Transform gunRoot = transform.Find("Gun Points");
//            int childCount = gunRoot.childCount;
//            gunpoints = new GunPoint[childCount];
//
//            for (int i = 0; i < childCount; i++) {
//                Transform child = gunRoot.GetChild(i);
//                gunpoints[i] = new GunPoint(child.localPosition * transform.localScale.x, child.forward);
//            }
//
//            CreateStateChart();
//        }
//
//        public void Update() {
//            chart.Tick();
//            Fire();
//        }
//
//        private void FixedUpdate() {
//            List<PossibleCollision> pcs = QueryPossibleCollisions(20, 20);
//            Vector3 adustedDirection =  GetCollisionAvoidanceForce(pcs, targetDirection);
//            OrientToDirection(adustedDirection);
//            rigidbody.velocity = transform.forward * speed;
//        }
//
//        private void OnCollisionEnter(Collision other) {
//            Debug.Log("Collided -> " + transform.name + " + " + other.gameObject.name);
//        }
//
//        // todo - data orient this
//        // todo - there is probably a way to not do n^2 bounds checks
//        // todo - take velocity into account, may not need to avoid collision for things moving away from us
//        public List<PossibleCollision> QueryPossibleCollisions(float detectionRange, float collisionHorizon) {
//            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
//            List<PossibleCollision> possibleCollisions = new List<PossibleCollision>(colliders.Length);
//
//            Vector3 velocity = rigidbody.velocity;
//            Vector3 position = transform.position;
//            
//            for (int i = 0; i < colliders.Length; i++) {
//                Collider col = colliders[i];
//                Transform otherTransform = col.transform;
//                if (col.transform.IsChildOf(transform)) continue;
//
//                Vector3 otherVelocity = Vector3.zero;
//                Vector3 otherPosition = otherTransform.position;
//                Vector3 size = col.bounds.size;
//                float otherRadius = Mathf.Max(size.x, Mathf.Max(size.y, size.z)); // * t.localScale.x;
//                Rigidbody otherRb = col.attachedRigidbody;
//
//                if (otherRb) {
//                    otherVelocity = otherRb.velocity;
//                }
//
//                float timeToCollision = Prediction.TimeToCollision(
//                    position, velocity, radius,
//                    otherPosition, otherVelocity, otherRadius
//                );
//                
//                if (timeToCollision < 0f || timeToCollision >= collisionHorizon) {
//                    continue;
//                }
//               
//                //todo -- pool these
//                PossibleCollision pc = new PossibleCollision();
//                pc.radius = otherRadius;
//                pc.timeToImpact = timeToCollision;
//                pc.transform = otherTransform;
//                pc.velocity = otherVelocity;
//                pc.strengthModifier = 1f;
//                pc.horizon = collisionHorizon;
//                
//                Debug.DrawLine(transform.position, otherTransform.position, Color.red);
//                possibleCollisions.Add(pc);
//            }
//
//            return possibleCollisions;
//        }
//
//        // todo - data orient this
//        protected Vector3 GetCollisionAvoidanceForce(List<PossibleCollision> possibleCollisions, Vector3 direction) {
//            Vector3 force = direction * speed;
//            Vector3 velocity = rigidbody.velocity;
//            Vector3 position = transform.position;
//
//            if (possibleCollisions.Count == 0) return direction;
//            
//            for (int i = 0; i < possibleCollisions.Count; i++) {
//                PossibleCollision pc = possibleCollisions[i];
//                Vector3 avoidForce;
//                float timeToImpact = pc.timeToImpact;
//
//                if (timeToImpact == 0) {
//                    avoidForce = (position - pc.transform.position).normalized * MaxSpeed * pc.strengthModifier;
//                }
//                else {
//                    avoidForce = position + velocity * timeToImpact - pc.transform.position - pc.velocity * timeToImpact;
//                    avoidForce.Normalize();
//
//                    float mag = 0f;
//                    if (timeToImpact >= 0 && timeToImpact <= pc.horizon) {
//                        mag = (pc.horizon - timeToImpact) / (timeToImpact + 0.001f) * pc.strengthModifier;
//                    }
//                    avoidForce *= mag;
//                }
//
//                force += avoidForce;
//            }
//            //I think this wants to be a normalized weighted average so that the pilot can decide speed
//            force /= (possibleCollisions.Count + 1);
//            return force.normalized;
//            //return (force.magnitude > MaxSpeed) ? force.normalized * MaxSpeed : force;
//        }
//        
//        public void CreateStateChart() {
//            chart = new StateChart((s) => {
//                Action<Action> Init = s.Init;
//                Action<Action> Enter = s.Enter;
//                Action<Action> Update = s.Update;
//                Action<Action> Exit = s.Exit;
//
//                s.State("Idle", () => {
//                    s.State("Patrol", () => {
//                        Update(FlyWaypoints);
//                    });
//
//                    Update(FindTarget);
//
//                    s.Transition<Evt_SetCombatTarget>("Target");
//                });
//
//                s.State("Combat", () => {
//                    s.State("NoTarget", () => {
//                        Update(FindTarget);
//                    });
//
//                    s.State("Target", () => {
//                        Update(CombatUpdate);
//                    });
//
//                    s.State("Distress");
//                    s.State("AttackRun");
//                    s.State("Dogfight", () => { });
//                    s.State("Evade");
//                    s.State("Dock");
//                    s.State("Suicide");
//                });
//
//                s.Event<Evt_SetCombatTarget>((evt) => target = evt.target);
//            });
//        }
//
//        private void FlyWaypoints() {
//
//        }
//
//        private void CombatUpdate() {
//            Vector3 targetPosition = target.transform.position;
//            Vector3 diff = (targetPosition - transform.position);
//            Vector3 toTarget = diff.normalized;
//            float distanceSqr = diff.sqrMagnitude;
//
//            targetDirection = toTarget;
//
//            // are we too close?
//            // re-evaluate pursuit style?
//            float goalDot = Vector3.Dot(toTarget.normalized, transform.forward);
//
//            // aligned, we can fire now
//            if (goalDot >= 0.98f) {
//                Fire();
//            }
//            
//            if (distanceSqr < arrivalThreshold * (radius * radius)) {
//                targetDirection = -toTarget;
//            }
//            else {
//                // are we facing them?
//                // are we too far?
//            }
//        }
//
//        private void FindTarget() {
//            const float sensorRange = 1000f;
//            List<Entity> targetOptions = EntityDatabase.FindHostilesInRange(entity.faction, transform.position, sensorRange);
//
//            if (targetOptions.Count == 0) {
//                chart.Trigger(new Evt_CombatEnd());
//            }
//            else {
//                Entity newTarget = FindNearestEntity(targetOptions);
//
//                if (newTarget != null) {
//                    chart.Trigger(new Evt_SetCombatTarget(newTarget));
//                }
//            }
//        }
//
//        private Entity FindNearestEntity(List<Entity> entities) {
//            float min = float.MaxValue;
//            Vector3 position = transform.position;
//            Entity retn = null;
//
//            for (int i = 0; i < entities.Count; i++) {
//                float distSquared = (entities[i].transform.position - position).sqrMagnitude;
//
//                if (distSquared < min) {
//                    min = distSquared;
//                    retn = entities[i];
//                }
//            }
//
//            return retn;
//        }
//        
//        private void Fire() {
//            if (GameTimer.Instance.GetFrameTimestamp() - lastFireTime >= fireRate) {
//                lastFireTime = GameTimer.Instance.GetFrameTimestamp();
//                Vector3 point = transform.position + gunpoints[gunIndex].point;
//                Vector3 direction = transform.rotation * gunpoints[gunIndex].normal;
//                gunIndex = (gunIndex + 1) % gunpoints.Length;
//                FiringParameters parameters = new FiringParameters();
//                parameters.weaponType = WeaponType.Vulcan;
//                parameters.direction = direction;
//                parameters.ownerId = entity.id;
//                parameters.position = point;
//                EventSystem.Instance.Trigger(Evt_WeaponFired.Spawn(parameters));
//            }
//        }
//
//        public void OrientToDirectionWithoutPhysics(Vector3 direction) {
//            Vector3 localTarget = transform.InverseTransformDirection(direction);
//            float radiansToTargetYaw = Mathf.Atan2(localTarget.x, localTarget.z);
//            float radiansToTargetPitch = -Mathf.Atan2(localTarget.y, localTarget.z);
//            float radiansToTargetRoll = -radiansToTargetYaw + GetRollAngle();
//            float turnRateRadians = turnRate * Mathf.Deg2Rad;
//            float pitch = Mathf.Clamp(radiansToTargetPitch, -turnRateRadians, turnRateRadians) * Mathf.Rad2Deg;
//            float yaw = Mathf.Clamp(radiansToTargetYaw, -turnRateRadians, turnRateRadians) * Mathf.Rad2Deg;
//            float roll = Mathf.Clamp(radiansToTargetRoll, -turnRateRadians, turnRateRadians) * Mathf.Rad2Deg;
//            Quaternion.LookRotation(transform.forward, transform.up);
//            Vector3 eulerAngles = new Vector3(pitch, yaw, roll) * Time.fixedDeltaTime;
//            Quaternion quaternion = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
//            rigidbody.rotation = rigidbody.rotation * quaternion;
//        }
//
//        public void OrientToDirection(Vector3 direction) {
//            Vector3 localAV = transform.InverseTransformDirection(rigidbody.angularVelocity);
//            Vector3 localTarget = transform.InverseTransformDirection(direction);
//            float radiansToTargetYaw = Mathf.Atan2(localTarget.x, localTarget.z);
//            float radiansToTargetPitch = -Mathf.Atan2(localTarget.y, localTarget.z);
//            float radiansToTargetRoll = -radiansToTargetYaw + GetRollAngle();
//            float turnRateRadians = turnRate * Mathf.Deg2Rad;
//
//            localAV.x = ComputeLocalAV(radiansToTargetPitch, localAV.x, turnRateRadians, 1f);
//            localAV.y = ComputeLocalAV(radiansToTargetYaw, localAV.y, turnRateRadians, 1f);
//            localAV.z = ComputeLocalAV(radiansToTargetRoll, localAV.z, turnRateRadians, 1f);
//
//            localAV.x = Mathf.Clamp(localAV.x, -turnRateRadians, turnRateRadians);
//            localAV.y = Mathf.Clamp(localAV.y, -turnRateRadians, turnRateRadians);
//            localAV.z = Mathf.Clamp(localAV.z, -turnRateRadians, turnRateRadians);
//            rigidbody.angularVelocity = transform.TransformDirection(localAV);
//        }
//
//        private float GetRollAngle() {
//            Vector3 flatForward = transform.forward;
//            float rollAngle = 0f;
//            flatForward.y = 0;
//
//            if (flatForward.sqrMagnitude > 0) {
//                flatForward.Normalize();
//                Vector3 flatRight = Vector3.Cross(Vector3.up, flatForward);
//                Vector3 localFlatRight = transform.InverseTransformDirection(flatRight);
//                rollAngle = Mathf.Atan2(localFlatRight.y, localFlatRight.x);
//            }
//
//            return rollAngle;
//        }
//
//        //calculates force needed across a single local angular velocity axis to align with a target. Away handles current AV direction being away from desired AV
//        private float Away(float inputVelocity, float maxVelocity, float radiansToGoal, float maxAcceleration, float deltaTime, bool noOvershoot) {
//            if ((-inputVelocity < 1e-5) && (radiansToGoal < 1e-5)) {
//                return 0f;
//            }
//
//            if (maxAcceleration == 0) {
//                return inputVelocity;
//            }
//
//            float t0 = -inputVelocity / maxAcceleration; //time until velocity is zero
//
//            if (t0 > deltaTime) { // no reversal in this time interval
//                return inputVelocity + maxAcceleration * deltaTime;
//            }
//
//            // use time remaining after v = 0
//            radiansToGoal -= 0.5f * inputVelocity * t0; //will be negative
//            return Approach(0.0f, maxVelocity, radiansToGoal, maxAcceleration, deltaTime - t0, noOvershoot);
//        }
//
//        //calculates force needed across a single local angular velocity axis to align with a target, optionally allow overshooting
//        private float Approach(float inputVelocity, float maxVelocity, float radiansToGoal, float maxAcceleration, float deltaTime, bool noOvershoot) {
//            if (maxAcceleration == 0) {
//                return inputVelocity;
//            }
//
//            if (noOvershoot && (inputVelocity * inputVelocity > 2.0f * 1.05f * maxAcceleration * radiansToGoal)) {
//                inputVelocity = Mathf.Sqrt(2.0f * maxAcceleration * radiansToGoal);
//            }
//
//            if (inputVelocity * inputVelocity > 2.0f * 1.05f * maxAcceleration * radiansToGoal) { // overshoot condition
//                float effectiveAngularAcceleration = 1.05f * maxAcceleration;
//                float deltaRadiansToGoal = inputVelocity * deltaTime - 0.5f * effectiveAngularAcceleration * deltaTime * deltaTime; // amount rotated during time delta_t
//
//                if (deltaRadiansToGoal > radiansToGoal) { // pass goal during this frame
//                    float timeToGoal = (-inputVelocity + Mathf.Sqrt(inputVelocity * inputVelocity + 2.0f * effectiveAngularAcceleration * radiansToGoal)) / effectiveAngularAcceleration;
//                    // get time to theta_goal and away
//                    inputVelocity -= effectiveAngularAcceleration * timeToGoal;
//                    return -Away(-inputVelocity, maxVelocity, 0.0f, maxAcceleration, deltaTime - timeToGoal, noOvershoot);
//                }
//                else {
//                    if (deltaRadiansToGoal < 0) {
//                        // pass goal and return this frame
//                        return 0.0f;
//                    }
//                    else {
//                        // do not pass goal this frame
//                        return inputVelocity - effectiveAngularAcceleration * deltaTime;
//                    }
//                }
//            }
//            else if (inputVelocity * inputVelocity < 2.0f * 0.95f * maxAcceleration * radiansToGoal) { // undershoot condition
//                // find peak angular velocity
//                float peakVelocitySqr = Mathf.Sqrt(maxAcceleration * radiansToGoal + 0.5f * inputVelocity * inputVelocity);
//
//                if (peakVelocitySqr > maxVelocity * maxVelocity) {
//                    float timeToMaxVelocity = (maxVelocity - inputVelocity) / maxAcceleration;
//
//                    if (timeToMaxVelocity < 0) {
//                        // speed already too high
//                        // TODO: consider possible ramp down to below w_max
//                        float outputVelocity = inputVelocity - maxAcceleration * deltaTime;
//
//                        if (outputVelocity < 0) {
//                            outputVelocity = 0.0f;
//                        }
//
//                        return outputVelocity;
//                    }
//                    else if (timeToMaxVelocity > deltaTime) {
//                        // does not reach w_max this frame
//                        return inputVelocity + maxAcceleration * deltaTime;
//                    }
//                    else {
//                        // reaches w_max this frame
//                        // TODO: consider when to ramp down from w_max
//                        return maxVelocity;
//                    }
//                }
//                else { // wp < w_max
//                    if (peakVelocitySqr > (inputVelocity + maxAcceleration * deltaTime) * (inputVelocity + maxAcceleration * deltaTime)) {
//                        // does not reach wp this frame
//                        return inputVelocity + maxAcceleration * deltaTime;
//                    }
//                    else {
//                        // reaches wp this frame
//                        float wp = Mathf.Sqrt(peakVelocitySqr);
//                        float timeToPeakVelocity = (wp - inputVelocity) / maxAcceleration;
//
//                        // accel
//                        float outputVelocity = wp;
//                        // decel
//                        float timeRemaining = deltaTime - timeToPeakVelocity;
//                        outputVelocity -= maxAcceleration * timeRemaining;
//
//                        if (outputVelocity < 0) { // reached goal
//                            outputVelocity = 0.0f;
//                        }
//
//                        return outputVelocity;
//                    }
//                }
//            }
//            else { // on target
//                // reach goal this frame
//                if (inputVelocity - maxAcceleration * deltaTime < 0) {
//                    // reach goal this frame
//
//                    return 0f;
//                }
//                else {
//                    // move toward goal
//                    return inputVelocity - maxAcceleration * deltaTime;
//                }
//            }
//        }
//
//        private float ComputeLocalAV(float targetAngle, float localAV, float maxVel, float acceleration) {
//            if (targetAngle > 0) {
//                if (localAV >= 0) {
//                    return Approach(localAV, maxVel, targetAngle, acceleration, Time.fixedDeltaTime, false);
//                }
//                else {
//                    return Away(localAV, maxVel, targetAngle, acceleration, Time.fixedDeltaTime, false);
//                }
//            }
//            else if (targetAngle < 0) {
//                if (localAV <= 0) {
//                    return -Approach(-localAV, maxVel, -targetAngle, acceleration, Time.fixedDeltaTime, false);
//                }
//                else {
//                    return -Away(-localAV, maxVel, -targetAngle, acceleration, Time.fixedDeltaTime, false);
//                }
//            }
//            else {
//                return 0;
//            }
//        }
//
////        private void OnDrawGizmosSelected() {
////            if (!EditorApplication.isPlaying) {
////                return;
////            }
////
////            Handles.BeginGUI();
////            DrawDebugGUI();
////            Handles.EndGUI();
////        }
////
////        private void DrawDebugGUI() {
////
////            GUIContent[] contents;
////
////            if (target != null) {
////                Vector3 targetPosition = target.transform.position;
////                Vector3 diff = (targetPosition - transform.position);
////                Vector3 toTarget = diff.normalized;
////
////                float goalDot = Vector3.Dot(toTarget.normalized, transform.forward);
////                contents = new[] {
////                    new GUIContent(gameObject.name),
////                    new GUIContent("State: " + ((chart != null) ? chart.GetStatePath() : "No State Chart")),
////                    new GUIContent("Target: " + target.name),
////                    new GUIContent("Distance: " + Math.Round(diff.magnitude, 3)),
////                    new GUIContent("Distance: " + Math.Round(speed, 3)),
////                    new GUIContent("Dot: " + Math.Round(goalDot, 3)),
////                };
////            }
////            else {
////                contents = new[] {
////                    new GUIContent(gameObject.name)
////                };
////            }
////
////            float yOffset = 0;
////
////            for (int i = 0; i < contents.Length; i++) {
////                GUIContent content = contents[i];
////                Vector2 size = GUI.skin.label.CalcSize(content);
////                GUI.color = Color.yellow;
////                GUI.Label(new Rect(0, yOffset, size.x, size.y), content);
////                yOffset += size.y;
////            }
////        }
////
////        private void OnGUI() {
////            if (Selection.activeGameObject == gameObject) {
////                DrawDebugGUI();
////            }
////        }
//
//    }
//
//}