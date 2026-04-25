public class GravityTransitioningState : BaseGravityState
{
    private float _zeroGravTimer;
    private bool _gravityApplied;

    public GravityTransitioningState(GravityState key, GravityContext context) : base(key, context) { }

    public override void EnterState()
    {
        Context.TransitionTimer = Context.TransitionDuration;
        Context.CooldownTimer   = Context.FlipCooldown;
        _zeroGravTimer          = 0f;
        _gravityApplied         = false;

        GravityStateMachine gm = GravityStateMachine.Instance;

        if (gm.EnableZeroGravityWindow)
        {
            // Apply zero gravity first — loose Rigidbodies start floating
            ApplyGravity(GravityDirection.None);
        }
        else
        {
            // Apply new gravity immediately
            ApplyGravity(Context.TargetDirection);
            _gravityApplied = true;
        }

        Context.OnGravityFlipStarted.Invoke(Context.TargetDirection);
    }

    public override void UpdateState()
    {
        TickTransition();

        if (_gravityApplied) return;

        _zeroGravTimer += UnityEngine.Time.deltaTime;

        if (_zeroGravTimer >= GravityStateMachine.Instance.ZeroGravityDuration)
        {
            ApplyGravity(Context.TargetDirection);
            Context.OnGravityFlipCompleted.Invoke(Context.CurrentDirection);
            _gravityApplied = true;
        }
    }

    public override void ExitState() { }

    public override GravityState GetNextState()
    {
        if (!_gravityApplied) return StateKey;
        return GravityState.Settled;
    }
}