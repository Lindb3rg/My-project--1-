using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EnvironmentInteractionContext
{
    public enum EEBodySide
    {
        RIGHT,
        LEFT

    }
    private readonly TwoBoneIKConstraint _leftLegIkConstraint;
    private readonly TwoBoneIKConstraint _rightLegIkConstraint;
    private readonly TwoBoneIKConstraint _leftHandIkConstraint;
    private readonly TwoBoneIKConstraint _rightHandIkConstraint;
    private readonly Rigidbody _rigidBody;
    private readonly CapsuleCollider _rootCollider;
    private readonly Transform _rootTransform;
    private readonly Vector3 _leftOriginalTargetPosition;
    private readonly Vector3 _rightOriginalTargetPosition;

    public EnvironmentInteractionContext(TwoBoneIKConstraint leftLegIkConstraint, TwoBoneIKConstraint rightLegIkConstraint, TwoBoneIKConstraint leftHandIkConstraint, TwoBoneIKConstraint rightHandIkConstraint, Rigidbody rigidBody, CapsuleCollider rootCollider, Transform rootTransform)
    {

        _leftLegIkConstraint = leftLegIkConstraint;
        _rightLegIkConstraint = rightLegIkConstraint;
        _leftHandIkConstraint = leftHandIkConstraint;
        _rightHandIkConstraint = rightHandIkConstraint;
        _rigidBody = rigidBody;
        _rootCollider = rootCollider;
        _rootTransform = rootTransform;
        _leftOriginalTargetPosition = _leftHandIkConstraint.data.target.transform.localPosition;
        _rightOriginalTargetPosition = _leftHandIkConstraint.data.target.transform.localPosition;

        CharacterLeftShoulderHeight = leftHandIkConstraint.data.root.transform.position.y;
        CharacterRightShoulderHeight = rightHandIkConstraint.data.root.transform.position.y;
        CharacterRightShoulderTargetPosition = rightHandIkConstraint.data.target.transform.position;
        SetCurrentIkTransforms();


    }

    // Read-only properties
    public TwoBoneIKConstraint LeftHandIkConstraint => _leftHandIkConstraint;
    public TwoBoneIKConstraint RightHandIkConstraint => _rightHandIkConstraint;
    public TwoBoneIKConstraint LeftLegIkConstraint => _leftLegIkConstraint;
    public TwoBoneIKConstraint RightLegIkConstraint => _rightLegIkConstraint;


    public Rigidbody Rb => _rigidBody;
    public CapsuleCollider RootCollider => _rootCollider;
    public Transform RootTransform => _rootTransform;

    public Collider CurrentIntersectingCollider { get; set; }


    public TwoBoneIKConstraint CurrentLeftIkConstraint { get; set; }
    public TwoBoneIKConstraint CurrentRightIkConstraint { get; set; }



    public Transform CurrentLeftIkTargetTransform { get; set; }
    public Transform CurrentRightIkTargetTransform { get; set; }

    public Transform CurrentLeftShoulderTransform { get; private set; }
    public Transform CurrentRightShoulderTransform { get; private set; }

    public EEBodySide CurrentBodySide { get; private set; }

    public Vector3 ClosestPointOnColliderFromLeftShoulder { get; set; } = Vector3.positiveInfinity;
    public Vector3 ClosestPointOnColliderFromRightShoulder { get; set; } = Vector3.positiveInfinity;

    public float CharacterLeftShoulderHeight;
    public float CharacterRightShoulderHeight;

    public Vector3 CharacterRightShoulderTargetPosition;
    public Vector3 CurrentLeftOriginalTargetPosition { get; private set; }
    public Vector3 CurrentRightOriginalTargetPosition { get; private set; }
    public float LowestDistance { get; set; } = Mathf.Infinity;

    public void SetCurrentIkTransforms()
    {
        // Left side
        CurrentLeftIkConstraint = _leftHandIkConstraint;
        CurrentLeftShoulderTransform = _leftHandIkConstraint.data.root.transform;
        CurrentLeftIkTargetTransform = _leftHandIkConstraint.data.target.transform;
        CurrentLeftOriginalTargetPosition = _leftOriginalTargetPosition;


        // Right side
        CurrentRightIkConstraint = _rightHandIkConstraint;
        CurrentRightShoulderTransform = _rightHandIkConstraint.data.root.transform;
        CurrentRightIkTargetTransform = _rightHandIkConstraint.data.target.transform;
        CurrentRightOriginalTargetPosition = _rightOriginalTargetPosition;
    }


}