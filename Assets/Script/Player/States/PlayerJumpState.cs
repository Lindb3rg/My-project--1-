using UnityEngine;

public class PlayerJumpState : BaseState<PlayerStateMachine.EPlayerState>
{
    private readonly PlayerContext _ctx;
    private float _currentMoveVelocity;

    public PlayerJumpState(PlayerStateMachine.EPlayerState key, PlayerContext ctx) : base(key)
    {
        _ctx = ctx;
    }

    public override void EnterState()
    {
        _ctx.CoyoteTimeCounter = 0f;
        _ctx.Anim.SetBool("inAir",      true);
        _ctx.Anim.SetBool("isGrounded", false);
        _ctx.Anim.ResetTrigger("Jump");
        _ctx.Anim.SetTrigger("Jump");

        float jumpForce = _ctx.SprintHeld
            ? _ctx.JumpForce * 1.2f
            : _ctx.JumpForce;

        // Lock in movement axis velocity at jump time
        float moveVelocity = _ctx.MoveInput.x != 0
            ? Mathf.Sign(_ctx.MoveInput.x) * _ctx.JumpHorizontalSpeed
            : 0f;

        _currentMoveVelocity = moveVelocity;

        // Build velocity using gravity axes so jump always goes away from gravity
        _ctx.Rb.linearVelocity = _ctx.BuildVelocity(_currentMoveVelocity, jumpForce);
    }

    public override void ExitState() { }

    public override void UpdateState() { }

    public override void FixedUpdateState()
    {
        float antiGravVelocity = _ctx.GetAntiGravVelocity();

        // Short hop — damp anti-gravity velocity when jump is tapped
        if (!_ctx.JumpHeld && antiGravVelocity > 0)
        {
            Vector3 vel = _ctx.Rb.linearVelocity;
            Vector3 antiGravComponent = _ctx.GravityUp * antiGravVelocity;
            Vector3 damped = antiGravComponent * (1f - _ctx.LowJumpMultiplier * Time.fixedDeltaTime);
            _ctx.Rb.linearVelocity = vel - antiGravComponent + damped;
        }

        // Small nudge along movement axis
        if (_ctx.MoveInput.x != 0)
        {
            float nudge = Mathf.Sign(_ctx.MoveInput.x) * _ctx.JumpHorizontalSpeed * 0.5f;
            _currentMoveVelocity += nudge * Time.fixedDeltaTime;
            _currentMoveVelocity  = Mathf.Clamp(_currentMoveVelocity, -_ctx.JumpHorizontalSpeed, _ctx.JumpHorizontalSpeed);
        }

        // Preserve current anti-grav velocity, update move axis velocity
        _ctx.Rb.linearVelocity = _ctx.BuildVelocity(_currentMoveVelocity, _ctx.GetAntiGravVelocity());
    }

    public override void LateUpdateState() { }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        // Transition to fall when moving in gravity direction (peak of jump passed)
        if (_ctx.GetGravityVelocity() > 0)
            return PlayerStateMachine.EPlayerState.Fall;

        return StateKey;
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }
}