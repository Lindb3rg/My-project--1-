using UnityEngine;

public class PlayerRunState : PlayerGroundedState
{
    public PlayerRunState(PlayerStateMachine.EPlayerState key, PlayerContext ctx) : base(key, ctx) { }

    public override void EnterState()
    {
        _ctx.Anim.SetBool("inAir",       false);
        _ctx.Anim.SetBool("isRunning",   true);
        _ctx.Anim.SetBool("isWalking",   false);
        _ctx.Anim.SetBool("isSprinting", false);
    }

    public override void ExitState()
    {
        _ctx.Anim.SetBool("isRunning", false);
    }

    public override void UpdateState()
    {
        _ctx.Anim.SetBool("isRunning", true);
    }

    public override void FixedUpdateState()
    {
        if (IsPushingIntoWall)
        {
            ZeroMoveVelocity();
            return;
        }

        _ctx.Rb.linearVelocity = _ctx.BuildVelocity(
            _ctx.FacingDirection * _ctx.RunSpeed,
            _ctx.GetAntiGravVelocity()
        );
    }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        var shared = GetSharedNextState();
        if (!shared.Equals(StateKey)) return shared;

        if (_ctx.SprintHeld && Mathf.Abs(_ctx.MoveInput.x) > 0.8f)
            return PlayerStateMachine.EPlayerState.Sprint;

        if (Mathf.Abs(_ctx.MoveInput.x) <= 0.8f)
            return PlayerStateMachine.EPlayerState.Walk;

        return StateKey;
    }
}