using UnityEngine;

namespace SpaceGame.Util {

    public static class Prediction {

        public static Vector3 Predict(Vector3 sPos, Vector3 tPos, Vector3 tLastPos, float pSpeed) {
            // Target velocity
            Vector3 tVel = (tPos - tLastPos) / Time.deltaTime;

            // Time to reach the target
            float flyTime = GetProjFlightTime(tPos - sPos, tVel, pSpeed);

            if (flyTime > 0)
                return tPos + flyTime * tVel;
            else
                return tPos;
        }

        public static float GetProjFlightTime(Vector3 dist, Vector3 tVel, float pSpeed) {
            float a = Vector3.Dot(tVel, tVel) - pSpeed * pSpeed;
            float b = 2.0f * Vector3.Dot(tVel, dist);
            float c = Vector3.Dot(dist, dist);

            float det = b * b - 4 * a * c;

            if (det > 0)
                return 2 * c / (Mathf.Sqrt(det) - b);
            else
                return -1;
        }
        
        public static float TimeToCollision(Vector3 position, Vector3 velocity, float radius, Vector3 otherPosition, Vector3 otherVelocity, float otherRadius) {
            float r = radius + (otherRadius * 1.25f); // 1.25 is a buffer
            Vector3 w = otherPosition - position;
            float c = Vector3.Dot(w, w) - r * r;
            if (c < 0) {
                return 0;
            }
            Vector3 v = velocity - otherVelocity;
            float a = Vector3.Dot(v, v);
            float b = Vector3.Dot(w, v);
            float discr = b * b - a * c;
            if (discr <= 0)
                return -1;
            float tau = (b - Mathf.Sqrt(discr)) / a;
            if (tau < 0)
                return -1;
            return tau;
        }
        
        public static float FindMaxArrivalSpeed(Vector2 currentDirection, Vector3 position, float turnRate, float accelerationRate, Vector3 goalDirection, Vector3 goalPosition) {
            float angle = Vector3.Angle(currentDirection, goalDirection) * Mathf.Deg2Rad;
            float distance = Vector3.Distance(position, goalPosition);
            float turnRateRadians = turnRate * Mathf.Deg2Rad;
            float speed = turnRateRadians * (distance * 0.5f) / Mathf.Cos(angle);
            speed += speed * accelerationRate;
            if (speed < 0) speed = -speed;
            return speed;
        }

        public static bool ReachableAtSpeed(Vector3 position, Vector3 forward, Vector3 target, float speed, float turnRateRadians) {
            float radius = (speed / turnRateRadians) * 2f; // I don't remember why this is doubled
            Vector3 toTarget = target - position;
            Vector3 projectedToTarget = Vector3.ProjectOnPlane(toTarget, forward).normalized * radius;
            Vector3 sphereCenter = position + projectedToTarget;
            return (sphereCenter - target).sqrMagnitude >= radius * radius;
        }

    }

}