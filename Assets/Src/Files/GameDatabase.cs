using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lib.Util;
using SpaceGame.Assets;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Weichx.Util;
using Object = UnityEngine.Object;

namespace SpaceGame {

    //todo mabye [instantiateonload] this
    //todo it would be good to seperate / back up fields
    public class GameDatabase {

        private static object[] constructorParams = new object[2];
        private static Type[] signature = { typeof(int), typeof(string) };
        
        public readonly List<ShipTypeGroup> shipTypeGroups;
        public readonly List<MissionDefinition> missionDefinitions;
        public readonly List<ShipType> shipTypes;

        public readonly List<BehaviorSet> behaviorSets;
        public readonly List<BehaviorDefinition> behaviorDefinitions;
        public readonly List<ActionDefinition> actionDefinitions;

        [SerializeField] private MissionDefinition currentMission;
        [SerializeField] private int idGenerator;

        private Dictionary<int, Entity> idToSceneEntity;
        private Dictionary<Entity, int> sceneEntityToId;
        private ListX<Entity> sceneEntities;
        
        public GameDatabase() {
            this.idGenerator = 0;
            this.shipTypeGroups = new List<ShipTypeGroup>();
            this.shipTypes = new List<ShipType>();
            this.missionDefinitions = new List<MissionDefinition>();
            this.behaviorSets = new List<BehaviorSet>(4);
            this.actionDefinitions = new List<ActionDefinition>(4);
            this.behaviorDefinitions = new List<BehaviorDefinition>(4);
            this.idToSceneEntity = new Dictionary<int, Entity>();
            this.sceneEntityToId = new Dictionary<Entity, int>();
            this.sceneEntities = new ListX<Entity>();
        }

        public static GameDatabase ActiveInstance { get; set; }

        private List<T> GetList<T>() {
            Type type = typeof(T);
            if (type == typeof(BehaviorSet)) return behaviorSets as List<T>;
            if (type == typeof(BehaviorDefinition)) return behaviorDefinitions as List<T>;
            if (type == typeof(ActionDefinition)) return actionDefinitions as List<T>;
            if (type == typeof(ShipTypeGroup)) return this.shipTypeGroups as List<T>;
            if (type == typeof(ShipType)) return this.shipTypes as List<T>;
            if (type == typeof(MissionDefinition)) return this.missionDefinitions as List<T>;
            return null;
        }

        private const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public T CreateAsset<T>() where T : GameAsset {
            constructorParams[0] = ++idGenerator;
            constructorParams[1] = $"{StringUtil.NicifyName(typeof(T).Name, "Asset")} {idGenerator}";
            T instance = (T) typeof(T).GetConstructor(bindFlags, null, signature, null)?.Invoke(constructorParams);
            Debug.Assert(instance != null, $"GameAssets must have an (int, string) constructor. {typeof(T).Name} does not.");
            GetList<T>().Add(instance);
            return instance;
        }

        public T FindOrCreate<T>(int selectedId) where T : GameAsset {
            return FindAsset<T>(selectedId) ?? CreateAsset<T>();
        }

        public T FindAsset<T>(int selectedId) where T : GameAsset {
            return GetList<T>().Find(selectedId, (def, id) => def.id == id);
        }

        public void DestroyAsset(GameAsset asset) {
            if (asset is BehaviorSet) {
                behaviorSets.Remove((BehaviorSet) asset);
            }
            else if (asset is BehaviorDefinition) {
                BehaviorDefinition behaviorDefinition = ((BehaviorDefinition) asset);
                int id = behaviorDefinition.behaviorSetId;
                BehaviorSet behaviorSet = (
                    from bs in behaviorSets
                    where bs.id == id
                    select bs
                ).FirstOrDefault();
                behaviorSet?.behaviors.Remove(behaviorDefinition);
                behaviorDefinitions.Remove(behaviorDefinition);
            }
            else if (asset is ActionDefinition) {
                ActionDefinition actionDefinition = (ActionDefinition) asset;
                int id = actionDefinition.behaviorId;
                BehaviorDefinition behaviorDefinition = (
                    from bs in behaviorSets
                    from bh in bs.behaviors
                    where bh.id == id
                    select bh
                ).FirstOrDefault();
                behaviorDefinition?.actions.Remove(actionDefinition);
                actionDefinitions.Remove(actionDefinition);
            }
            else if (asset is ShipTypeGroup) {
                shipTypeGroups.Remove((ShipTypeGroup) asset);
            }
            else if (asset is ShipType) {
                ShipType shipType = (ShipType) asset;
                ShipTypeGroup shipGroup = FindAsset<ShipTypeGroup>(shipType.shipGroupId);
                shipGroup?.ships.Remove(shipType);
                shipTypes.Remove(shipType);
            }

        }

        public void DestroyAsset<T>(int id) where T : GameAsset {
            List<T> list = GetList<T>();
            int index = list.FindIndex((def) => def.id == id);
            if (index != -1) {
                DestroyAsset(list[index]);
            }
        }

        public List<ShipTypeGroup> GetShipTypeGroups() {
            return shipTypeGroups;
        }

        public void SetCurrentMission(MissionDefinition mission) {
            this.currentMission = mission;
        }

        public MissionDefinition GetCurrentMission() {
            if (currentMission != null) return currentMission;
            if (missionDefinitions.Count > 0) {
                currentMission = missionDefinitions[0];
            }
            else {
                currentMission = CreateAsset<MissionDefinition>();
            }
            return currentMission;

        }

