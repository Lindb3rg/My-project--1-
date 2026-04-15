using UnityEngine;

public class PlayerJumpState : BaseState<PlayerStateMachine.EPlayerState>
{
    private readonly PlayerStateMachine _ctx;

    public PlayerJumpState(PlayerStateMachine.EPlayerState key, PlayerStateMachine ctx) : base(key)
    {
        _ctx = ctx;
    }

    public override void EnterState()
    {
        _ctx.CoyoteTimeCounter = 0f;
        _ctx.Anim.SetTrigger("Jump");
        _ctx.Anim.SetBool("inAir",      true);
        _ctx.Anim.SetBool("isGrounded", false);
        _ctx.Rb.linearVelocity = new Vector3(
            _ctx.Rb.linearVelocity.x,
            _ctx.JumpForce,
            0f
        );
    }

    public override void ExitState() { }

    public override void UpdateState()
    {
        if (_ctx.Rb.linearVelocity.y < 0)
        {
            _ctx.TransitionToState(PlayerStateMachine.EPlayerState.Fall);
            return;
        }
    }

    public override void FixedUpdateState()
    {
        if (!_ctx.JumpHeld && _ctx.Rb.linearVelocity.y > 0)
        {
            _ctx.Rb.linearVelocity = new Vector3(
                _ctx.Rb.linearVelocity.x,
                _ctx.Rb.linearVelocity.y * (1f - _ctx.LowJumpMultiplier * Time.fixedDeltaTime),
                0f
            );
        }

        ApplyHorizontalMomentum();
    }

    public override void LateUpdateState() { }

    public override PlayerStateMachine.EPlayerState GetNextState() => StateKey;

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }

    private void ApplyHorizontalMomentum()
    {
        if (_ctx.MoveInput.x != 0)
        {
            float targetSpeed = _ctx.FacingDirection * _ctx.RunSpeed * 0.8f;
            float currentX    = _ctx.Rb.linearVelocity.x;

            _ctx.Rb.linearVelocity = new Vector3(
                Mathf.Lerp(currentX, targetSpeed, _ctx.AirAcceleration * _ctx.AirAccelerationMultiplier * Time.fixedDeltaTime),
                _ctx.Rb.linearVelocity.y,
                0f
            );
        }
    }
}