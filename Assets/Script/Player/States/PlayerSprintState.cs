using UnityEngine;

public class PlayerSprintState : PlayerGroundedState
{
    public PlayerSprintState(PlayerStateMachine.EPlayerState key, PlayerContext ctx) : base(key, ctx) { }

    public override void EnterState()
    {
        _ctx.Anim.SetBool("inAir",       false);
        _ctx.Anim.SetBool("isSprinting", true);
        _ctx.Anim.SetBool("isRunning",   true);
        _ctx.Anim.SetBool("isWalking",   false);
    }

    public override void ExitState()
    {
        _ctx.Anim.SetBool("isSprinting", false);
        _ctx.Anim.SetBool("isRunning",   false);
    }

    public override void UpdateState()
    {
        _ctx.Anim.SetBool("isSprinting", true);
        _ctx.Anim.SetBool("isRunning",   true);
    }

    public override void FixedUpdateState()
    {
        if (IsPushingIntoWall)
        {
            ZeroMoveVelocity();
            return;
        }

        _ctx.Rb.linearVelocity = _ctx.BuildVelocity(
            _ctx.FacingDirection * _ctx.SprintSpeed,
            _ctx.GetAntiGravVelocity()
        );
    }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        var shared = GetSharedNextState();
        if (!shared.Equals(StateKey)) return shared;

        float magnitude = Mathf.Abs(_ctx.MoveInput.x);

        if (!_ctx.SprintHeld)
            return magnitude > 0.8f
                ? PlayerStateMachine.EPlayerState.Run
                : PlayerStateMachine.EPlayerState.Walk;

        if (magnitude <= 0.8f)
            return PlayerStateMachine.EPlayerState.Walk;

        return StateKey;
    }
}