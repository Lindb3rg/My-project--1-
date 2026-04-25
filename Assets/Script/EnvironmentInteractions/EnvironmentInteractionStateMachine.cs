using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class EnvironmentInteractionStateMachine : StateManager<EnvironmentInteractionStateMachine.EEnvironmentInteractionState>
{
    public enum EEnvironmentInteractionState
    {
        Search,
        Approach,
        Rise,
        Touch,
        Reset

    }

    private EnvironmentInteractionContext _context;

    [SerializeField] private TwoBoneIKConstraint _leftLegIkConstraint;
    [SerializeField] private TwoBoneIKConstraint _rightLegIkConstraint;
    [SerializeField] private TwoBoneIKConstraint _leftHandIkConstraint;
    [SerializeField] private TwoBoneIKConstraint _rightHandIkConstraint;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private CapsuleCollider _rootCollider;

    private void ValidateConstraints()
    {

        Assert.IsNotNull(_leftLegIkConstraint, "Left Leg IK constraint is not assigned.");
        Assert.IsNotNull(_rightLegIkConstraint, "Right Leg IK constraint is not assigned.");
        Assert.IsNotNull(_leftHandIkConstraint, "Left Hand IK constraint is not assigned.");
        Assert.IsNotNull(_rightHandIkConstraint, "Right Hand IK constraint is not assigned.");
        Assert.IsNotNull(_rigidBody, "Rigidbody is not assigned.");
        Assert.IsNotNull(_rootCollider, "Capsule collider is not assigned.");

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (_context != null && _context.ClosestPointOnColliderFromLeftShoulder != null)
        {
            Gizmos.DrawSphere(_context.ClosestPointOnColliderFromLeftShoulder, .03f);
        }
        Gizmos.color = Color.green;
        if (_context != null && _context.ClosestPointOnColliderFromRightShoulder != null)
        {
            Gizmos.DrawSphere(_context.ClosestPointOnColliderFromRightShoulder, .03f);
        }
    }

    private void InitializeStates()
    {
        States.Add(EEnvironmentInteractionState.Reset, new ResetState(_context, EEnvironmentInteractionState.Reset));
        States.Add(EEnvironmentInteractionState.Search, new SearchState(_context, EEnvironmentInteractionState.Search));
        States.Add(EEnvironmentInteractionState.Approach, new ApproachState(_context, EEnvironmentInteractionState.Approach));
        States.Add(EEnvironmentInteractionState.Rise, new RiseState(_context, EEnvironmentInteractionState.Rise));
        States.Add(EEnvironmentInteractionState.Touch, new TouchState(_context, EEnvironmentInteractionState.Touch));

        CurrentState = States[EEnvironmentInteractionState.Reset];


    }
    protected override void Awake()
    {
        ValidateConstraints();
        _context = new EnvironmentInteractionContext(_leftLegIkConstraint, _rightLegIkConstraint, _leftHandIkConstraint, _rightHandIkConstraint, _rigidBody, _rootCollider, transform.root);
        InitializeStates();
        ConstructEnvironmentDetectionCollider();



    }

    private void ConstructEnvironmentDetectionCollider()
    {
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(0.2224872f, 0.9084835f, 0.4958839f);
        boxCollider.center = new Vector3(0.0006010415f, 0.5084553f, 0.334828f);
        boxCollider.isTrigger = true;
    }
}