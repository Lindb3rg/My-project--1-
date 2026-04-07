using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float speed = 2f;

    private bool gravityKeyHeld = false;
    private float movementX;
    private float movementY;
    private GravityDirection currentGravity = GravityDirection.Down;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (GravityController.Instance != null)
        {
            currentGravity = GravityController.Instance.CurrentDirection;
            GravityController.Instance.OnGravityChanged.AddListener(OnGravityChanged);
        }
    }

    void OnDestroy()
    {
        if (GravityController.Instance != null)
            GravityController.Instance.OnGravityChanged.RemoveListener(OnGravityChanged);
    }

    void OnGravityChanged(GravityDirection dir)
    {
        currentGravity = dir;

        if (dir == GravityDirection.None)
            rb.linearVelocity = Vector3.zero;
    }

    void OnGravity(InputValue value)
    {
        gravityKeyHeld = value.isPressed;
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void OnJump()
    {

        if (currentGravity == GravityDirection.None) return;

        Vector3 jumpDir = currentGravity switch
        {
            GravityDirection.Down => Vector3.up,
            GravityDirection.Up => Vector3.down,
            GravityDirection.Left => Vector3.right,
            GravityDirection.Right => Vector3.left,
            _ => Vector3.up,
        };

        rb.AddForce(jumpDir * jumpForce, ForceMode.Impulse);
    }

    void Update()
{
    if (gravityKeyHeld)
    {
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)    GravityController.Instance.SetGravity(GravityDirection.Up);
        if (Keyboard.current.downArrowKey.wasPressedThisFrame)  GravityController.Instance.SetGravity(GravityDirection.Down);
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)  GravityController.Instance.SetGravity(GravityDirection.Left);
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame) GravityController.Instance.SetGravity(GravityDirection.Right);
        if (Keyboard.current.spaceKey.wasPressedThisFrame)      GravityController.Instance.SetGravity(GravityDirection.None);
    }
}

    void FixedUpdate()
    {
        Vector3 movement = currentGravity switch
        {
            GravityDirection.Down => new Vector3(movementX, 0f, 0f),
            GravityDirection.Up => new Vector3(movementX, 0f, 0f),
            GravityDirection.Left => new Vector3(0f, -movementX, 0f),
            GravityDirection.Right => new Vector3(0f, movementX, 0f),

            // Free float: X input moves horizontally, Y input moves vertically
            GravityDirection.None => new Vector3(movementX, movementY, 0f),
            _ => new Vector3(movementX, 0f, 0f),
        };

        rb.AddForce(movement * speed);
    }
}