using UnityEngine;
public abstract class PlayerGroundedState : BaseState<PlayerStateMachine.EPlayerState>
{
    protected readonly PlayerStateMachine _ctx;

    protected PlayerGroundedState(PlayerStateMachine.EPlayerState key, PlayerStateMachine ctx) : base(key)
    {
        _ctx = ctx;
    }

    protected bool CheckSharedTransitions()
    {
        if (Mathf.Abs(_ctx.MoveInput.x) < 0.05f)
        {
            _ctx.TransitionToState(PlayerStateMachine.EPlayerState.Idle);
            return true;
        }

        if (!_ctx.IsGrounded)
        {
            _ctx.Anim.SetTrigger("fallTrigger");
            _ctx.TransitionToState(PlayerStateMachine.EPlayerState.Fall);
            return true;
        }

        if (_ctx.JumpPressed && _ctx.CoyoteTimeCounter > 0f)
        {
            _ctx.TransitionToState(PlayerStateMachine.EPlayerState.Jump);
            return true;
        }

        bool pushingIntoWall = (_ctx.TouchesWall && _ctx.FacingDirection == 1 && _ctx.MoveInput.x > 0)
                            || (_ctx.TouchesWall && _ctx.FacingDirection == -1 && _ctx.MoveInput.x < 0);

        if (pushingIntoWall)
        {
            _ctx.TransitionToState(PlayerStateMachine.EPlayerState.Idle);
            return true;
        }

        return false;
    }

    protected bool IsPushingIntoWall =>
        (_ctx.TouchesWall && _ctx.FacingDirection == 1 && _ctx.MoveInput.x > 0)
     || (_ctx.TouchesWall && _ctx.FacingDirection == -1 && _ctx.MoveInput.x < 0);

    public override void LateUpdateState() {}
    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
    public override void OnTriggerExit(Collider other) { }
}