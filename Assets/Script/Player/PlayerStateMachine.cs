using UnityEngine;
using UnityEngine.InputSystem;
using Divers;

public class PlayerStateMachine : CharacterStateMachine
{

    // Speed & physics settings (editable in Inspector)
    public float RunSpeed = 10f;
    public float SprintSpeed = 15f;
    public float WalkSpeed = 3f;
    public float CrouchSpeed = 3f;
    public float RollSpeed = 5f;
    public float TurnDelay = 0.15f;
    public float TurnTimer { get; set; } = 0f;

    // Audio and Footsteps
    AudioSource audioSource;
    public AudioClip audioClip;


    // IK animations
    public Transform LeftFootTarget;
    public Transform RightFootTarget;
    public Transform LeftHandTarget;
    public Transform RightHandTarget;
    // In PlayerStateMachine
    public UnityEngine.Animations.Rigging.TwoBoneIKConstraint LeftLegIK;
    public UnityEngine.Animations.Rigging.TwoBoneIKConstraint RightLegIK;
    public UnityEngine.Animations.Rigging.TwoBoneIKConstraint LeftHandIK;
    public UnityEngine.Animations.Rigging.TwoBoneIKConstraint RightHandIK;
    private Collider _nearbyWallCollider;
    public float maxReachDistance = 1.2f;





