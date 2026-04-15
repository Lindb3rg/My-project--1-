using UnityEngine;

public class PlayerFallState : BaseState<PlayerStateMachine.EPlayerState>
{
    private readonly PlayerStateMachine _ctx;

    private bool _canDoubleJump;
    private bool _didDoubleJump;
    public bool DidDoubleJump => _didDoubleJump;

    public PlayerFallState(PlayerStateMachine.EPlayerState key, PlayerStateMachine ctx) : base(key)
    {
        _ctx = ctx;
    }

    public override void EnterState()
    {
        _canDoubleJump = true;
        _didDoubleJump = false;
        _ctx.Anim.SetBool("inAir", true);
    }

    public override void ExitState()
    {
        _ctx.Anim.SetBool("inAir", false);
    }

    public override void UpdateState()
    {
        if (_ctx.TouchesWall && _ctx.EdgeDetected)
        {
            _ctx.TransitionToState(PlayerStateMachine.EPlayerState.EdgeClimb);
            return;
        }

        if (_ctx.JumpPressed && _canDoubleJump)
        {
            _ctx.Rb.linearVelocity = new Vector3(
                _ctx.Rb.linearVelocity.x,
                _ctx.DoubleJumpForce,
                0f
            );
            _canDoubleJump = false;
            _didDoubleJump = true;
        }

        _ctx.HandleTurning();

        if (_ctx.IsGrounded && _ctx.Rb.linearVelocity.y >= -0.1f)
        {
            _ctx.TransitionToState(_ctx.MoveInput.x != 0
                ? PlayerStateMachine.EPlayerState.Walk
                : PlayerStateMachine.EPlayerState.Idle);
        }
    }

    public override void FixedUpdateState()
    {
        if (_ctx.Rb.linearVelocity.y < 0)
        {
            _ctx.Rb.linearVelocity += Vector3.up * Physics.gravity.y
                * (_ctx.FallMultiplier - 1f) * Time.fixedDeltaTime;
        }

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

    public override void LateUpdateState() { }

    public override PlayerStateMachine.EPlayerState GetNextState() => StateKey;

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }
}