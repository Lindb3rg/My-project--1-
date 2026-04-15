using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EnvironmentInteractionContext
{
    private readonly TwoBoneIKConstraint _leftLegIkConstraint;
    private readonly TwoBoneIKConstraint _rightLegIkConstraint;
    private readonly TwoBoneIKConstraint _leftHandIkConstraint;
    private readonly TwoBoneIKConstraint _rightHandIkConstraint;
    private readonly Rigidbody _rigidBody;
    private readonly CapsuleCollider _rootCollider;

    public EnvironmentInteractionContext(TwoBoneIKConstraint leftLegIkConstraint, TwoBoneIKConstraint rightLegIkConstraint, TwoBoneIKConstraint leftHandIkConstraint, TwoBoneIKConstraint rightHandIkConstraint, Rigidbody rigidBody, CapsuleCollider rootCollider)
    {

        _leftLegIkConstraint = leftLegIkConstraint;
        _rightLegIkConstraint = rightLegIkConstraint;
        _leftHandIkConstraint = leftHandIkConstraint;
        _rightHandIkConstraint = rightHandIkConstraint;
        _rigidBody = rigidBody;
        _rootCollider = rootCollider;

    }

    public TwoBoneIKConstraint LeftLegIkConstraint => _leftLegIkConstraint;
    public TwoBoneIKConstraint RightLegIkConstraint => _rightLegIkConstraint;
    public TwoBoneIKConstraint LeftHandIkConstraint => _leftHandIkConstraint;
    public TwoBoneIKConstraint RightHandIkConstraint => _rightHandIkConstraint;

    public Rigidbody Rb => _rigidBody;
    public CapsuleCollider RootCollider => _rootCollider;

}