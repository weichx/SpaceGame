using System.Diagnostics;
using SpaceGame;
using UnityEngine;
using Weichx.Util;
using Debug = UnityEngine.Debug;

public class FlightController : MonoBehaviour {
    
    public float currentSpeed;
    public float currentThrottle;

    public float maxSpeed = 50;
    
    public Vector3 targetUp;
    public Vector3 velocity;
    public Vector3 velocityTrend;
    public Vector3 targetPosition;
    public Vector3 angularVelocity;
    public ApproachType arrivalType;

    public float turnRate = 90f;
    
    public void SetTargetDirection(Vector3 direction, ApproachType arrivalType = ApproachType.Normal) {
     //   AssertOnlyOneCallPerFrame();
        targetPosition = direction * 10000f;
        this.arrivalType = arrivalType;
    }

    public void SetTargetPosition(Vector3 position, ApproachType arrivalType) {
       // AssertOnlyOneCallPerFrame();
        targetPosition = position;
        this.arrivalType = arrivalType;
    }
    
    // editor only
//    private uint lastFrameSet;
//
//    [Conditional("UNITY_ASSERTIONS")]
//    private void AssertOnlyOneCallPerFrame() {
//        Debug.Assert(lastFrameSet != GameTimer.Instance.frameId,
//            $"Agent {name} had multiple calls to {nameof(SetTargetDirection)} or {nameof(SetTargetPosition)} in a single frame"
//        );
//        lastFrameSet = GameTimer.Instance.frameId;
//    }
}
