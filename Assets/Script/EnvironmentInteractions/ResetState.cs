using UnityEngine;

public class ResetState : EnvironmentInteractionState
{

    float _elapsedTime = 0.0f;
    float _resetDuration = 0.5f;
    float _lerpDuration = 5.0f;
    float _startLeftWeight;
    float _startRightWeight;

    public ResetState(
    EnvironmentInteractionContext context,
    EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate
    ) : base(context, estate)

    {

    }

    public override void EnterState()
    {
        Debug.Log("RESET STATE");
        _elapsedTime = 0.0f;
        Context.ClosestPointOnColliderFromLeftShoulder = Vector3.positiveInfinity;
        Context.ClosestPointOnColliderFromRightShoulder = Vector3.positiveInfinity;
        Context.CurrentIntersectingCollider = null;
        _elapsedTime = 0.0f;
        _startLeftWeight = Context.CurrentLeftIkConstraint.weight;
        _startRightWeight = Context.CurrentRightIkConstraint.weight;
    }
    public override void UpdateState()
    {

        _elapsedTime += Time.deltaTime;

        float t = Mathf.Clamp01(_elapsedTime / _resetDuration);

        Context.CurrentLeftIkConstraint.weight = Mathf.Lerp(_startLeftWeight, 0f, t);
        Context.CurrentRightIkConstraint.weight = Mathf.Lerp(_startRightWeight, 0f, t);
        Context.CurrentLeftIkTargetTransform.localPosition = Vector3.Lerp(Context.CurrentLeftIkTargetTransform.localPosition, Context.CurrentLeftOriginalTargetPosition, _elapsedTime / _lerpDuration);
        Context.CurrentRightIkTargetTransform.localPosition = Vector3.Lerp(Context.CurrentRightIkTargetTransform.localPosition, Context.CurrentRightOriginalTargetPosition, _elapsedTime / _lerpDuration);
        if (t >= 1f)
        {
            Context.CurrentLeftIkConstraint.weight = 0f;
            Context.CurrentRightIkConstraint.weight = 0f;
        }

    }
    public override void FixedUpdateState() { }
    public override void LateUpdateState() { }
    public override void ExitState() { }
    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {

        bool isMoving = Context.Rb.linearVelocity != Vector3.zero;
        if (_elapsedTime >= _resetDuration && isMoving)
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Search;
        }

        return StateKey;

    }
    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
    public override void OnTriggerExit(Collider other) { }



}