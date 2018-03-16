using System.Collections.Generic;
using SpaceGame.Editor.Reflection;
using SpaceGame.FileTypes;
using SpaceGame.Util;
using UnityEditor;
using UnityEngine;
using Weichx.Persistence;

namespace SpaceGame.Editor.MissionWindow {

    public class MissionWindowState {

        private const string ResourcePath = "Missions/Mission";
        private const string AssetPath = "Assets/Resources/Missions/Mission.asset";
        private const string ENTITY_DEFINITIONS = nameof(MissionDefinition.entityDefinitions);
        
        [SerializeField] private string currentMissionGuid;
        public int currentPageIndex;

        private MissionDataFile m_missionDataFile;
        private ReflectedObject m_currentMissionReflected;
        private List<ReflectedObject> m_reflectedMissions;

        public IReadOnlyList<ReflectedObject> missions => m_reflectedMissions;

        public ReflectedObject currentMission {
            get { return m_currentMissionReflected; }
            set {
                if (value == null) return;
                if (m_reflectedMissions.Contains(value)) {
                    m_currentMissionReflected = value;
                    currentMissionGuid = (string) (m_currentMissionReflected[nameof(MissionDefinition.guid)].Value);
                }
            }
        }

        public string[] GetMissionNames() {
            return m_reflectedMissions.MapArray((mission) => {
                return mission[nameof(ReflectedProperty.name)].Value as string;
            });
        }

        private void Load() {

            m_missionDataFile = Resources.Load<MissionDataFile>(ResourcePath);

            if (m_missionDataFile == null) {
                m_missionDataFile = MissionDataFile.Create("Mission");
                AssetDatabase.CreateAsset(m_missionDataFile, AssetPath);
                AssetDatabase.SaveAssets();
            }

            List<MissionDefinition> missionList = m_missionDataFile.GetMissions();
            MissionDefinition selectedMission = null;

            if (!string.IsNullOrEmpty(currentMissionGuid)) {
                selectedMission = missionList.Find(currentMissionGuid, (mission, guid) => {
                    return mission.guid == guid;
                });
            }

            if (selectedMission == null) selectedMission = missionList[0];

            m_reflectedMissions = missionList.Map((mission) => {
                return new ReflectedObject(mission);
            });

            m_currentMissionReflected = m_reflectedMissions.Find((mission) => mission.Value == selectedMission);
            Debug.Assert(m_currentMissionReflected != null, "m_currentMissionReflected != null");
            currentMissionGuid = (string) (m_currentMissionReflected[nameof(MissionDefinition.guid)].Value);

        }

        public void AddMission() {
            m_reflectedMissions.Add(new ReflectedObject(new MissionDefinition()));
        }

        public void RemoveMission(ReflectedProperty mission) { }

        public void SaveMission() {
            if (m_currentMissionReflected != null) {
                m_currentMissionReflected.ApplyModifiedProperties();
                m_missionDataFile.SaveMission((MissionDefinition) m_currentMissionReflected.Value);
            }
        }

        public void AddEntityDefinition(EntityDefinition entityDefinition) {
            ((ReflectedListProperty) currentMission[ENTITY_DEFINITIONS]).AddArrayElement(entityDefinition);    
        }
        
        public void Save(string key) {
            EditorPrefs.SetString(key, Snapshot<MissionWindowState>.Serialize(this));
        }

        public static MissionWindowState Restore(string key) {
            string serializedState = EditorPrefs.GetString(key);
            MissionWindowState state;
            if (string.IsNullOrEmpty(serializedState)) {
                state = new MissionWindowState();
            }
            else {
                state = Snapshot<MissionWindowState>.Deserialize(serializedState);
            }
            state.Load();
            return state;
        }

        public void Save() {
            EditorPrefs.SetString("MissionWindowState", Snapshot<MissionWindowState>.Serialize(this));
        }

        public static MissionWindowState Restore() {
            string stateString = EditorPrefs.GetString("MissionWindowState");
            if (!string.IsNullOrEmpty(stateString)) {
                return Snapshot<MissionWindowState>.Deserialize(stateString);
            }
            return new MissionWindowState();
        }

    }

}