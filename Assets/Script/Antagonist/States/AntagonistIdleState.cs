using UnityEngine;

public class AntagonistIdleState : IState
{
    private readonly AntagonistStateMachine _ctx;
    private readonly StateMachine _sm;

    public AntagonistIdleState(AntagonistStateMachine ctx, StateMachine sm)
    {
        _ctx = ctx;
        _sm = sm;
    }

    public void Enter()
    {




    }

    public void Exit() { }




    public void Tick()
    {

    }

    public void FixedTick()
    {
        if (_ctx.MoveInput.x < 0.1 || _ctx.MoveInput.x > -0.1)
            _ctx.Rb.linearVelocity = new Vector3(0f, _ctx.Rb.linearVelocity.y, 0f);

    }
    public void LateTick() { }
}