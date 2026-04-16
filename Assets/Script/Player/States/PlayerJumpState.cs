using UnityEngine;

public class PlayerJumpState : BaseState<PlayerStateMachine.EPlayerState>
{
    private readonly PlayerContext _ctx;

    public PlayerJumpState(PlayerStateMachine.EPlayerState key, PlayerContext ctx) : base(key)
    {
        _ctx = ctx;
    }

    public override void EnterState()
    {

        _ctx.CoyoteTimeCounter = 0f;
        _ctx.Anim.SetBool("inAir", true);
        _ctx.Anim.SetBool("isGrounded", false);
        _ctx.Anim.ResetTrigger("Jump");
        _ctx.Anim.SetTrigger("Jump");

        float jumpForce = _ctx.SprintHeld
            ? _ctx.JumpForce * 1.2f
            : _ctx.JumpForce;

        float horizontalVelocity = 0f;
        if (_ctx.MoveInput.x != 0)
        {
            horizontalVelocity = _ctx.FacingDirection * (_ctx.SprintHeld
                ? _ctx.SprintSpeed
                : _ctx.RunSpeed);
        }

        _ctx.Rb.linearVelocity = new Vector3(
            horizontalVelocity,
            jumpForce,
            0f
        );
    }


    public override void ExitState() { }
    public override void UpdateState() { }

    public override void FixedUpdateState()
    {
        
        Debug.Log($"Before air control - velocity: {_ctx.Rb.linearVelocity}");
        if (!_ctx.JumpHeld && _ctx.Rb.linearVelocity.y > 0)
        {
            _ctx.Rb.linearVelocity = new Vector3(
                _ctx.Rb.linearVelocity.x,
                _ctx.Rb.linearVelocity.y * (1f - _ctx.LowJumpMultiplier * Time.fixedDeltaTime),
                0f
            );
        }
        Debug.Log(_ctx.MoveInput.x);

        if (_ctx.MoveInput.x != 0)
        {
            float targetSpeed = _ctx.FacingDirection * (_ctx.SprintHeld ? _ctx.SprintSpeed : _ctx.RunSpeed) * 0.8f;
            float currentX = _ctx.Rb.linearVelocity.x;
            Debug.Log($"targetSpeed: {targetSpeed}, currentX: {currentX}, AirAcceleration: {_ctx.AirAcceleration}, Multiplier: {_ctx.AirAccelerationMultiplier}");
            _ctx.Rb.linearVelocity = new Vector3(
                Mathf.Lerp(currentX, targetSpeed, _ctx.AirAcceleration * _ctx.AirAccelerationMultiplier * Time.fixedDeltaTime),
                _ctx.Rb.linearVelocity.y,
                0f
            );
        }
    }

    public override void LateUpdateState() { }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        if (_ctx.Rb.linearVelocity.y < 0)
            return PlayerStateMachine.EPlayerState.Fall;

        return StateKey;
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
    public override void OnTriggerExit(Collider other) { }
}