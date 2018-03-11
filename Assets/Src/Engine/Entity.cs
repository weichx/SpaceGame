using System;
using System.Diagnostics;
using SpaceGame.AI;
using SpaceGame.Engine;
using SpaceGame.Util;
using Src.Engine;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SpaceGame {

    public class Generic<T> {

        public T value;

    }

    [System.Serializable]
    public class NonGeneric : Generic<float> {

        

    }

    [SelectionBase]
    [DisallowMultipleComponent]
    [DebuggerDisplay("Id = {" + nameof(id) + "}")]
    public class Entity : MonoBehaviour, ISerializationCallbackReceiver {

        // todo maybe replace this with a single monobehavior that just has an id
        // todo make entity a runtime class not extending monobehavior
        
        [NonSerialized] public Faction faction;
        [NonSerialized] public int id;
        [FactionAttribute] [SerializeField] private int factionId;
        [HideInInspector] [SerializeField] private string guid;
        public float hitPoints = 100f;
        public NonGeneric nonGeneric;
        public byte[] bytes;
        public TransformInfo transformInfo => GameData.Instance.transformInfoMap[id];
        public FlightInput flightInput => GameData.Instance.flightInputs[id];
        public AIInfo aiInfo => GameData.Instance.aiInfoMap[id];
        
        public void Awake() {
            faction = Faction.GetFaction(factionId);
            gameObject.layer = LayerMask.NameToLayer("Entity");
            GameData.Instance.RegisterEntity(this);
        }

        public void OnBeforeSerialize() {
            if (guid == null) {
                guid = Guid.NewGuid().ToString();
            }
        }

        public void OnAfterDeserialize() {
            if (guid == null) {
                guid = Guid.NewGuid().ToString();
            }
        }

//        public int id;
        public Vector3 targetPosition;
        public ApproachType arrivalType;

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


        private uint lastFrameSet;

        [Conditional("DEBUG")]
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