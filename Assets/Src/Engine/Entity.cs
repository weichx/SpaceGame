using System;
using System.Diagnostics;
using SpaceGame.AI;
using UnityEngine;
using Weichx.ReflectionAttributes;
using Weichx.Util;
using SpaceGame.Engine;
using Debug = UnityEngine.Debug;

namespace SpaceGame {

    public interface INameGenerator {

        string GenerateName(Entity entity);

    }
    

//    public class EntityTemplate {
//
//        public Faction faction;
//        public INameGenerator nameGenerator;
//        public AssetPointer<GameObject> chassis;
//        
//        //public AIGoalSet goals;
//        //public WeaponLoadout loadout;
//        //public AIPreferences preferences;
//        //public AIRestrictions restrictions;
//        
//        public int maxHitpoints;
//        public int startingHitpoints;
//
//        public float maxSpeed;
//        public float minSpeed;
//        public float breakingRate;
//        public float turnRateDegrees;
//        public float accelerationRate;
//        
//    }

    public class WeaponSystem {

        public float CurrentWeaponCooldownRemaining;

        public void Fire() { }

    }

    public class FlightSystem { }

    [SelectionBase]
    [DisallowMultipleComponent]
    [DebuggerDisplay("Id = {" + nameof(id) + "}")]
    [ExecuteInEditMode]
    public class Entity : MonoBehaviour {

        // todo maybe replace this with a single monobehavior that just has an id
        // todo make entity a runtime class not extending monobehavior
        [NonSerialized] public Faction faction;
        [NonSerialized] public int id;
        [UsePropertyDrawer(typeof(Faction))] [SerializeField] public int factionId;
        
        public float hitPoints = 100f;
        public float maxHitPoints = 120f;
        
        public TransformInfo transformInfo => GameData.Instance.transformInfoMap[id];
        public FlightInput flightInput => GameData.Instance.flightInputs[id];
        public AIInfo aiInfo => GameData.Instance.aiInfoMap[id];
        public int instanceID = 0; //this value is duplicated with the gameobject

        public Vector3 targetPosition;
        public ApproachType arrivalType;

//        public WeaponSystem WeaponSystem => WeaponSystem;
//        public FlightSystem FlightSystem => FlightSystem;
        
        private void Awake() {
            if (instanceID != 0) {
               // Debug.Log("Duplication Detected of " + gameObject.name + " oldID:" + instanceID + " newID: " + gameObject.GetInstanceID());
            }
            instanceID = gameObject.GetInstanceID();
            faction = Faction.GetFaction(factionId);
            gameObject.layer = LayerMask.NameToLayer("Entity");
            GameData.Instance.RegisterEntity(this);
        }

        public void SetTargetDirection(Vector3 direction, ApproachType arrivalType = ApproachType.Normal) {
            AssertOnlyOneCallPerFrame();
            targetPosition = direction * 10000f;
            this.arrivalType = arrivalType;
        }

        public void SetTargetPosition(Vector3 position, ApproachType arrivalType) {
            AssertOnlyOneCallPerFrame();
            targetPosition = position;
            this.arrivalType = arrivalType;
        }

        // editor only
        private uint lastFrameSet;

        [Conditional("UNITY_ASSERTIONS")]
        private void AssertOnlyOneCallPerFrame() {
            Debug.Assert(lastFrameSet != GameTimer.Instance.frameId,
                "Agent " +
                nameof(id) + " had multiple calls to " +
                nameof(SetTargetDirection) + " or " +
                nameof(SetTargetPosition) +
                " in a single frame"
            );
            lastFrameSet = GameTimer.Instance.frameId;
        }

    }

}