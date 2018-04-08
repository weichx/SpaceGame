using SpaceGame.AI;
using SpaceGame.Missions;
using UnityEngine;
using Weichx.Util;

namespace SpaceGame.Engine {

    [RequireComponent(typeof(WeaponManager))]
    [RequireComponent(typeof(DamageManager))]
    [RequireComponent(typeof(PropulsionManager))]
    public class Main : MonoBehaviour {

        private WeaponManager weaponManager;
        private DamageManager damageManager;
        private PropulsionManager propulsionManager;
        private IntelligenceSystem intelligenceSystem;
        
        private void Awake() {

            GameDatabase.ActiveInstance.ClearSceneEntities();
            GameDatabase.ActiveInstance.UpdateSceneEntities();
            
            weaponManager = GetComponent<WeaponManager>();
            damageManager = GetComponent<DamageManager>();
            propulsionManager = GetComponent<PropulsionManager>();
            intelligenceSystem = new IntelligenceSystem();
            
            intelligenceSystem.Initialize();

            EntityDefinition[] entityDefinitions = GameDatabase.ActiveInstance.GetCurrentMission().GetLinkedEntities();
            
            
            for (int i = 0; i < entityDefinitions.Length; i++) {
                EntityDefinition entityDefinition = entityDefinitions[i];
                Entity entity = GameData.Instance.CreateEntity(entityDefinition);
                if (entity != null) {
                    intelligenceSystem.CreateAgent(entity, entityDefinition);
                }
            }

            weaponManager.Initialize();
            damageManager.Initialize();
            propulsionManager.Initialize();

        }

        private void Update() {
            GameTimer.Instance.Tick();
            EventSystem.Instance.Tick();
            propulsionManager.Tick();
            weaponManager.Tick();
            // process collisions somewhere here 
            damageManager.Tick();
            intelligenceSystem.Tick();
        }

    }

}