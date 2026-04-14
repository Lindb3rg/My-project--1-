using UnityEngine;
public class PlayerAirState : IState
{
    private readonly PlayerStateMachine _ctx;
    private readonly StateMachine _sm;

    private bool _canDoubleJump;
    private bool _didDoubleJump;
    public bool DidDoubleJump => _didDoubleJump;
    private float _currentDroop;

    private Vector3 _leftFootDefault;
    private Vector3 _rightFootDefault;

    public PlayerAirState(PlayerStateMachine ctx, StateMachine sm)
    {
        _ctx = ctx;
        _sm = sm;
    }

    public void Enter()
    {
        if (!_ctx.IsGrounded)
        {
            Debug.Log("Entered Jump Loop state");
            _canDoubleJump = true;
            _didDoubleJump = false;
            _ctx.Anim.SetBool("inAir", true);
        }
        _ctx.LeftLegIK.weight = 0.5f;
        _ctx.RightLegIK.weight = 0.5f;
        _leftFootDefault = _ctx.LeftFootTarget.localPosition;
        _rightFootDefault = _ctx.RightFootTarget.localPosition;
        _currentDroop = 0f;
        Debug.Log($"Left default: {_leftFootDefault}, Right default: {_rightFootDefault}");


    }

    public void Exit()
    {
        _ctx.Anim.SetBool("inAir", false);
        _ctx.LeftFootTarget.localPosition = _leftFootDefault;
        _ctx.RightFootTarget.localPosition = _rightFootDefault;
        _ctx.LeftLegIK.weight = 0f;
        _ctx.RightLegIK.weight = 0f;

    }

    public void Tick()
    {
        if (_ctx.TouchesWall && _ctx.EdgeDetected)
        {

            _sm.ChangeState(_ctx.EdgeClimb);
            return;
        }

        if (_ctx.JumpPressed)
        {


            if (_canDoubleJump)
            {
                _ctx.Rb.linearVelocity = new Vector3(
                    _ctx.Rb.linearVelocity.x,
                    _ctx.DoubleJumpForce,
                    0f
                );
                _canDoubleJump = false;
                _didDoubleJump = true;
            }
        }


        _ctx.HandleTurning();

        if (_ctx.IsGrounded && _ctx.Rb.linearVelocity.y >= -0.1f)
        {
            if (_ctx.MoveInput.x != 0)
                _sm.ChangeState(_ctx.Move);
            else
                _sm.ChangeState(_ctx.Idle);
        }

    }

    public void FixedTick()
    {
        // Asymmetric gravity

        if (_ctx.Rb.linearVelocity.y < 0)
        {
            _ctx.Rb.linearVelocity += Vector3.up * Physics.gravity.y
                * (_ctx.FallMultiplier - 1f) * Time.fixedDeltaTime;
        }

        // Momentum based air control
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
    

    public void LateTick()
    {
        float vy = _ctx.Rb.linearVelocity.y;
        float targetDroop = Mathf.Lerp(0f, -0.8f, Mathf.InverseLerp(0f, -15f, vy));

        // Smoothly move toward the target droop
        _currentDroop = Mathf.Lerp(_currentDroop, targetDroop, Time.deltaTime * 3f);

        _ctx.LeftFootTarget.localPosition = _leftFootDefault + new Vector3(0f, _currentDroop, 0f);
        _ctx.RightFootTarget.localPosition = _rightFootDefault + new Vector3(0f, _currentDroop, 0f);
    }
}