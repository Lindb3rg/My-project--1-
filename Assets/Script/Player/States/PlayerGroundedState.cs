using UnityEngine;

public abstract class PlayerGroundedState : BaseState<PlayerStateMachine.EPlayerState>
{
    protected readonly PlayerContext _ctx;

    protected PlayerGroundedState(PlayerStateMachine.EPlayerState key, PlayerContext ctx) : base(key)
    {
        _ctx = ctx;
    }

    protected bool IsPushingIntoWall =>
        (_ctx.TouchesWall && _ctx.FacingDirection == 1  && _ctx.MoveInput.x > 0)
     || (_ctx.TouchesWall && _ctx.FacingDirection == -1 && _ctx.MoveInput.x < 0);

    // Zero move axis velocity, preserve gravity axis velocity
    protected void ZeroMoveVelocity()
    {
        _ctx.Rb.linearVelocity = _ctx.BuildVelocity(0f, _ctx.GetAntiGravVelocity());
    }

    protected PlayerStateMachine.EPlayerState GetSharedNextState()
    {
        if (!_ctx.IsGrounded)
            return PlayerStateMachine.EPlayerState.Fall;

        if (_ctx.JumpPressed && _ctx.CoyoteTimeCounter > 0f)
            return PlayerStateMachine.EPlayerState.Jump;

        if (Mathf.Abs(_ctx.MoveInput.x) < 0.05f || IsPushingIntoWall)
            return PlayerStateMachine.EPlayerState.Idle;

        return StateKey;
    }

    public override void LateUpdateState() { }
    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }
}