    // Jumping
    public float JumpForce = 5f;
    public float DoubleJumpForce = 6f;
    public float FallMultiplier = 2.5f;      // faster falling
    public float LowJumpMultiplier = 2f;     // short hop when tap
    public float AirAcceleration = 5f;
    public float AirAccelerationMultiplier = 1f;  // fine tune without changing base
    public float CoyoteTime = 0.1f;          // grace period after walking off edge
    public float CoyoteTimeCounter { get; set; }
    public bool JumpHeld { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool ArmsToggled { get; private set; }
    public bool IsArmed { get; private set; }

    // Edge Control
    public float ClimbRepositionThreshold = 0.05f;  // how close before animation triggers
    public float ClimbLedgeOffset = 0.1f;           // how high above ledge to place player
    public float ClimbRepositionSpeed = 10f;





    // For when Apply Root Motion is needed
    public Transform MeshChild { get; private set; }
    public Vector3 GroundCheckOriginalPos { get; private set; }
    public Vector3 FrontCheckOriginalPos { get; private set; }

    // Input
    private InputSystem_Actions _input;

    // Aiming
    public Vector2 AimInput { get; private set; }
    public float AimSpeed = 30f;
    private float _currentAimAngle = 0f;
    private float _aimResetTimer = 0f;
    public float AimResetDelay = 1f;   // time before it starts resetting, editable in Inspector
    public float AimResetSpeed = 50f;

    // States
    public PlayerIdleState Idle { get; private set; }
    public PlayerMoveState Move { get; private set; }
    public PlayerJumpState Jump { get; private set; }
    public PlayerAirState Air { get; private set; }
    public PlayerLandState Land { get; private set; }
    public PlayerEdgeClimbState EdgeClimb { get; private set; }




    protected override void Awake()
    {
        base.Awake();

        _sm = new StateMachine();
        _input = new InputSystem_Actions();

        Idle = new PlayerIdleState(this, _sm);
        Move = new PlayerMoveState(this, _sm);
        Jump = new PlayerJumpState(this, _sm);
        Air = new PlayerAirState(this, _sm);
        Land = new PlayerLandState(this, _sm);
        EdgeClimb = new PlayerEdgeClimbState(this, _sm);

        MeshChild = transform.Find("Ch44");
        GroundCheckOriginalPos = GroundCheck.localPosition;
        FrontCheckOriginalPos = FrontCheck.localPosition;
        audioSource = GetComponent<AudioSource>();

    }

    void Start() => _sm.ChangeState(Idle);

    void OnEnable() => _input.Player.Enable();
    void OnDisable() => _input.Player.Disable();

    protected override void Update()
    {
        // Ground check
        base.Update();

        if (IsGrounded)
        {
            CoyoteTimeCounter = CoyoteTime;
        }
        else
        {
            CoyoteTimeCounter -= Time.deltaTime;
        }



        JumpHeld = _input.Player.Jump.IsPressed();
        JumpPressed = _input.Player.Jump.WasPressedThisFrame();
        MoveInput = _input.Player.Move.ReadValue<Vector2>();
        SprintHeld = _input.Player.Sprint.IsPressed();
        // ArmsToggled = _input.Player.ToggleArm.WasPressedThisFrame();
        AimInput = _input.Player.Look.ReadValue<Vector2>();
        _sm.Tick();
        if (ArmsToggled)
        {
            IsArmed = !IsArmed;
            _sm.ChangeState(_sm.CurrentState);
        }





    }

    void LateUpdate()
    {
        _sm.LateTick(); // ← dispatch to current state first

        Transform spineBone = Anim.GetBoneTransform(HumanBodyBones.UpperChest);
        if (spineBone == null) return;

        if (!IsArmed)
        {
            ResetAim();
        }
        else if (Mathf.Abs(AimInput.y) > 0.1f)
        {
            _currentAimAngle += -AimInput.y * AimSpeed * Time.deltaTime;
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
        if (Mathf.Abs(_currentAimAngle) > 0.1f)
        {
            _currentAimAngle = Mathf.MoveTowards(
                _currentAimAngle,
                0f,
                AimResetSpeed * Time.deltaTime
            );
        }
        else
        {
            _currentAimAngle = 0f;
        }
    }

    public void FootstepSound()
    {
        audioSource.PlayOneShot(audioClip);

    }

    public void UpdateWallHandIK()
{
    if (_nearbyWallCollider != null)
    {
        Vector3 leftHit  = GetWallSurfacePoint(LeftHandTarget.position);
        Vector3 rightHit = GetWallSurfacePoint(RightHandTarget.position);

        float leftDist  = Vector3.Distance(LeftHandTarget.position,  leftHit);
        float rightDist = Vector3.Distance(RightHandTarget.position, rightHit);
        float avgDist   = (leftDist + rightDist) * 0.5f;


        float weight = 1f - Mathf.Clamp01(avgDist / maxReachDistance);

        LeftHandIK.weight  = Mathf.Lerp(LeftHandIK.weight,  weight, Time.deltaTime * 8f);
        RightHandIK.weight = Mathf.Lerp(RightHandIK.weight, weight, Time.deltaTime * 8f);

        LeftHandTarget.position  = Vector3.Lerp(LeftHandTarget.position,  leftHit,  Time.deltaTime * 10f);
        RightHandTarget.position = Vector3.Lerp(RightHandTarget.position, rightHit, Time.deltaTime * 10f);
    }
    else
    {
        LeftHandIK.weight  = Mathf.Lerp(LeftHandIK.weight,  0f, Time.deltaTime * 8f);
        RightHandIK.weight = Mathf.Lerp(RightHandIK.weight, 0f, Time.deltaTime * 8f);
    }
}

private Vector3 GetWallSurfacePoint(Vector3 fromPosition)
{
    RaycastHit hit;
    if (Physics.Raycast(fromPosition, transform.forward, out hit, 2f, WallLayer))
        return hit.point;

    // Fallback to closest point if raycast misses
    return _nearbyWallCollider.ClosestPoint(fromPosition);
}


    public void HandleTurning()
    {
        bool wantsToTurn = (MoveInput.x > 0f && FacingDirection == -1)
                        || (MoveInput.x < 0f && FacingDirection == 1);

        if (wantsToTurn)
        {
            TurnTimer += Time.deltaTime;

            if (TurnTimer >= TurnDelay)
            {
                if (MoveInput.x > 0f)
                {
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    FacingDirection = 1;
                }
                else if (MoveInput.x < 0f)
                {
                    transform.rotation = Quaternion.Euler(0, -90, 0);
                    FacingDirection = -1;
                }
                TurnTimer = 0f;
            }
        }
        else
        {
            TurnTimer = 0f;
        }
        if (FrontCheck != null)
        {
            Vector3 pos = FrontCheck.localPosition;
            pos.x = Mathf.Abs(pos.x) * FacingDirection;
            FrontCheck.localPosition = pos;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Inside Trigger");
        if (((1 << other.gameObject.layer) & WallLayer) != 0)
            _nearbyWallCollider = other;
            Debug.Log("Inside Condition");
    }

    void OnTriggerExit(Collider other)
    {
        if (other == _nearbyWallCollider)
            _nearbyWallCollider = null;
    }
}