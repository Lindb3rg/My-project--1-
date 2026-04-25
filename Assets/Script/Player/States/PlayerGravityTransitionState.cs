using UnityEngine;

public class PlayerGravityTransitionState : BaseState<PlayerStateMachine.EPlayerState>
{
    private readonly PlayerContext _ctx;

    // Phase tracking
    private enum TransitionPhase { FloatUp, Hover, Rotate, Done }
    private TransitionPhase _phase;
    private float _phaseTimer;

    // Float phase
    private Vector3 _floatStartPosition;
    private Vector3 _floatPeakPosition;

    // Rotate phase
    private Quaternion _startRotation;
    private Quaternion _targetRotation;
    private float _rotateTimer;
    private Vector3 _initialPivotToRoot; // captured once at rotation start

    // Safety timeout
    private float _timeoutTimer;
    private const float MaxTransitionTime = 5f;

    public PlayerGravityTransitionState(PlayerStateMachine.EPlayerState key, PlayerContext ctx) : base(key)
    {
        _ctx = ctx;
    }

    public override void EnterState()
    {
        _phase        = TransitionPhase.FloatUp;
        _phaseTimer   = 0f;
        _timeoutTimer = 0f;

        GravityContext gravCtx = GravityStateMachine.Instance.Context;

        // Zero velocity first then go kinematic — prevents input momentum carrying into transition
        _ctx.Rb.linearVelocity = Vector3.zero;
        _ctx.Rb.angularVelocity = Vector3.zero;
        _ctx.Rb.isKinematic = true;

        // Float away from current ground — use PreviousDirection not TargetDirection
        GravityStateMachine gm  = GravityStateMachine.Instance;
        Vector3 antiGravDir     = -GravityDirectionToVector(gravCtx.PreviousDirection);
        _floatStartPosition     = _ctx.Transform.position;

        _floatPeakPosition = (gravCtx.PreviousDirection == GravityDirection.Left || gravCtx.PreviousDirection == GravityDirection.Right)
            ? new Vector3(_floatStartPosition.x + antiGravDir.x * gm.FloatPeakHeight, _floatStartPosition.y, _floatStartPosition.z)
            : new Vector3(_floatStartPosition.x, _floatStartPosition.y + antiGravDir.y * gm.FloatPeakHeight, _floatStartPosition.z);

        // Store target rotation for rotate phase
        _targetRotation = GetSnappedRotation(gravCtx.TargetDirection);

        _ctx.Anim.SetTrigger("gravityTransition");
    }

    public override void UpdateState()
    {
        _timeoutTimer += Time.deltaTime;
        _phaseTimer   += Time.deltaTime;

        GravityStateMachine gm = GravityStateMachine.Instance;

        switch (_phase)
        {
            case TransitionPhase.FloatUp:   UpdateFloatUp(gm);   break;
            case TransitionPhase.Hover:     UpdateHover(gm);     break;
            case TransitionPhase.Rotate:    UpdateRotate(gm);    break;
        }
    }

    private void UpdateFloatUp(GravityStateMachine gm)
    {
        float t       = Mathf.Clamp01(_phaseTimer / gm.FloatRiseDuration);
        float smoothT = Mathf.SmoothStep(0f, 1f, t);
        Vector3 newPos = Vector3.Lerp(_floatStartPosition, _floatPeakPosition, smoothT);

        // Lock axes that shouldn't move during float
        GravityDirection previous = GravityStateMachine.Instance.Context.PreviousDirection;
        _ctx.Transform.position = (previous == GravityDirection.Left || previous == GravityDirection.Right)
            ? new Vector3(newPos.x, _floatStartPosition.y, _floatStartPosition.z)
            : new Vector3(_floatStartPosition.x, newPos.y, _floatStartPosition.z);

        if (_phaseTimer >= gm.FloatRiseDuration)
        {
            _ctx.Transform.position = _floatPeakPosition;
            _phase      = TransitionPhase.Hover;
            _phaseTimer = 0f;
        }
    }

    private void UpdateHover(GravityStateMachine gm)
    {
        if (_phaseTimer >= gm.HoverDuration)
        {
            _startRotation = _ctx.Transform.rotation;

            // Capture pivot offset once — pivot position is live but offset is fixed
            Vector3 pivot      = _ctx.PivotCheck != null ? _ctx.PivotCheck.position : _ctx.Transform.position;
            _initialPivotToRoot = _ctx.Transform.position - pivot;

            _phase       = TransitionPhase.Rotate;
            _phaseTimer  = 0f;
            _rotateTimer = 0f;
        }
    }

    private void UpdateRotate(GravityStateMachine gm)
    {
        _rotateTimer += Time.deltaTime;
        float t       = Mathf.Clamp01(_rotateTimer / gm.RotateDuration);
        float curvedT = gm.RotationCurve.Evaluate(t);

        // Pivot position is live (child moves with character during float)
        // but the initial offset is fixed so rotation stays on the spot
        Vector3 pivot        = _ctx.PivotCheck != null ? _ctx.PivotCheck.position : _ctx.Transform.position;
        Quaternion newRot    = Quaternion.Slerp(_startRotation, _targetRotation, curvedT);
        Quaternion rotDelta  = newRot * Quaternion.Inverse(_startRotation);

        _ctx.Transform.rotation = newRot;
        _ctx.Transform.position = pivot + rotDelta * _initialPivotToRoot;

        Debug.DrawRay(pivot, Vector3.up * 0.5f, Color.cyan, 0.1f);

        if (_rotateTimer >= gm.RotateDuration)
        {
            _ctx.Transform.rotation = _targetRotation;
            _phase = TransitionPhase.Done;
        }
    }

    public override void FixedUpdateState() { }
    public override void LateUpdateState()  { }

    public override void ExitState()
    {
        _ctx.Rb.isKinematic = false;
        _ctx.Transform.rotation = GetSnappedRotation(_ctx.CurrentGravityDirection);
    }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        if (_timeoutTimer >= MaxTransitionTime)
            return PlayerStateMachine.EPlayerState.Fall;

        if (_phase == TransitionPhase.Done)
            return PlayerStateMachine.EPlayerState.Fall;

        return StateKey;
    }

    // --- Helpers ---

    private Vector3 GravityDirectionToVector(GravityDirection dir) => dir switch
    {
        GravityDirection.Up    => Vector3.up,
        GravityDirection.Left  => Vector3.left,
        GravityDirection.Right => Vector3.right,
        _                      => Vector3.down,
    };

    private Quaternion GetSnappedRotation(GravityDirection gravityDirection)
    {
        float facingY = _ctx.FacingDirection == 1 ? 90f : -90f;

        return gravityDirection switch
        {
            GravityDirection.Up    => Quaternion.Euler(0f,   facingY,  180f),
            GravityDirection.Left  => Quaternion.Euler(-90f, 0f,       -90f),
            GravityDirection.Right => Quaternion.Euler(-90f, 0f,        90f),
            _                      => Quaternion.Euler(0f,   facingY,    0f),
        };
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }
}