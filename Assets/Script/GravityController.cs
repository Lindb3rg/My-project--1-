using UnityEngine;
using UnityEngine.Events;

public enum GravityDirection { Down, Up, Left, Right,None }

public class GravityController : MonoBehaviour
{
    public static GravityController Instance { get; private set; }

    [SerializeField] private float gravityStrength = 9.81f;
    [SerializeField] private GravityDirection initialDirection = GravityDirection.Down;

    // Change this in the Inspector at runtime to instantly switch gravity
    [SerializeField] private GravityDirection debugDirection;
    private GravityDirection lastDirection;

    public GravityDirection CurrentDirection { get; private set; }
    public Vector3 GravityVector { get; private set; }

    public UnityEvent<GravityDirection> OnGravityChanged = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        SetGravity(initialDirection);
        debugDirection = initialDirection;
        lastDirection = initialDirection;
    }

    void Update()
    {
        if (debugDirection != lastDirection)
        {
            SetGravity(debugDirection);
            lastDirection = debugDirection;
        }
    }

    public void SetGravity(GravityDirection dir)
    {
        CurrentDirection = dir;
        GravityVector = dir switch
        {
            GravityDirection.Up => Vector3.up * gravityStrength,
            GravityDirection.Left => Vector3.left * gravityStrength,
            GravityDirection.Right => Vector3.right * gravityStrength,
            GravityDirection.None => Vector3.zero,
            _ => Vector3.down * gravityStrength,
        };
        Physics.gravity = GravityVector;
        OnGravityChanged.Invoke(CurrentDirection);
    }
}