        public void ClearSceneEntities() {
            sceneEntities.Clear();
            idToSceneEntity.Clear();
            sceneEntityToId.Clear();
        }
        
        public void UpdateSceneEntities() {
            sceneEntities.Clear();
            Entity[] entities = Resources.FindObjectsOfTypeAll<Entity>();

            for (int i = 0; i < entities.Length; i++) {
                if (!EditorUtility.IsPersistent(entities[i])) {
                    sceneEntities.Add(entities[i]);
                }
            }
            
            if (sceneEntityToId.Count == 0) {
                foreach (Entity entity in sceneEntities) {
                    if (entity.id == 0) {
                        entity.id = GetCurrentMission().AllocateEntityId();
                    }
                    idToSceneEntity.Add(entity.id, entity);
                    sceneEntityToId.Add(entity, entity.id);
                    EditorUtility.SetDirty(entity);
                }

                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            
            bool didChange = false;
            for (int i = 0; i < sceneEntities.Count; i++) {
                Entity entity = sceneEntities[i];
                int currentId = entity.id;
                int storedId;
                if (sceneEntityToId.TryGetValue(entity, out storedId)) {
                    Debug.Assert(currentId == storedId, $"Should never hit this. Expected {currentId} to be {storedId}");
                }
                else {
                    entity.id = GetCurrentMission().AllocateEntityId();
                    sceneEntityToId.Add(entity, entity.id);
                    EditorUtility.SetDirty(entity); // todo -- dev only
                    didChange = true;
                    if (currentId != 0) {
                        Debug.Log("Duplicated");
//                        db.GetCurrentMission().CloneEntityDefinition(en)
                        // gameData.GetMission(state.activeMissionGuid).CloneEntityDefinition(entity.guid, newGuid);
                    }
                    else {
                        Debug.Log("Created");
                        currentMission.CreateEntityDefinition();
                    }
                }
            }
            if (didChange) {
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }

        public EntityDefinition GetEntityDefinitionForSceneEntity(int id) {
            return GetCurrentMission().GetEntities().Find(id, (entity, i) => entity.sceneEntityId == i);
        }
        
        public Entity FindSceneEntityById(int sceneEntityId) {
            return idToSceneEntity.Get(sceneEntityId);
        }
        
        public void SetShipTypeShipGroup(ShipType shipType, int groupId, int index = -1) {
            int oldGroupId = shipType.shipGroupId;
            shipType.shipGroupId = groupId;
            ShipTypeGroup shipGroup = shipTypeGroups.Find(groupId, (group, id) => group.id == id);
            if (oldGroupId == shipGroup.id) {
                shipGroup.ships.MoveToIndex(shipType, index);
            }
            else {
                ShipTypeGroup oldGroup = shipTypeGroups.Find(oldGroupId, (group, id) => group.id == id);
                oldGroup?.ships.Remove(shipType);
                shipGroup.AddShipDefinition(shipType, index);
            }
        }

        // todo -- probably needs work
        public void UpdateSceneEntityFromDefinition(Entity sceneEntity, EntityDefinition definition) {
            if (sceneEntity == null || definition == null) return;
            ShipType shipType = FindAsset<ShipType>(definition.shipTypeId);
            if (shipType != null) {
                Chassis chassis = shipType.chassis.GetAsset();
                if (chassis != null) {
                    GameObject obj = chassis.gameObject;
                    sceneEntity.transform.ClearChildrenImmediate();
                    GameObject newModel = Object.Instantiate(obj, sceneEntity.transform);
                    newModel.transform.localPosition = Vector3.zero;
                    newModel.transform.localRotation = Quaternion.identity;
                }
            }
        }

        /*
         * Todo -- Link Scene name to entity definition name w/ notification of unlinked scene entities
         * Todo -- Figure out Chassis handling w/ ids and ship types
         * Todo -- Figure out weapons loadout w/ ship type / chassis
         * Todo -- Change GameDatabase to initialize on load and handle what mission window handles right now
         * Todo -- Multiple save files / locations in case we fuck up
         * Todo -- Name generator from Faction / Flight Group
         * Todo -- Allow Template Entity Definitions, mark appropriately
         * Todo -- Index hierarchy lists w/ dictionaries instead of n ^ 3 search
         */
        
        // todo -- definitely needs work
        public void UpdateDefinitionFromSceneEntity(EntityDefinition definition, Entity sceneEntity) {
            if (sceneEntity == null || definition == null) return;
            Chassis chassis = sceneEntity.GetComponentInChildren<Chassis>();
            GameObject chassisGameObject = chassis?.gameObject;
            if (chassisGameObject == null) {
                return;
            }

            Object prefabRoot = PrefabUtility.GetPrefabParent(chassisGameObject);
            string path = AssetDatabase.GetAssetPath(prefabRoot);
            Debug.Log(path);
            path = path.Replace("Assets/Resources/", "");
            Debug.Log(path);
            // todo -- chassis needs an id! its possible to have 2 ship types w/ the same chassis asset
            for (int i = 0; i < shipTypes.Count; i++) {
                ShipType shipType = shipTypes[i];
                if (shipType.chassis.assetPath == path) {
                    Debug.Log("Found it");
                    return;
                }

            }
            Debug.Log("Nope");
        }

    }

}