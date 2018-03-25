using SpaceGame.AI;
using SpaceGame.EditorComponents;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Weichx.EditorReflection;
using Weichx.Persistence;
using Debug = System.Diagnostics.Debug;

namespace SpaceGame.Editor {
//
//    [CustomEditor(typeof(Agent))]
//    public class AgentEditor : UnityEditor.Editor {
//
//        private ReflectedObject obj;
//    
//        private void OnEnable() {
//            Agent agent = target as Agent;
//            Debug.Assert(agent != null, nameof(agent) + " != null");
//            agent.agentData = Snapshot<AgentData>.Deserialize(agent.serializedData);
//            obj = new ReflectedObject(agent.agentData);    
//        }
//
//        public override void OnInspectorGUI() {
//            ReflectedListProperty list = obj.FindChild("decisions") as ReflectedListProperty;
//            int childCount = list.ChildCount;
//            obj.Root.IsExpanded = true;
//            EditorGUILayout.BeginVertical(GUILayout.MinHeight(obj.Root.Drawer.GetPropertyHeight(obj.Root)));
//            if (list.Value == null) list.Value = new Decision[1];
//            for (int i = 0; i < childCount; i++) {
//                if (list[i].Value == null) {
//                    list[i].Value = new Decision();
//                    list[i].Update();
//                }
//            }
//            EditorGUILayoutX.DrawProperties(obj);
//            EditorGUILayout.EndVertical();
//        }
//
//        private void OnDisable() {
//            obj.ApplyModifiedProperties();
//            ((Agent) target).serializedData = Snapshot<AgentData>.Serialize(obj.Value as AgentData);
//            if (target != null && !EditorApplication.isPlaying) {
//                EditorUtility.SetDirty(target);
//                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
//            }
//        }
//
//    }



    
}