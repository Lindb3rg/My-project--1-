using UnityEngine;

public class PlayerFallState : BaseState<PlayerStateMachine.EPlayerState>
{
    private readonly PlayerContext _ctx;
    private float _currentMoveVelocity;
    private bool _canDoubleJump;
    private bool _didDoubleJump;
    public bool DidDoubleJump => _didDoubleJump;

    public PlayerFallState(PlayerStateMachine.EPlayerState key, PlayerContext ctx) : base(key)
    {
        _ctx = ctx;
    }

    public override void EnterState()
    {
        _currentMoveVelocity = _ctx.GetMoveAxisVelocity();
        _canDoubleJump = true;
        _didDoubleJump = false;
        _ctx.Anim.SetBool("inAir", true);
        _ctx.Anim.SetTrigger("fallTrigger");
    }

    public override void ExitState()
    {
        _ctx.Anim.SetBool("inAir", false);
    }

    public override void UpdateState()
    {
        if (_ctx.JumpPressed && _canDoubleJump)
        {
            // Double jump always goes against gravity
            _ctx.Rb.linearVelocity = _ctx.BuildVelocity(
                _ctx.GetMoveAxisVelocity(),
                _ctx.DoubleJumpForce
            );
            _canDoubleJump = false;
            _didDoubleJump = true;
        }
    }

    public override void FixedUpdateState()
    {
        // Extra fall gravity — Physics.gravity is already in the correct direction
        if (_ctx.GetGravityVelocity() > 0)
        {
            _ctx.Rb.linearVelocity += Physics.gravity
                * (_ctx.FallMultiplier - 1f)
                * Time.fixedDeltaTime;
        }

        // Wall check
        bool pushingIntoWall = (_ctx.TouchesWall && _ctx.FacingDirection == 1  && _ctx.MoveInput.x > 0)
                            || (_ctx.TouchesWall && _ctx.FacingDirection == -1 && _ctx.MoveInput.x < 0);

        if (pushingIntoWall)
        {
            _currentMoveVelocity = 0f;
            _ctx.Rb.linearVelocity = _ctx.BuildVelocity(0f, _ctx.GetAntiGravVelocity());
            return;
        }

        // Nudge along movement axis
        if (_ctx.MoveInput.x != 0)
        {
            float nudge = Mathf.Sign(_ctx.MoveInput.x) * _ctx.JumpHorizontalSpeed * 0.5f;
            _currentMoveVelocity += nudge * Time.fixedDeltaTime;
            _currentMoveVelocity  = Mathf.Clamp(_currentMoveVelocity, -_ctx.JumpHorizontalSpeed, _ctx.JumpHorizontalSpeed);
        }

        _ctx.Rb.linearVelocity = _ctx.BuildVelocity(_currentMoveVelocity, _ctx.GetAntiGravVelocity());
    }

    public override void LateUpdateState() { }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        if (_ctx.TouchesWall && _ctx.EdgeDetected)
            return PlayerStateMachine.EPlayerState.EdgeClimb;

        // Grounded and not moving fast toward gravity (avoid false positives)
        if (_ctx.IsGrounded && _ctx.GetGravityVelocity() <= 0.1f)
            return _didDoubleJump
                ? PlayerStateMachine.EPlayerState.Land
                : (_ctx.MoveInput.x != 0
                    ? PlayerStateMachine.EPlayerState.Walk
                    : PlayerStateMachine.EPlayerState.Idle);

        return StateKey;
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }
}