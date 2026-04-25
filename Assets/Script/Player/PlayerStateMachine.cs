using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : CharacterStateMachine<PlayerStateMachine.EPlayerState>
{
    public enum EPlayerState
    {
        Idle,
        Walk,
        Run,
        Sprint,
        Jump,
        Fall,
        Land,
        EdgeClimb,
        GravityTransition  // NEW
    }

    [Header("Movement")]
    public float WalkSpeed = 3f;
    public float RunSpeed = 10f;
    public float SprintSpeed = 15f;
    public float TurnDelay = 0.15f;

    [Header("Jumping")]
    public float JumpForce = 5f;
    public float JumpHorizontalSpeed = 8f;
    public float DoubleJumpForce = 6f;
    public float FallMultiplier = 2.5f;
    public float LowJumpMultiplier = 2f;
    public float AirAcceleration = 5f;
    public float AirAccelerationMultiplier = 1f;
    public float CoyoteTime = 0.1f;
    public float JumpLockTimer;
    public float JumpLockDuration = 0.15f;

    [Header("Edge Climbing")]
    public float ClimbRepositionThreshold = 0.05f;
    public float ClimbLedgeOffset = 0.1f;
    public float ClimbRepositionSpeed = 10f;

    [Header("Aiming")]
    public float AimSpeed = 30f;
    public float AimResetDelay = 1f;
    public float AimResetSpeed = 50f;

    [Header("Audio")]
    public AudioClip FootstepClip;
    private AudioSource _audioSource;

    private InputSystem_Actions _input;
    private float _currentAimAngle = 0f;
    private float _aimResetTimer = 0f;

    private PlayerContext _context;
    public PlayerContext Context => _context;

    public PlayerFallState Fall => (PlayerFallState)States[EPlayerState.Fall];

    protected override void Awake()
    {
        base.Awake();

        _input = new InputSystem_Actions();
        _audioSource = GetComponent<AudioSource>();

        _context = new PlayerContext(
            Rb, Anim,
            transform,
            transform.Find("Ch44"),
            GroundCheck,
            PivotCheck,
            FrontCheck,
            WalkSpeed, RunSpeed, SprintSpeed, TurnDelay,
            JumpForce, DoubleJumpForce,
            FallMultiplier, LowJumpMultiplier,
            AirAcceleration, AirAccelerationMultiplier,
            CoyoteTime,
            ClimbRepositionThreshold,
            ClimbLedgeOffset,
            ClimbRepositionSpeed
        );

        States[EPlayerState.Idle]             = new PlayerIdleState(EPlayerState.Idle, _context);
        States[EPlayerState.Walk]             = new PlayerWalkState(EPlayerState.Walk, _context);
        States[EPlayerState.Run]              = new PlayerRunState(EPlayerState.Run, _context);
        States[EPlayerState.Sprint]           = new PlayerSprintState(EPlayerState.Sprint, _context);
        States[EPlayerState.Jump]             = new PlayerJumpState(EPlayerState.Jump, _context);
        States[EPlayerState.Fall]             = new PlayerFallState(EPlayerState.Fall, _context);
        States[EPlayerState.Land]             = new PlayerLandState(EPlayerState.Land, _context);
        States[EPlayerState.EdgeClimb]        = new PlayerEdgeClimbState(EPlayerState.EdgeClimb, _context);
        States[EPlayerState.GravityTransition] = new PlayerGravityTransitionState(EPlayerState.GravityTransition, _context); // NEW

        CurrentState = States[EPlayerState.Idle];
    }

    void OnEnable() => _input.Player.Enable();
    void OnDisable() => _input.Player.Disable();

    protected override void Update()
    {
        ReadInput();
        SyncContext();
        HandleTurning();
        HandleGravityFlipInput(); // NEW
        base.Update();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        UpdateAim();
    }

    private void ReadInput()
    {
        Vector2 moveInput = _input.Player.Move.ReadValue<Vector2>();
        _context.MoveInput = moveInput;
        _context.MoveInput = _input.Player.Move.ReadValue<Vector2>();
        _context.AimInput = _input.Player.Look.ReadValue<Vector2>();
        _context.JumpHeld = _input.Player.Jump.IsPressed();
        _context.JumpPressed = _input.Player.Jump.WasPressedThisFrame();
        _context.SprintHeld = _input.Player.Sprint.IsPressed();
        _context.GravityFlipPressed = _input.Player.GravityFlip.WasPressedThisFrame(); // NEW
        _context.GravityFlipDirectionInput = _input.Player.GravityFlipDirection.ReadValue<Vector2>(); // NEW
    }

    private void SyncContext()
    {
        _context.IsGrounded = IsGrounded;
        _context.TouchesWall = TouchesWall;
        _context.EdgeDetected = EdgeDetected;
        _context.EdgePosition = EdgePosition;
        _context.FacingDirection = FacingDirection;

        _context.JumpForce = JumpForce;
        _context.JumpHorizontalSpeed = JumpHorizontalSpeed;
        _context.DoubleJumpForce = DoubleJumpForce;
        _context.FallMultiplier = FallMultiplier;
        _context.LowJumpMultiplier = LowJumpMultiplier;
        _context.AirAcceleration = AirAcceleration;
        _context.AirAccelerationMultiplier = AirAccelerationMultiplier;
        _context.CoyoteTime = CoyoteTime;
        _context.JumpLockDuration = JumpLockDuration;
        _context.JumpLockTimer = JumpLockTimer;

        if (_context.IsGrounded)
            _context.CoyoteTimeCounter = CoyoteTime;
        else
            _context.CoyoteTimeCounter -= Time.deltaTime;

        SyncGravityAxes();
    }

    private void SyncGravityAxes()
    {
        _context.GravityDown = _context.CurrentGravityDirection switch
        {
            GravityDirection.Up    => Vector3.up,
            GravityDirection.Left  => Vector3.left,
            GravityDirection.Right => Vector3.right,
            _                      => Vector3.down,
        };
        _context.GravityUp  = -_context.GravityDown;
        _context.MoveAxis   = _context.CurrentGravityDirection switch
        {
            GravityDirection.Right => Vector3.up,
            GravityDirection.Left  => Vector3.down,
            _                      => Vector3.right,
        };
    }

    // NEW — called from Update, sends the flip request to the world gravity system
    private void HandleGravityFlipInput()
    {
        if (!_context.GravityFlipPressed) return;
        if (GravityStateMachine.Instance == null) return;

        GravityDirection targetDirection = GetGravityDirectionFromInput(_context.GravityFlipDirectionInput);
        if (targetDirection == GravityDirection.None) return;

        GravityStateMachine.Instance.RequestGravityFlip(targetDirection);
    }

    private GravityDirection GetGravityDirectionFromInput(Vector2 input)
    {
        if (Mathf.Abs(input.x) < 0.1f && Mathf.Abs(input.y) < 0.1f)
            return GravityDirection.None;

        // Use whichever axis has the stronger input
        if (Mathf.Abs(input.y) >= Mathf.Abs(input.x))
            return input.y > 0f ? GravityDirection.Up : GravityDirection.Down;
        else
            return input.x > 0f ? GravityDirection.Right : GravityDirection.Left;
    }

    // NEW — called by PlayerGravityListener when the world flip begins
    public void OnGravityFlipStarted(GravityDirection newDirection)
    {
        _context.CurrentGravityDirection = newDirection;
        TransitionToState(EPlayerState.GravityTransition);
    }

    // NEW — called by PlayerGravityListener when the world has settled
    public void OnGravityFlipCompleted(GravityDirection newDirection)
    {
        _context.CurrentGravityDirection = newDirection;
    }

    private void UpdateAim()
    {
        Transform spineBone = Anim.GetBoneTransform(HumanBodyBones.UpperChest);
        if (spineBone == null) return;

        if (!_context.IsArmed)
        {
            ResetAim();
        }
        else if (Mathf.Abs(_context.AimInput.y) > 0.1f)
        {
            _currentAimAngle += -_context.AimInput.y * AimSpeed * Time.deltaTime;
            _currentAimAngle = Mathf.Clamp(_currentAimAngle, -20f, 20f);
            _aimResetTimer = AimResetDelay;
        }
        else
        {
            _aimResetTimer -= Time.deltaTime;
            if (_aimResetTimer <= 0f)
                ResetAim();
        }

        if (Mathf.Abs(_currentAimAngle) > 0.01f)
            spineBone.localRotation *= Quaternion.Euler(_currentAimAngle, 0f, 0f);
    }

    private void ResetAim()
    {
        _currentAimAngle = Mathf.Abs(_currentAimAngle) > 0.1f
            ? Mathf.MoveTowards(_currentAimAngle, 0f, AimResetSpeed * Time.deltaTime)
            : 0f;
    }

    public void HandleTurning()
    {
        bool wantsToTurn = (_context.MoveInput.x > 0f && _context.FacingDirection == -1)
                        || (_context.MoveInput.x < 0f && _context.FacingDirection == 1);

        if (!wantsToTurn)
        {
            _context.TurnTimer = 0f;
            return;
        }

        _context.TurnTimer += Time.deltaTime;
        if (_context.TurnTimer < _context.TurnDelay) return;

        float facingY = _context.MoveInput.x > 0f ? 90f : -90f;
        Quaternion targetRotation = _context.CurrentGravityDirection switch
        {
            GravityDirection.Up    => Quaternion.Euler(0f,   facingY,  180f),
            GravityDirection.Left  => _context.MoveInput.x > 0f
                ? Quaternion.Euler( 90f, 0f, -90f)
                : Quaternion.Euler(-90f, 0f, -90f),
            GravityDirection.Right => _context.MoveInput.x > 0f
                ? Quaternion.Euler(-90f, 0f,  90f)
                : Quaternion.Euler( 90f, 0f,  90f),
            _                      => Quaternion.Euler(0f,   facingY,    0f),
        };

        transform.rotation = targetRotation;

        _context.FacingDirection = _context.MoveInput.x > 0f ? 1 : -1;
        FacingDirection = _context.FacingDirection;
        _context.TurnTimer = 0f;

        if (FrontCheck != null)
        {
            Vector3 pos = FrontCheck.localPosition;
            pos.x = Mathf.Abs(pos.x) * _context.FacingDirection;
            FrontCheck.localPosition = pos;
        }
    }

    public void FootstepSound()
    {
        _audioSource.PlayOneShot(FootstepClip);
    }
}