using UnityEngine;

public class PlayerWalkState : PlayerGroundedState
{
    public PlayerWalkState(PlayerStateMachine.EPlayerState key, PlayerContext ctx) : base(key, ctx) { }

    public override void EnterState()
    {
        _ctx.Anim.SetBool("inAir", false);
        _ctx.Anim.SetBool("isWalking", true);
        _ctx.Anim.SetBool("isRunning", false);
        _ctx.Anim.SetBool("isSprinting", false);
        Debug.Log("We entered Walk state");
    }

    public override void ExitState()
    {
        _ctx.Anim.SetBool("isWalking", false);
    }

    public override void UpdateState()
    {
        _ctx.Anim.SetBool("isWalking", true);
    }

    public override void FixedUpdateState()
    {
        if (IsPushingIntoWall)
        {
            _ctx.Rb.linearVelocity = new Vector3(0f, _ctx.Rb.linearVelocity.y, 0f);
            return;
        }

        _ctx.Rb.linearVelocity = new Vector3(
            _ctx.FacingDirection * _ctx.WalkSpeed,
            _ctx.Rb.linearVelocity.y,
            0f
        );
        Debug.Log($"FacingDirection: {_ctx.FacingDirection}, WalkSpeed: {_ctx.WalkSpeed}, IsPushingIntoWall: {IsPushingIntoWall}");
    }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        var shared = GetSharedNextState();
        if (!shared.Equals(StateKey)) return shared;

        float magnitude = Mathf.Abs(_ctx.MoveInput.x);
        if (magnitude > 0.8f)
            return _ctx.SprintHeld
                ? PlayerStateMachine.EPlayerState.Sprint
                : PlayerStateMachine.EPlayerState.Run;

        return StateKey;
    }
}