using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;

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

    protected override void Awake()
    {
        ValidateConstraints();
        _context = new EnvironmentInteractionContext(_leftLegIkConstraint, _rightLegIkConstraint, _leftHandIkConstraint, _rightHandIkConstraint, _rigidBody, _rootCollider);
        States[EEnvironmentInteractionState.Reset] = new ResetState(_context, EEnvironmentInteractionState.Reset);
        CurrentState = States[EEnvironmentInteractionState.Reset];
    }
}