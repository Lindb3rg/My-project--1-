
using UnityEngine;

public abstract class EnvironmentInteractionState : BaseState<EnvironmentInteractionStateMachine.EEnvironmentInteractionState>
{
    float _movingAwayOffset = .05f;
    bool _shouldReset;
    protected EnvironmentInteractionContext Context;
    public EnvironmentInteractionState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EEnvironmentInteractionState stateKey) : base(stateKey)
    {

        Context = context;

    }

    protected bool CheckShouldReset()
    {

        if (_shouldReset)
        {
            Context.LowestDistance = Mathf.Infinity;
            _shouldReset = false;
            return true;
        }
        bool isMovingAway = CheckIsMovingAway();
        bool isPlayerJumping = Mathf.Round(Context.Rb.linearVelocity.y) >= 1;
        if (isMovingAway || isPlayerJumping)
        {
            Context.LowestDistance = Mathf.Infinity;
            return true;
        }
        return false;

    }

    protected bool CheckIsMovingAway()
    {
        float currentDistanceToTarget = Vector3.Distance(Context.RootTransform.position, Context.ClosestPointOnColliderFromLeftShoulder);

        bool isSearchingForNewInteraction = Context.CurrentIntersectingCollider == null;
        if (isSearchingForNewInteraction)
        {
            return false;
        }

        bool isGettingCloserToTarget = currentDistanceToTarget <= Context.LowestDistance;
        if (isGettingCloserToTarget)
        {
            Context.LowestDistance = currentDistanceToTarget;
            return false;
        }
        bool isMovingAwayFromTarget = currentDistanceToTarget > Context.LowestDistance + _movingAwayOffset;
        if (isMovingAwayFromTarget)
        {
            Context.LowestDistance = Mathf.Infinity;
            return true;
        }
        return false;
    }

    private Vector3 GetClosestPointOnCollider(Collider intersectingCollider, Vector3 positionToCheck)
    {
        return intersectingCollider.ClosestPoint(positionToCheck);
    }

    protected void StartIkTargetPositionTracking(Collider intersectingCollider)
    {
        if (intersectingCollider.gameObject.layer == LayerMask.NameToLayer("Wall") && Context.CurrentIntersectingCollider == null)
        {
            Context.CurrentIntersectingCollider = intersectingCollider;
            Context.SetCurrentIkTransforms();
            SetIkTargetPosition();
        }

    }

    protected void UpdateIkTargetPositionTracking(Collider intersectingCollider)
    {
        if (intersectingCollider == Context.CurrentIntersectingCollider)
        {
            SetIkTargetPosition();
        }
    }

    protected void ResetIkTargetPositionTracking(Collider intersectingCollider)
    {
        if (intersectingCollider == Context.CurrentIntersectingCollider)
        {
            Context.CurrentIntersectingCollider = null;
            Context.ClosestPointOnColliderFromLeftShoulder = Vector3.positiveInfinity;
            Context.ClosestPointOnColliderFromRightShoulder = Vector3.positiveInfinity;
            _shouldReset = true;


        }
    }
    private void SetIkTargetPosition()
    {
        // Left hand
        Context.ClosestPointOnColliderFromLeftShoulder = GetClosestPointOnCollider(
            Context.CurrentIntersectingCollider,
            new Vector3(Context.CurrentLeftShoulderTransform.position.x, Context.CharacterLeftShoulderHeight, Context.CurrentLeftShoulderTransform.position.z)); // live position

        Vector3 leftRayDirection = (Context.CurrentLeftShoulderTransform.position - Context.ClosestPointOnColliderFromLeftShoulder).normalized;
        float leftOffSetDistance = .05f;
        Vector3 leftOffset = leftRayDirection * leftOffSetDistance;
        Vector3 leftOffsetPosition = Context.ClosestPointOnColliderFromLeftShoulder + leftOffset;
        Context.CurrentLeftIkTargetTransform.position = new Vector3(leftOffsetPosition.x, Context.CharacterLeftShoulderHeight, leftOffsetPosition.z);

        // Right hand
        Context.ClosestPointOnColliderFromRightShoulder = GetClosestPointOnCollider(
            Context.CurrentIntersectingCollider,
            new Vector3(Context.CurrentRightShoulderTransform.position.x, Context.CharacterRightShoulderHeight, Context.CurrentRightShoulderTransform.position.z)); // live position

        Vector3 rightRayDirection = (Context.CurrentRightShoulderTransform.position - Context.ClosestPointOnColliderFromRightShoulder).normalized;
        float rightOffSetDistance = .05f;
        Vector3 rightOffset = rightRayDirection * rightOffSetDistance;
        Vector3 rightOffsetPosition = Context.ClosestPointOnColliderFromRightShoulder + rightOffset;
        Context.CurrentRightIkTargetTransform.position = new Vector3(rightOffsetPosition.x, Context.CharacterLeftShoulderHeight, rightOffsetPosition.z);

    }
}