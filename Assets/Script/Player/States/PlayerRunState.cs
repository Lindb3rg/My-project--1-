using UnityEngine;

public class PlayerRunState : PlayerGroundedState
{
    public PlayerRunState(PlayerStateMachine.EPlayerState key, PlayerStateMachine ctx) : base(key, ctx) { }

    public override void EnterState()
    {
        _ctx.Anim.SetBool("inAir", false);
        _ctx.Anim.SetBool("isRunning", true);
        _ctx.Anim.SetBool("isWalking", false);
        _ctx.Anim.SetBool("isSprinting", false);
    }

    public override void ExitState()
    {
        _ctx.Anim.SetBool("isRunning", false);
    }

    public override void UpdateState()
    {
        if (CheckSharedTransitions()) return;

        float magnitude = Mathf.Abs(_ctx.MoveInput.x);

        if (magnitude <= 0.8f)
        {
            _ctx.TransitionToState(PlayerStateMachine.EPlayerState.Walk);
            return;
        }

        if (_ctx.SprintHeld)
        {
            _ctx.TransitionToState(PlayerStateMachine.EPlayerState.Sprint);
            return;
        }

        _ctx.HandleTurning();
    }

    public override void FixedUpdateState()
    {
        if (IsPushingIntoWall)
        {
            _ctx.Rb.linearVelocity = new Vector3(0f, _ctx.Rb.linearVelocity.y, 0f);
            return;
        }

        _ctx.Rb.linearVelocity = new Vector3(
            _ctx.FacingDirection * _ctx.RunSpeed,
            _ctx.Rb.linearVelocity.y,
            0f
        );
    }

    public override PlayerStateMachine.EPlayerState GetNextState() => StateKey;
}