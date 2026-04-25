using UnityEngine;

public class ApproachState : EnvironmentInteractionState
{
    float _elapsedTime = 0.0f;
    float _approachDuration = 2.0f;
    float _lerpDuration = 2.0f;
    float _approachWeight = .5f;
    float _riseDistanceThreshold = 1f;
    public ApproachState(
    EnvironmentInteractionContext context,
    EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate
    ) : base(context, estate)

    {

    }

    public override void EnterState()
    {
        Debug.Log("APPROACH STATE");
        _elapsedTime = 0.0f;
    }
    public override void UpdateState()
    {

        _elapsedTime += Time.deltaTime;
        Context.CurrentLeftIkConstraint.weight = Mathf.Lerp(Context.CurrentLeftIkConstraint.weight, _approachWeight, _elapsedTime / _lerpDuration);
        Context.CurrentRightIkConstraint.weight = Mathf.Lerp(Context.CurrentRightIkConstraint.weight, _approachWeight, _elapsedTime / _lerpDuration);

    }
    public override void FixedUpdateState(){}
    public override void LateUpdateState(){}
    public override void ExitState(){}
    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        bool isOverStateLifeDuration = _elapsedTime >= _approachDuration;

        if (CheckShouldReset())
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;
        }
        
        bool isWithinArmsReach = Vector3.Distance(Context.ClosestPointOnColliderFromLeftShoulder, Context.CurrentLeftShoulderTransform.position) < _riseDistanceThreshold;
        
    
        

        bool isClosestPointOnColliderReal = Context.ClosestPointOnColliderFromLeftShoulder != Vector3.positiveInfinity;
        if (isClosestPointOnColliderReal && isWithinArmsReach)
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Rise;
        }
        return StateKey;

    }
    public override void OnTriggerEnter(Collider other)
    {
        StartIkTargetPositionTracking(other);
    }
    public override void OnTriggerStay(Collider other)
    {
        UpdateIkTargetPositionTracking(other);
    }
    public override void OnTriggerExit(Collider other)
    {
        ResetIkTargetPositionTracking(other);
    }



}