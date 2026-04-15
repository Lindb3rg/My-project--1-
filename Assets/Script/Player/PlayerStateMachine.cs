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
        EdgeClimb
    }

    // Speed & physics
    [Header("Movement")]
    public float WalkSpeed = 3f;
    public float RunSpeed = 10f;
    public float SprintSpeed = 15f;
    public float TurnDelay = 0.15f;
    public float TurnTimer { get; set; } = 0f;

    // Jumping
    [Header("Jumping")]
    public float JumpForce = 5f;
    public float DoubleJumpForce = 6f;
    public float FallMultiplier = 2.5f;
    public float LowJumpMultiplier = 2f;
    public float AirAcceleration = 5f;
    public float AirAccelerationMultiplier = 1f;
    public float CoyoteTime = 0.1f;
    public float CoyoteTimeCounter { get; set; }

    // Edge climbing
    [Header("Edge Climbing")]
    public float ClimbRepositionThreshold = 0.05f;
    public float ClimbLedgeOffset = 0.1f;
    public float ClimbRepositionSpeed = 10f;

    // Aiming
    [Header("Aiming")]
    public float AimSpeed = 30f;
    public float AimResetDelay = 1f;
    public float AimResetSpeed = 50f;
    private float _currentAimAngle = 0f;
    private float _aimResetTimer = 0f;

    // Audio
    [Header("Audio")]
    public AudioClip FootstepClip;
    private AudioSource _audioSource;

    // Input
    private InputSystem_Actions _input;
    public bool JumpHeld { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool IsArmed { get; private set; }
    public Vector2 AimInput { get; private set; }

    // Root motion helpers
    public Transform MeshChild { get; private set; }
    public Vector3 GroundCheckOriginalPos { get; private set; }
    public Vector3 FrontCheckOriginalPos { get; private set; }
    public PlayerFallState Fall => (PlayerFallState)States[EPlayerState.Fall];

    protected override void Awake()
    {
        base.Awake();

        _input = new InputSystem_Actions();
        _audioSource = GetComponent<AudioSource>();
        MeshChild = transform.Find("Ch44");
        GroundCheckOriginalPos = GroundCheck.localPosition;
        FrontCheckOriginalPos = FrontCheck.localPosition;

        States[EPlayerState.Idle] = new PlayerIdleState(EPlayerState.Idle, this);
        States[EPlayerState.Walk] = new PlayerWalkState(EPlayerState.Walk, this);
        States[EPlayerState.Run] = new PlayerRunState(EPlayerState.Run, this);
        States[EPlayerState.Sprint] = new PlayerSprintState(EPlayerState.Sprint, this);
        States[EPlayerState.Jump] = new PlayerJumpState(EPlayerState.Jump, this);
        States[EPlayerState.Fall] = new PlayerFallState(EPlayerState.Fall, this);
        States[EPlayerState.Land] = new PlayerLandState(EPlayerState.Land, this);
        States[EPlayerState.EdgeClimb] = new PlayerEdgeClimbState(EPlayerState.EdgeClimb, this);

        CurrentState = States[EPlayerState.Idle];
    }

    protected override void Start()
    {
        base.Start();
    }

    void OnEnable()  => _input.Player.Enable();
    void OnDisable() => _input.Player.Disable();

    protected override void Update()
    {
        ReadInput();
        UpdateCoyoteTime();
        base.Update(); // runs CharacterStateMachine checks + StateManager tick
    }

    protected override void LateUpdate()
    {
        UpdateAim();
        base.LateUpdate();
    }

    private void ReadInput()
    {
        MoveInput   = _input.Player.Move.ReadValue<Vector2>();
        AimInput    = _input.Player.Look.ReadValue<Vector2>();
        JumpHeld    = _input.Player.Jump.IsPressed();
        JumpPressed = _input.Player.Jump.WasPressedThisFrame();
        SprintHeld  = _input.Player.Sprint.IsPressed();
    }

    private void UpdateCoyoteTime()
    {
        if (IsGrounded)
            CoyoteTimeCounter = CoyoteTime;
        else
            CoyoteTimeCounter -= Time.deltaTime;
    }

    private void UpdateAim()
    {
        Transform spineBone = Anim.GetBoneTransform(HumanBodyBones.UpperChest);
        if (spineBone == null) return;

        if (!IsArmed)
        {
            ResetAim();
        }
        else if (Mathf.Abs(AimInput.y) > 0.1f)
        {
            _currentAimAngle += -AimInput.y * AimSpeed * Time.deltaTime;
            _currentAimAngle  = Mathf.Clamp(_currentAimAngle, -20f, 20f);
            _aimResetTimer    = AimResetDelay;
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

    public override void HandleTurning()
    {
        bool wantsToTurn = (MoveInput.x > 0f && FacingDirection == -1)
                        || (MoveInput.x < 0f && FacingDirection == 1);

        if (!wantsToTurn)
        {
            TurnTimer = 0f;
            return;
        }

        TurnTimer += Time.deltaTime;
        if (TurnTimer < TurnDelay) return;

        transform.rotation = MoveInput.x > 0f
            ? Quaternion.Euler(0, 90, 0)
            : Quaternion.Euler(0, -90, 0);

        FacingDirection = MoveInput.x > 0f ? 1 : -1;
        TurnTimer = 0f;

        if (FrontCheck != null)
        {
            Vector3 pos = FrontCheck.localPosition;
            pos.x = Mathf.Abs(pos.x) * FacingDirection;
            FrontCheck.localPosition = pos;
        }
    }

    public void FootstepSound()
    {
        _audioSource.PlayOneShot(FootstepClip);
    }
}