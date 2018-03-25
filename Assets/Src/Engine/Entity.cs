using System;
using System.Diagnostics;
using SpaceGame.AI;
using SpaceGame.Engine;
using SpaceGame.Weapons;
using UnityEngine;
using Weichx.ReflectionAttributes;
using Weichx.Util;

namespace SpaceGame {
    /*
     *
     * Entity
     *     - Ship Model
     *     - Ship Stats
     *     - AI Behaviors
     *     - AI Preferences
     *     - AI Goals
     *     - Weapon System [Hardpoints]
     *     - Weapon Loadout
     *     - Weapon Overrides
     *     - Faction
     *     - Flight Group
     *     - Name / Call Sign
     *     - Unique Id
     *     - Flight System
     *     - Cargo System
     *     - Navigation System
     *     - Communication System
     *     - Sensor System
     *     - Damage System
     *     - Turret System
     */
    [SelectionBase]
    [DisallowMultipleComponent]
    [DebuggerDisplay("Id = {" + nameof(index) + "}")]
    public class Entity : MonoBehaviour {

        public int index = 0;
        public string guid = "--default--"; // todo remove in favor of id
        
        [NonSerialized] public Faction faction;
        [UsePropertyDrawer(typeof(Faction))] [SerializeField] 
        public int factionId;
        
        public HealthComponent health;
        private FlightController flightSystem;
        private WeaponSystemComponent weaponSystem;
        
        private void Awake() {
//            flightSystem = GetComponent<FlightController>();
//            weaponSystem = GetComponent<WeaponSystemComponent>();
//            health = this.GetOrCreateComponent<HealthComponent>();
//            faction = Faction.GetFaction(factionId);
//            gameObject.layer = LayerMask.NameToLayer("Entity");
//            GameData.Instance.RegisterEntity(this);
        }

        public FlightController FlightSystem => flightSystem;
        public AIInfo aiInfo => GameData.Instance.aiInfoMap[index];
        public WeaponSystemComponent WeaponSystem => weaponSystem;
        public TransformInfo transformInfo => GameData.Instance.transformInfoMap[index];

    }

}