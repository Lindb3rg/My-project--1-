using UnityEngine;

public class TouchState : EnvironmentInteractionState
{
    public float _elapsedTime = 0.0f;
    public float _resetThreshold = 0.5f;
    public TouchState(
    EnvironmentInteractionContext context,
    EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate
    ) : base(context, estate)

    {

    }

    public override void EnterState()
    {

        Debug.Log("TOUCH STATE");
        _elapsedTime = 0.0f;

    }
    public override void UpdateState(){}
    public override void FixedUpdateState(){}
    public override void LateUpdateState(){}
    public override void ExitState(){}
    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {

        if (_elapsedTime > _resetThreshold || CheckShouldReset())
        {

            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;

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