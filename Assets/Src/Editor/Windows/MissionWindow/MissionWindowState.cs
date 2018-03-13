using System.Collections.Generic;
using SpaceGame.FileTypes;
using SpaceGame.Util;
using UnityEditor;
using UnityEngine;
using Weichx.Persistence;

namespace SpaceGame.Editor.MissionWindow {

    public class MissionWindowState {

        public string currentMissionName;

        private List<MissionDefinition> missions;
        private List<EntityDefinition> entityDefinitions;

        public MissionWindowState() {
            MissionDefinition[] loadedMissions = Resources.LoadAll<MissionDefinition>("Missions");
            this.missions = new List<MissionDefinition>(loadedMissions);
        }

        public MissionDefinition CurrentMission {
            get {
                if (currentMissionName == null) return null;
                return missions.Find(currentMissionName, (m, n) => {
                    return m.name == n;
                });
            }
            set { currentMissionName = value != null ? value.name : null; }
        }

        public List<EntityDefinition> GetEntityDefinitions() {
            MissionDefinition currentMission = CurrentMission;
            if (currentMission == null) return null;
            return entityDefinitions ?? (entityDefinitions = currentMission.GetEntityDefinitions());
        }

        public void CreateEntityDefinition() {
            GameObject entityObject = new GameObject("Entity");
            entityObject.transform.Reset();
            Entity entity = entityObject.AddComponent<Entity>();
            entityDefinitions.Add(new EntityDefinition(entity));
        }

        public void SetEntityDefinitions(List<EntityDefinition> entityDefinitions) {
            this.entityDefinitions = entityDefinitions;
        }

        public string GetMissionName() {
            MissionDefinition currentMission = CurrentMission;
            return (currentMission != null) ? currentMission.name : "No Mission Selected";
        }

        public void SaveEntityDefinitions() {
            if (currentMissionName == null || entityDefinitions == null) return;
            if (CurrentMission != null) {
                CurrentMission.SetEntityDefinitions(entityDefinitions);
            }
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