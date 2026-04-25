using UnityEngine;

public class SearchState : EnvironmentInteractionState
{

    float _approachDistanceThreshold = 8f;
    public SearchState(
    EnvironmentInteractionContext context,
    EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate
    ) : base(context, estate)

    {

    }

    public override void EnterState()
    {

        Debug.Log("SEARCH STATE");
    }
    public override void UpdateState() { }
    public override void FixedUpdateState() { }
    public override void LateUpdateState() { }
    public override void ExitState() { }
    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {

        if (Context.CurrentLeftShoulderTransform == null ||
        Context.CurrentIntersectingCollider == null)
        {
            return StateKey;
        }
        if (CheckShouldReset())
        {

            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;

        }
        bool isClosestPointOnColliderValid = Context.ClosestPointOnColliderFromLeftShoulder != Vector3.positiveInfinity;

        bool isCloseToTarget = isClosestPointOnColliderValid && Vector3.Distance(
            Context.ClosestPointOnColliderFromLeftShoulder,
            Context.RootTransform.position) < _approachDistanceThreshold;

        if (isCloseToTarget)
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Approach;
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