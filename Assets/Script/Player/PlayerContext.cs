using UnityEngine;

public class PlayerContext
{
    // Components
    public Rigidbody Rb { get; }
    public Animator Anim { get; }
    public Transform Transform { get;}
    public Transform MeshChild { get; }
    public Transform GroundCheck { get; }
    public Transform FrontCheck { get; }

    // Movement settings
    public float WalkSpeed { get; }
    public float RunSpeed { get; }
    public float SprintSpeed { get; }
    public float TurnDelay { get; }

    // Jump settings
    public float JumpForce { get; }
    public float DoubleJumpForce { get; }
    public float FallMultiplier { get; }
    public float LowJumpMultiplier { get; }
    public float AirAcceleration { get; }
    public float AirAccelerationMultiplier { get; }
    public float CoyoteTime { get; }
    public bool DidDoubleJump { get; set; }

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

    public PlayerContext(
        Rigidbody rb,
        Animator anim,
        Transform transform,
        Transform meshChild,
        Transform groundCheck,
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