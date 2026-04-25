public class GravityBlockedState : BaseGravityState
{
    public GravityBlockedState(GravityState key, GravityContext context) : base(key, context) { }

    public override void EnterState() { }

    public override void UpdateState() { }

    public override void ExitState() { }

    public override GravityState GetNextState() => StateKey;
}