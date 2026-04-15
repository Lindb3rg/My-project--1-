using UnityEngine;

public class PlayerIdleState : BaseState<PlayerStateMachine.EPlayerState>
{
    private readonly PlayerStateMachine _ctx;

    public PlayerIdleState(PlayerStateMachine.EPlayerState key, PlayerStateMachine context) : base(key)
    {
        _ctx = context;
    }

    public override void EnterState()
    {
        _ctx.Anim.SetBool("inAir", false);
    }

    public override void ExitState() { }

    public override void UpdateState()
    {
        if (!_ctx.IsGrounded)
        {
            _ctx.Anim.SetTrigger("fallTrigger");
            _ctx.TransitionToState(PlayerStateMachine.EPlayerState.Fall);
            return;
        }

        if (_ctx.JumpPressed && _ctx.CoyoteTimeCounter > 0f)
        {
            _ctx.TransitionToState(PlayerStateMachine.EPlayerState.Jump);
            return;
        }

        if (_ctx.IsMoving)
        {
            bool movingAwayFromWall = (_ctx.TouchesWall && _ctx.FacingDirection == 1  && _ctx.MoveInput.x < 0)
                                   || (_ctx.TouchesWall && _ctx.FacingDirection == -1 && _ctx.MoveInput.x > 0)
                                   || !_ctx.TouchesWall;

            if (movingAwayFromWall)
                _ctx.TransitionToState(PlayerStateMachine.EPlayerState.Walk);
        }
    }

    public override void FixedUpdateState()
    {
        if (Mathf.Abs(_ctx.MoveInput.x) < 0.1f)
            _ctx.Rb.linearVelocity = new Vector3(0f, _ctx.Rb.linearVelocity.y, 0f);
    }

    public override void LateUpdateState()
    {
        
    }

    public override PlayerStateMachine.EPlayerState GetNextState() => StateKey;

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }
}