using System;
using UnityEngine;

public abstract class CharacterStateMachine<EState> : StateManager<EState> where EState : Enum
{
    // Ground check
    [Header("Ground Check")]
    public float GroundCheckRadius = 0.2f;
    public float GroundCheckDistance = 0.5f;
    public LayerMask GroundLayer;
    public Transform GroundCheck;
    public bool IsGrounded { get; private set; }


    // Pivot check
    [Header("Pivot Check")]
    public Transform PivotCheck;

    // Front check
    [Header("Front Check")]
    public Transform FrontCheck;
    public Vector3 FrontCheckSize = new Vector3(0.1f, 1.5f, 0.1f);
    public LayerMask EdgeLayer;
    public LayerMask WallLayer;
    public bool TouchesWall { get; private set; }
    public bool EdgeDetected { get; private set; }
    public Vector3 EdgePosition { get; private set; }

    // Movement
    public Vector2 MoveInput { get; protected set; }
    public bool IsMoving => Mathf.Abs(MoveInput.x) > 0.1f;
    public int FacingDirection { get; set; } = 1;

    // Components
    public Rigidbody Rb { get; private set; }
    public Animator Anim { get; private set; }

    // Logging
    public bool UseLogs = true;

    protected override void Awake()
    {
        base.Awake();
        Rb = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        UpdateGroundCheck();
        UpdateFrontCheck();
        UpdateAnimatorParams();
        base.Update();
    }

    private void UpdateGroundCheck()
    {
        // Use gravity direction from GravityStateMachine if available,
        // otherwise fall back to Vector3.down
        Vector3 gravityDirection = GravityStateMachine.Instance != null
            ? GravityStateMachine.Instance.GetGravityVector().normalized
            : Vector3.down;

        IsGrounded = Physics.SphereCast(
            GroundCheck.position,
            GroundCheckRadius,
            gravityDirection,
            out RaycastHit _,
            GroundCheckDistance,
            GroundLayer
        );
    }

    private void UpdateFrontCheck()
    {
        Collider[] frontHits = Physics.OverlapBox(
            FrontCheck.position,
            FrontCheckSize / 2,
            FrontCheck.rotation,
            WallLayer | EdgeLayer
        );

        TouchesWall = false;
        EdgeDetected = false;

        foreach (Collider col in frontHits)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Wall"))
                TouchesWall = true;

            if (col.gameObject.layer == LayerMask.NameToLayer("Edge"))
            {
                EdgeDetected = true;
                EdgePosition = new Vector3(
                    transform.position.x,
                    col.bounds.max.y,
                    transform.position.z
                );
            }
        }
    }

    private void UpdateAnimatorParams()
    {
        Anim.SetBool("isGrounded", IsGrounded);
        Anim.SetBool("touchesWall", TouchesWall);
        Anim.SetBool("edgeDetected", EdgeDetected);
    }

    private void OnDrawGizmos()
    {
        if (GroundCheck != null)
        {
            Vector3 gravityDirection = GravityStateMachine.Instance != null
                ? GravityStateMachine.Instance.GetGravityVector().normalized
                : Vector3.down;

            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(GroundCheck.position, GroundCheckRadius);
            Gizmos.DrawRay(GroundCheck.position, gravityDirection * GroundCheckDistance);
        }

        if (PivotCheck != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(PivotCheck.position, 0.1f);
        }

        if (FrontCheck != null)
        {
            Gizmos.color = TouchesWall ? Color.blue : EdgeDetected ? Color.green : Color.cyan;
            Gizmos.matrix = Matrix4x4.TRS(FrontCheck.position, FrontCheck.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, FrontCheckSize);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    public void Log(string message)
    {
        if (UseLogs)
            Debug.Log(message);
    }
}