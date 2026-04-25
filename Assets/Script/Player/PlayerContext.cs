using UnityEngine;

public class PlayerContext
{
    // Components
    public Rigidbody Rb { get; }
    public Animator Anim { get; }
    public Transform Transform { get; }
    public Transform MeshChild { get; }
    public Transform GroundCheck { get; }
    public Transform PivotCheck { get; }
    public Transform FrontCheck { get; }

    // Movement settings
    public float WalkSpeed { get; }
    public float RunSpeed { get; }
    public float SprintSpeed { get; }
    public float TurnDelay { get; }

    // Jump settings
    public float JumpForce { get; set; }
    public float JumpHorizontalSpeed { get; set; }
    public float DoubleJumpForce { get; set; }
    public float FallMultiplier { get; set; }
    public float LowJumpMultiplier { get; set; }
    public float AirAcceleration { get; set; }
    public float AirAccelerationMultiplier { get; set; }
    public float CoyoteTime { get; set; }
    public bool DidDoubleJump { get; set; }
    public float JumpLockTimer { get; set; }
    public float JumpLockDuration { get; set; }

    // Edge climb settings
    public float ClimbRepositionThreshold { get; }
    public float ClimbLedgeOffset { get; }
    public float ClimbRepositionSpeed { get; }

    // Mutable — updated every frame
    public Vector2 MoveInput { get; set; }
    public Vector2 AimInput { get; set; }
    public bool JumpPressed { get; set; }
    public bool JumpHeld { get; set; }
    public bool SprintHeld { get; set; }
    public bool IsArmed { get; set; }
    public bool IsGrounded { get; set; }
    public bool TouchesWall { get; set; }
    public bool EdgeDetected { get; set; }
    public Vector3 EdgePosition { get; set; }
    public int FacingDirection { get; set; } = 1;
    public float CoyoteTimeCounter { get; set; }
    public float TurnTimer { get; set; }

    // NEW — gravity
    public bool GravityFlipPressed { get; set; }
    public Vector2 GravityFlipDirectionInput { get; set; }
    public GravityDirection CurrentGravityDirection { get; set; } = GravityDirection.Down;

    // Gravity axes — synced every frame from PlayerStateMachine
    public Vector3 GravityDown { get; set; } = Vector3.down;   // direction of gravity pull
    public Vector3 GravityUp { get; set; }   = Vector3.up;     // direction of jump / anti-gravity
    public Vector3 MoveAxis { get; set; }    = Vector3.right;  // horizontal movement axis

    // Velocity helpers
    public float GetGravityVelocity()     => Vector3.Dot(Rb.linearVelocity, GravityDown);  // positive = falling
    public float GetAntiGravVelocity()    => Vector3.Dot(Rb.linearVelocity, GravityUp);    // positive = rising
    public float GetMoveAxisVelocity()    => Vector3.Dot(Rb.linearVelocity, MoveAxis);

    public Vector3 BuildVelocity(float moveVelocity, float antiGravVelocity)
        => MoveAxis * moveVelocity + GravityUp * antiGravVelocity;

    public PlayerContext(
        Rigidbody rb,
        Animator anim,
        Transform transform,
        Transform meshChild,
        Transform groundCheck,
        Transform pivotCheck,
        Transform frontCheck,
        float walkSpeed,
        float runSpeed,
        float sprintSpeed,
        float turnDelay,
        float jumpForce,
        float doubleJumpForce,
        float fallMultiplier,
        float lowJumpMultiplier,
        float airAcceleration,
        float airAccelerationMultiplier,
        float coyoteTime,
        float climbRepositionThreshold,
        float climbLedgeOffset,
        float climbRepositionSpeed)
    {
        Rb = rb;
        Anim = anim;
        Transform = transform;
        MeshChild = meshChild;
        GroundCheck = groundCheck;
        PivotCheck = pivotCheck;
        FrontCheck = frontCheck;

        WalkSpeed = walkSpeed;
        RunSpeed = runSpeed;
        SprintSpeed = sprintSpeed;
        TurnDelay = turnDelay;

        JumpForce = jumpForce;
        DoubleJumpForce = doubleJumpForce;
        FallMultiplier = fallMultiplier;
        LowJumpMultiplier = lowJumpMultiplier;
        AirAcceleration = airAcceleration;
        AirAccelerationMultiplier = airAccelerationMultiplier;
        CoyoteTime = coyoteTime;

        ClimbRepositionThreshold = climbRepositionThreshold;
        ClimbLedgeOffset = climbLedgeOffset;
        ClimbRepositionSpeed = climbRepositionSpeed;
    }

    public bool IsMoving() => Mathf.Abs(MoveInput.x) > 0.1f;
}