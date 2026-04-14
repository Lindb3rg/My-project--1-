// CharacterStateMachine.cs
using UnityEngine;

public class CharacterStateMachine : MonoBehaviour
{

    //Ground check
    public float GroundCheckRadius = 0.2f;
    public float GroundCheckDistance = 0.5f;
    public LayerMask GroundLayer;
    public Transform GroundCheck;
    public bool IsGrounded { get; protected set; }

    
    
    

    // Front Check
    public bool TouchesWall { get; protected set; }
    public Transform FrontCheck;
    public Vector3 FrontCheckSize = new Vector3(0.1f, 1.5f, 0.1f);
    public LayerMask EdgeLayer;
    public LayerMask WallLayer;
    public bool EdgeDetected { get; protected set; }
    public Vector3 EdgePosition { get; protected set; }
    public bool DetectedWall { get; protected set; }


    // Movement
    public Vector2 MoveInput { get; protected set; }
    public bool IsMoving => Mathf.Abs(MoveInput.x) > 0.1f;


    public Rigidbody Rb { get; protected set; }
    public Animator Anim { get; protected set; }
    public int FacingDirection { get; set; } = 1;


    protected StateMachine _sm;

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();
    }



    protected virtual void Update()
    {
        IsGrounded = Physics.SphereCast(
            GroundCheck.position,
            GroundCheckRadius,
            Vector3.down,
            out RaycastHit hit,
            GroundCheckDistance,
            GroundLayer
        );



        
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




        Anim.SetBool("isGrounded", IsGrounded);
        Anim.SetBool("touchesWall", TouchesWall);
        Anim.SetBool("edgeDetected", EdgeDetected);



    }



    protected virtual void FixedUpdate() => _sm?.FixedTick();

    void OnDrawGizmos()
    {
        if (GroundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GroundCheck.position, GroundCheckRadius);
            Gizmos.DrawRay(GroundCheck.position, Vector3.down * GroundCheckDistance);
        }

        if (FrontCheck != null)
        {
            Gizmos.color = TouchesWall ? Color.blue : EdgeDetected ? Color.green : Color.cyan;
            Gizmos.matrix = Matrix4x4.TRS(FrontCheck.position, FrontCheck.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, FrontCheckSize);
            Gizmos.matrix = Matrix4x4.identity; // ← reset matrix after
        }

    }

}