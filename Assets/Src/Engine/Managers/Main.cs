using System.Linq;
using SpaceGame.Events;
using SpaceGame.Systems;
using Weichx.Util;
using Src.Engine;
using UnityEngine;

namespace SpaceGame.Engine {

    [RequireComponent(typeof(WeaponManager))]
    [RequireComponent(typeof(DamageManager))]
    [RequireComponent(typeof(DestructionManager))]
    [RequireComponent(typeof(PropulsionManager))]
    [RequireComponent(typeof(AIManager))]
    public class Main : MonoBehaviour {

        private WeaponManager weaponManager;
        private DamageManager damageManager;
        private DestructionManager destroManager;
        private PropulsionManager propulsionManager;
        private AIManager aiManager;

        private void Awake() {
            WaypointPath[] waypoints = Resources.FindObjectsOfTypeAll<WaypointPath>();
            waypoints.ToList().ForEach((waypoint) => {
                if (!waypoint.gameObject.IsPrefab()) {
                    GameData.Instance.RegisterWaypointPath(waypoint);
                }
            });
            
            weaponManager = GetComponent<WeaponManager>();
            damageManager = GetComponent<DamageManager>();
            destroManager = GetComponent<DestructionManager>();
            propulsionManager = GetComponent<PropulsionManager>();
            aiManager = GetComponent<AIManager>();

            aiManager.Initialize();
            weaponManager.Initialize();
            damageManager.Initialize();
            destroManager.Initialize();
            propulsionManager.Initialize();

            EventSystem.Instance.AddListener<Evt_EntityArriving>(OnEntityArriving);
            EventSystem.Instance.AddListener<Evt_EntityDeparted>(OnEntityDeparted);
        }

        private void OnEntityArriving(Evt_EntityArriving evt) {
            EntityDatabase.GetEntityById(evt.entityId).gameObject.SetActive(true);
        }

        private void OnEntityDeparted(Evt_EntityDeparted evt) {
            EntityDatabase.GetEntityById(evt.entityId).gameObject.SetActive(false);
        }

        private void Update() {
            GameTimer.Instance.Tick();
            propulsionManager.Tick();

            EventSystem.Instance.Tick();
            weaponManager.Tick();
            EventSystem.Instance.Tick();
            // process collisions somewhere here 

            damageManager.Tick();
            EventSystem.Instance.Tick();

            destroManager.Tick();
            EventSystem.Instance.Tick();

            aiManager.Tick();
        }

    }

}