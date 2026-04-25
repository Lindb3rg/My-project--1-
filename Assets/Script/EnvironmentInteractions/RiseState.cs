using UnityEngine;

public class RiseState : EnvironmentInteractionState
{

    float _elapsedTime = 0.0f;
    float _lerpDuration = 2.0f;
    float _riseWeight = 1f;
    float _touchDistanceThreshold = .05f;
    float _touchTimeThreshold = 1f;
    

    public RiseState(
    EnvironmentInteractionContext context,
    EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate
    ) : base(context, estate)

    {

    }

    public override void EnterState()
    {

        _elapsedTime = 0.0f;
        Debug.Log("RISE STATE");


    }
    public override void UpdateState()
    {

        Context.CurrentLeftIkConstraint.weight = Mathf.Lerp(Context.CurrentLeftIkConstraint.weight, _riseWeight, _elapsedTime / _lerpDuration);
        Context.CurrentRightIkConstraint.weight = Mathf.Lerp(Context.CurrentRightIkConstraint.weight, _riseWeight, _elapsedTime / _lerpDuration);
        _elapsedTime += Time.deltaTime;

    }
    public override void FixedUpdateState() { }
    public override void LateUpdateState() { }
    public override void ExitState() { }
    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {

        if (CheckShouldReset())
        {

            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;

        }
        if (Vector3.Distance(Context.CurrentLeftIkTargetTransform.position, Context.ClosestPointOnColliderFromLeftShoulder) < _touchDistanceThreshold && _elapsedTime >= _touchTimeThreshold)
        {

            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Touch;

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