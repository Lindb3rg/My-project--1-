using UnityEngine;
public class PlayerJumpState : IState
{
    private readonly PlayerStateMachine _ctx;
    private readonly StateMachine _sm;


    public PlayerJumpState(PlayerStateMachine ctx, StateMachine sm)
    {
        _ctx = ctx;
        _sm = sm;
    }

    public void Enter()
    {
        Debug.Log("Entered Jumpstate");
        _ctx.CoyoteTimeCounter = 0f; // consume coyote time
        _ctx.Anim.SetTrigger("Jump");
        _ctx.Anim.SetBool("inAir", true);
        _ctx.Anim.SetBool("isGrounded", false);
        _ctx.Rb.linearVelocity = new Vector3(
            _ctx.Rb.linearVelocity.x,
            _ctx.JumpForce,
            0f
        );

    }

    public void Exit()
    {

    }

    public void Tick()
    {
        // Once we start falling, hand off to AirState
        if (_ctx.Rb.linearVelocity.y < 0)
            _sm.ChangeState(_ctx.Air);
    }

    public void FixedTick()
    {
        if (!_ctx.JumpHeld && _ctx.Rb.linearVelocity.y > 0)
        {
            _ctx.Rb.linearVelocity = new Vector3(
                _ctx.Rb.linearVelocity.x,
                _ctx.Rb.linearVelocity.y * (1f - _ctx.LowJumpMultiplier * Time.fixedDeltaTime),
                0f
            );
        }

        // Momentum based horizontal movement
        ApplyHorizontalMomentum();


    }
    public void LateTick() { }

    private void ApplyHorizontalMomentum()
    {
        if (_ctx.MoveInput.x != 0)
        {
            float targetSpeed = _ctx.FacingDirection * _ctx.RunSpeed * 0.8f;
            float currentX = _ctx.Rb.linearVelocity.x;

            _ctx.Rb.linearVelocity = new Vector3(
                Mathf.Lerp(currentX, targetSpeed, _ctx.AirAcceleration * _ctx.AirAccelerationMultiplier * Time.fixedDeltaTime),
                _ctx.Rb.linearVelocity.y,
                0f
            );
        }
    }
     
}