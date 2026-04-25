using UnityEngine;

public class PlayerIdleState : BaseState<PlayerStateMachine.EPlayerState>
{
    private readonly PlayerContext _ctx;

    public PlayerIdleState(PlayerStateMachine.EPlayerState key, PlayerContext ctx) : base(key)
    {
        _ctx = ctx;
    }

    public override void EnterState()
    {
        _ctx.Anim.SetBool("inAir", false);
        _ctx.Anim.ResetTrigger("fallTrigger");
    }

    public override void ExitState() { }

    public override void UpdateState()
    {
        if (!_ctx.IsGrounded)
            _ctx.Anim.SetTrigger("fallTrigger");
    }

    public override void FixedUpdateState()
    {
        if (Mathf.Abs(_ctx.MoveInput.x) < 0.1f)
            _ctx.Rb.linearVelocity = _ctx.BuildVelocity(0f, _ctx.GetAntiGravVelocity());
    }

    public override void LateUpdateState() { }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        if (!_ctx.IsGrounded)
            return PlayerStateMachine.EPlayerState.Fall;

        if (_ctx.JumpPressed && _ctx.CoyoteTimeCounter > 0f)
            return PlayerStateMachine.EPlayerState.Jump;

        if (_ctx.IsMoving())
        {
            bool movingAwayFromWall = (_ctx.TouchesWall && _ctx.FacingDirection == 1  && _ctx.MoveInput.x < 0)
                                   || (_ctx.TouchesWall && _ctx.FacingDirection == -1 && _ctx.MoveInput.x > 0)
                                   || !_ctx.TouchesWall;

            if (movingAwayFromWall)
                return PlayerStateMachine.EPlayerState.Walk;
        }

        return StateKey;
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }
}