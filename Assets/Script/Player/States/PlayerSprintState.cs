using UnityEngine;
public class PlayerSprintState : PlayerGroundedState
{
    public PlayerSprintState(PlayerStateMachine.EPlayerState key, PlayerStateMachine ctx) : base(key, ctx) { }

    public override void EnterState()
    {
        _ctx.Anim.SetBool("inAir", false);
        _ctx.Anim.SetBool("isSprinting", true);
        _ctx.Anim.SetBool("isRunning", true);
        _ctx.Anim.SetBool("isWalking", false);
    }

    public override void ExitState()
    {
        _ctx.Anim.SetBool("isSprinting", false);
        _ctx.Anim.SetBool("isRunning", false);
    }

    public override void UpdateState()
    {
        if (CheckSharedTransitions()) return;

        float magnitude = Mathf.Abs(_ctx.MoveInput.x);

        if (!_ctx.SprintHeld)
        {
            _ctx.TransitionToState(magnitude > 0.8f
                ? PlayerStateMachine.EPlayerState.Run
                : PlayerStateMachine.EPlayerState.Walk);
            return;
        }

        if (magnitude <= 0.8f)
        {
            _ctx.TransitionToState(PlayerStateMachine.EPlayerState.Walk);
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
            _ctx.FacingDirection * _ctx.SprintSpeed,
            _ctx.Rb.linearVelocity.y,
            0f
        );
    }

    public override PlayerStateMachine.EPlayerState GetNextState() => StateKey;
}