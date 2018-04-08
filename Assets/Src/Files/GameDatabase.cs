using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Compatibility;
using SpaceGame.Assets;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Weichx.Util;
using Object = UnityEngine.Object;

namespace SpaceGame {

    //todo it would be good to seperate / back up fields
    public class GameDatabase {

        private const string UnlinkedString = " [Unlinked]";
        private static object[] constructorParams = new object[2];
        private static Type[] signature = { typeof(int), typeof(string) };

        [SerializeField] private MissionDefinition currentMission;
        [SerializeField] private int idGenerator;

        private BookKeeper<Entity, int> sceneEntityToSceneIdMap;
        private BookKeeper<Entity, EntityDefinition> sceneEntityToDefinitionMap;

        [SerializeField] private AssetMap<FactionDefinition> factionAssetMap;
        [SerializeField] private AssetMap<EntityDefinition> entityAssetMap;
        [SerializeField] private AssetMap<FlightGroupDefinition> flightGroupAssetMap;

        [SerializeField] private AssetMap<BehaviorSet> behaviorSetAssetMap;

        [SerializeField] private AssetMap<ShipType> shipTypeAssetMap;
        [SerializeField] private AssetMap<ShipTypeGroup> shipTypeGroupAssetMap;

        private ListX<Entity> sceneEntities;
        private ListX<PrefabRef> prefabs;

        public GameDatabase() {
            this.idGenerator = 0;

            this.prefabs = new ListX<PrefabRef>();
            this.sceneEntities = new ListX<Entity>();

            this.factionAssetMap = new AssetMap<FactionDefinition>();
            this.flightGroupAssetMap = new AssetMap<FlightGroupDefinition>();
            this.entityAssetMap = new AssetMap<EntityDefinition>();

            this.behaviorSetAssetMap = new AssetMap<BehaviorSet>();

            this.shipTypeAssetMap = new AssetMap<ShipType>();
            this.shipTypeGroupAssetMap = new AssetMap<ShipTypeGroup>();

            this.sceneEntityToSceneIdMap = new BookKeeper<Entity, int>();
            this.sceneEntityToDefinitionMap = new BookKeeper<Entity, EntityDefinition>();
        }

        public static GameDatabase ActiveInstance { get; set; }

        public enum AssetType {

            Unknown,
            EntityDefinition,
            SceneEntity,
            FlighGroupDefinition,
            FactionDefinition,
            ShipType,
            ShipTypeGroup,
            Chassis,
            BehaviorSet

        }

        private AssetType GetAssetType(Type type) {
            if (type == typeof(EntityDefinition)) return AssetType.EntityDefinition;
            if (type == typeof(FlightGroupDefinition)) return AssetType.FlighGroupDefinition;
            if (type == typeof(FactionDefinition)) return AssetType.FactionDefinition;

            if (type == typeof(BehaviorSet)) return AssetType.BehaviorSet;

            if (type == typeof(ShipType)) return AssetType.ShipType;
            if (type == typeof(ShipTypeGroup)) return AssetType.ShipTypeGroup;

            if (type == typeof(Entity)) return AssetType.SceneEntity;
            if (type == typeof(Chassis)) return AssetType.Chassis;
            return AssetType.Unknown;
        }

        private AssetType GetAssetType<T>() where T : Asset {
            return GetAssetType(typeof(T));
        }

        private AssetMap<T> GetAssetMap<T>() where T : Asset {
            return GetAssetMap(typeof(T)) as AssetMap<T>;
        }

        private AssetMap GetAssetMap(Type type) {
            if (type == typeof(EntityDefinition)) return entityAssetMap;
            if (type == typeof(FlightGroupDefinition)) return flightGroupAssetMap;
            if (type == typeof(FactionDefinition)) return factionAssetMap;

            if (type == typeof(BehaviorSet)) return behaviorSetAssetMap;

            if (type == typeof(ShipType)) return shipTypeAssetMap;
            if (type == typeof(ShipTypeGroup)) return shipTypeGroupAssetMap;

            return null;
        }

        private const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private int GetNextIdForType(Type type) {
            return type.IsSubclassOf(typeof(MissionAsset)) ? GetCurrentMission().NextAssetId : ++idGenerator;
        }

