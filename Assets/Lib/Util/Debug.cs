using UnityEngine;
using System.Collections;
using System.Text;

namespace Weichx.Util {

    public static class DebugConsole {
        
        private static readonly StringBuilder builder = new StringBuilder();
        
        private static string StringifyArgs(params object[] list) {
            builder.Length = 0;
            for (int i = 0; i < list.Length; i++) {
                builder.Append(list[i]);
                builder.Append(" ");
            }
            return builder.ToString();
        }
        
        public static void Error(params object[] list) {
            Debug.LogError(StringifyArgs(list));    
        }
        
        public static void Warn(params object[] list) {
            Debug.LogWarning(StringifyArgs(list));
        }
        
        public static void Log(params object[] list) {
            Debug.Log(StringifyArgs(list));
        }    

    }
    
    public static class DebugDrawUtil {

        public static void DrawGizmoArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
            Gizmos.DrawRay(pos, direction);
            if (direction == Vector3.zero) return;

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void DrawGizmoArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
            Gizmos.color = color;
            Gizmos.DrawRay(pos, direction);
            if (direction == Vector3.zero) return;

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
            Debug.DrawRay(pos, direction);
            if (direction == Vector3.zero) return;

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Debug.DrawRay(pos + direction, right * arrowHeadLength);
            Debug.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
            Debug.DrawRay(pos, direction, color);
            if (direction == Vector3.zero) return;
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
            Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
        }

        public static void DrawPlane(Vector3 position, Vector3 normal) {
            Vector3 v3;

            if (normal.normalized != Vector3.forward) {
                v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
            }
            else {
                v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;
            }

            var corner0 = position + v3;
            var corner2 = position - v3;

            var q = Quaternion.AngleAxis(90.0f, normal);
            v3 = q * v3;
            var corner1 = position + v3;
            var corner3 = position - v3;

            Debug.DrawLine(corner0, corner2, Color.green);
            Debug.DrawLine(corner1, corner3, Color.green);
            Debug.DrawLine(corner0, corner1, Color.green);
            Debug.DrawLine(corner1, corner2, Color.green);
            Debug.DrawLine(corner2, corner3, Color.green);
            Debug.DrawLine(corner3, corner0, Color.green);
            Debug.DrawRay(position, normal, Color.red);
        }

    }

}