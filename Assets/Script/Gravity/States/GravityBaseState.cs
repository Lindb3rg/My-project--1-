using UnityEngine;

public abstract class BaseGravityState : BaseState<GravityState>
{
    protected GravityContext Context;

    public BaseGravityState(GravityState key, GravityContext context) : base(key)
    {
        Context = context;
    }

    // --- Shared helpers available to all gravity states ---

    protected bool IsCooldownComplete()
    {
        return Context.CooldownTimer <= 0f;
    }

    protected bool IsTransitionComplete()
    {
        return Context.TransitionTimer <= 0f;
    }

    protected Vector3 DirectionToVector(GravityDirection direction)
    {
        return direction switch
        {
            GravityDirection.Up    => Vector3.up    * Context.GravityStrength,
            GravityDirection.Left  => Vector3.left  * Context.GravityStrength,
            GravityDirection.Right => Vector3.right * Context.GravityStrength,
            GravityDirection.None  => Vector3.zero,
            _                      => Vector3.down  * Context.GravityStrength,
        };
    }

    protected void ApplyGravity(GravityDirection direction)
    {
        Context.GravityVector = DirectionToVector(direction);
        Context.CurrentDirection = direction;
        Physics.gravity = Context.GravityVector;
    }

    protected void TickCooldown()
    {
        if (Context.CooldownTimer > 0f)
            Context.CooldownTimer -= Time.deltaTime;
    }

    protected void TickTransition()
    {
        if (Context.TransitionTimer > 0f)
            Context.TransitionTimer -= Time.deltaTime;
    }

    // --- Default empty implementations for unused callbacks ---

    public override void FixedUpdateState() { }
    public override void LateUpdateState() { }
    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
    public override void OnTriggerExit(Collider other) { }
}