        public T CreateAsset<T>() where T : Asset {
            constructorParams[0] = GetNextIdForType(typeof(T));
            constructorParams[1] = $"{StringUtil.NicifyName(typeof(T).Name, "Asset")} {idGenerator}";
            T instance = (T) typeof(T).GetConstructor(bindFlags, null, signature, null)?.Invoke(constructorParams);
            Debug.Assert(instance != null, $"GameAssets must have an (int, string) constructor. {typeof(T).Name} does not.");
            GetAssetMap<T>().Add(instance);
            switch (GetAssetType<T>()) {
                case AssetType.BehaviorSet:
                    break;
                case AssetType.EntityDefinition:
                    break;
                case AssetType.FlighGroupDefinition:
                    break;
                case AssetType.FactionDefinition:
                    break;
                case AssetType.ShipType:
                    break;
                case AssetType.ShipTypeGroup:
                    break;
            }
            return instance;
        }

        public T CreateStandaloneAsset<T>() where T : Asset {
            constructorParams[0] = GetNextIdForType(typeof(T));
            constructorParams[1] = $"{StringUtil.NicifyName(typeof(T).Name, "Asset")} {idGenerator}";
            T instance = (T) typeof(T).GetConstructor(bindFlags, null, signature, null)?.Invoke(constructorParams);
            Debug.Assert(instance != null, $"GameAssets must have an (int, string) constructor. {typeof(T).Name} does not.");
            return instance;
        }

        public T FindOrCreate<T>(int selectedId) where T : Asset {
            return FindAsset<T>(selectedId) ?? CreateAsset<T>();
        }

        public T FindAsset<T>(int selectedId) where T : Asset {
            return GetAssetMap<T>().Get(selectedId);
        }

        public T DestroyAsset<T>(int id) where T : Asset {
            return DestroyAsset(FindAsset<T>(id)) as T;
        }

        public Asset DestroyAsset(Asset asset) {
            GetAssetMap(asset.GetType()).Remove(asset);
            switch (GetAssetType(asset.GetType())) {
                case AssetType.EntityDefinition:
                    EntityDefinition ed = (EntityDefinition) asset;
                    FindAsset<FlightGroupDefinition>(ed.flightGroupId)?.entities.Remove(ed);
                    break;
                case AssetType.FlighGroupDefinition:
                    FlightGroupDefinition fg = (FlightGroupDefinition) asset;
                    FindAsset<FactionDefinition>(fg.factionId)?.flightGroups.Remove(fg);
                    break;
                case AssetType.ShipType:
                    ShipType shipType = (ShipType) asset;
                    FindAsset<ShipTypeGroup>(shipType.shipGroupId)?.ships.Remove(shipType);
                    break;
            }
            return asset;
        }

        public void DuplicateAsset<T>(int id) where T : GameAsset {
            throw new NotImplementedException();
        }

        // only supporting one mission right now
        public MissionDefinition GetCurrentMission() {
            if (currentMission != null) return currentMission;
            currentMission = CreateAsset<MissionDefinition>();
            return currentMission;
        }

        public void ClearSceneEntities() {
            sceneEntities.Clear();
            sceneEntityToDefinitionMap.Clear();
            sceneEntityToSceneIdMap.Clear();
        }

        public void UpdateProjectAssets() {

            prefabs.Clear();
            
            ListX<Chassis> chassis = new ListX<Chassis>(Resources.FindObjectsOfTypeAll<Chassis>().Where(EditorUtility.IsPersistent));

            for (int i = 0; i < chassis.Count; i++) {
                GameObject prefabRoot = chassis[i].gameObject;
                string path = AssetDatabase.GetAssetPath(prefabRoot);
                prefabs.Add(new PrefabRef(AssetDatabase.AssetPathToGUID(path), prefabRoot));
            }

        }

        public void ClearProjectAssets() {
            prefabs.Clear();
        }

        private void CollectSceneEntities() {
            sceneEntities.Clear();
            Entity[] entities = Resources.FindObjectsOfTypeAll<Entity>();

            for (int i = 0; i < entities.Length; i++) {
                if (!EditorUtility.IsPersistent(entities[i])) {
                    sceneEntities.Add(entities[i]);
                }
            }

            List<Entity> list = sceneEntityToSceneIdMap.GetTValues();
            List<Entity> result = list.Where(p => !sceneEntities.Contains(p)).ToList();

            foreach (Entity entity in result) {
                sceneEntityToDefinitionMap.Remove(entity);
                sceneEntityToSceneIdMap.Remove(entity);
            }

            if (sceneEntityToSceneIdMap.Count == 0) {
                foreach (Entity entity in sceneEntities) {
                    if (entity.id == 0) {
                        entity.id = GetCurrentMission().AllocateEntityId();
                    }
                    sceneEntityToSceneIdMap.Set(entity, entity.id);
                    if (!Application.isPlaying) {
                        EditorUtility.SetDirty(entity);
                    }
                }
                if (!Application.isPlaying) {
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }

        }

        public void UpdateSceneEntities() {
            CollectSceneEntities();

            bool didChange = false;
            for (int i = 0; i < sceneEntities.Count; i++) {

                int storedId;

                Entity sceneEntity = sceneEntities[i];
                EntityDefinition definition;

                Chassis entityChassis = sceneEntity.GetComponent<Chassis>();
                if (entityChassis != null) {
                    Debug.Log($"Removed Chassis from {sceneEntity.name} because you cannot have both an entity and a Chassis component");
                    Object.DestroyImmediate(entityChassis);
                }

                int currentId = sceneEntity.id;

                if (sceneEntityToSceneIdMap.TryGetValue(sceneEntity, out storedId)) {
                    Debug.Assert(currentId == storedId, $"Should never hit this. Expected {currentId} to be {storedId}");
                    definition = GetEntityDefinitionForSceneEntity(sceneEntity);
                }
                else {
                    Debug.Log(sceneEntity.name);
                    didChange = true;
                    sceneEntity.id = GetCurrentMission().AllocateEntityId();
                    EditorUtility.SetDirty(sceneEntity); // todo -- dev only
                    if (currentId != 0) {
                        Debug.Log("Duplicated");
                        EntityDefinition toClone = GetEntityDefinitionForSceneEntity(currentId);
                        definition = currentMission.CloneEntityDefinition(toClone);
                    }
                    else {
                        Debug.Log("Created");
                        definition = currentMission.CreateEntityDefinition();
                    }
                }

                // if a new one is created, add a definition and link
                // if a one is duplicated, clone definition and link
                // we *might* have unlinked scene entities which is ok

                sceneEntityToSceneIdMap.Set(sceneEntity, sceneEntity.id);
                sceneEntityToDefinitionMap.Set(sceneEntity, definition);

                SetSceneEntityName(sceneEntity, definition);

            }

            GetCurrentMission().Change();

            if (didChange) {
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }

        public EntityDefinition GetEntityDefinitionForSceneEntity(int id) {
            return GetCurrentMission().GetEntities().Find(id, (entity, i) => entity.sceneEntityId == i);
        }

        public EntityDefinition GetEntityDefinitionForSceneEntity(Entity sceneEntity) {
            return GetCurrentMission().GetEntities().Find(sceneEntity.id, (entity, i) => entity.sceneEntityId == i);
        }

        public Entity FindSceneEntityById(int sceneEntityId) {
            if (sceneEntityId == -1) return null;
            // maybe also check the mission id
            // Unity likes to sneakily destroy things :(
            Entity entity = sceneEntityToSceneIdMap.Get(sceneEntityId);
            if (entity == null) return null;
            return entity;
        }

        public void SetShipTypeShipGroup(ShipType shipType, int groupId, int index = -1) {
            int oldGroupId = shipType.shipGroupId;
            shipType.shipGroupId = groupId;
            ShipTypeGroup shipGroup = shipTypeGroupAssetMap.Get(groupId);

            if (oldGroupId == shipGroup.id) {
                shipGroup.ships.MoveToIndex(shipType, index);
            }
            else {
                ShipTypeGroup oldGroup = shipTypeGroupAssetMap.Get(oldGroupId);
                oldGroup?.ships.Remove(shipType);
                shipGroup.AddShipDefinition(shipType, index);
            }
        }

        /*
         * Todo -- Figure out Chassis handling w/ ids and ship types
         * Todo -- Figure out weapons loadout w/ ship type / chassis
         * Todo -- Change GameDatabase to initialize on load and handle what mission window handles right now
         * Todo -- Multiple save files / locations in case we fuck up
         * Todo -- Name generator from Faction / Flight Group
         * Todo -- Allow Template Entity Definitions, mark appropriately
         * Todo -- Index hierarchy lists w/ dictionaries instead of n ^ 3 search
         */

        private void SetShipTypeFromChassis(Entity sceneEntity, EntityDefinition entityDefinition) {
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
            IReadonlyListX<ShipType> shipTypes = shipTypeAssetMap.GetList();
            for (int i = 0; i < shipTypes.Count; i++) {
                ShipType shipType = shipTypes[i];
                if (shipType.chassis.assetPath == path) {
                    Debug.Log("Found it");
                    return;
                }
            }
            Debug.Log("Nope");
        }

        private void SetChassisFromShipType(Entity sceneEntity, string chassisGuid) {
            if (sceneEntity == null || string.IsNullOrEmpty(chassisGuid)) return;

            GameObject prefab = GetPrefabAsset(chassisGuid);
            
            if (prefab == null) {
                return;
            }
            
            sceneEntity.transform.ClearChildrenImmediate();
            GameObject newModel = Object.Instantiate(prefab, sceneEntity.transform);
            newModel.transform.localPosition = Vector3.zero;
            newModel.transform.localRotation = Quaternion.identity;
        }

        private void SetSceneEntityName(Entity sceneEntity, EntityDefinition definition) {
            if (sceneEntity == null) return;
            if (definition == null) {
                if (sceneEntity.name.IndexOf(UnlinkedString, StringComparison.Ordinal) == -1) {
                    sceneEntity.name += UnlinkedString;
                }
            }
            else if (sceneEntityToDefinitionMap.Get(sceneEntity) == definition) {
                // heirarchy updates like crazy if we don't do this check
                // because OnHierchyChanged fires on rename (which this is)
                if (sceneEntity.name != definition.name) {
                    sceneEntity.name = definition.name;
                }
            }
            else {
                definition.name = sceneEntity.name.Replace(UnlinkedString, string.Empty);
                sceneEntity.name = definition.name;
            }
        }

        public void LinkSceneEntity(Entity sceneEntity, EntityDefinition entityDefinition) {
            Debug.Assert(entityDefinition != null, "entityDefinition != null");

            SetSceneEntityName(sceneEntity, entityDefinition);
            UpdateSceneEntityBookKeeping(sceneEntity, entityDefinition);

            if (sceneEntity == null) return;

            if (sceneEntity.id == entityDefinition.sceneEntityId) {
                // update
                SetChassisFromShipType(sceneEntity, entityDefinition.chassisGuid);
            }
            else {
                // new assignment
                SetShipTypeFromChassis(sceneEntity, entityDefinition);
            }

        }

        private void UpdateSceneEntityBookKeeping(Entity sceneEntity, EntityDefinition entityDefinition) {
            if (sceneEntity == null && entityDefinition == null) return;

            sceneEntityToDefinitionMap.Set(sceneEntity, entityDefinition);
            entityDefinition.sceneEntityId = sceneEntity == null ? -1 : sceneEntity.id;

        }

        public Entity SceneEntityFromDefinitionId(int entityDefinitionId) {
            EntityDefinition ed = GetCurrentMission().GetEntities().Find((e) => e.id == entityDefinitionId);
            if (ed == null) return null;
            return FindSceneEntityById(ed.sceneEntityId);
        }

        public IReadonlyListX<T> GetAssetList<T>() where T : Asset {
            return GetAssetMap<T>().GetList();
        }

        public void SetAssetIndex<T>(T asset, int index) where T : Asset {
            switch (GetAssetType<T>()) {
                case AssetType.FactionDefinition:
                    //factionAssetMap.SetIndex(asset as T, index);
                    break;
            }
        }

        public GameObject GetPrefabAsset(string assetGuid) {
            if (string.IsNullOrEmpty(assetGuid)) {
                return null;
            }
            int idx = prefabs.FindIndex((prefabref) => prefabref.guid == assetGuid);
            if (idx == -1) {
                return null;
            }
            return prefabs[idx].prefab;
        }

        private struct PrefabRef {

            public readonly string guid;
            public readonly GameObject prefab;

            public PrefabRef(string guid, GameObject prefab) {
                this.guid = guid;
                this.prefab = prefab;
            }

        }

    }

}