using UnityEngine;

public class AntagonistRoarState : IState
{
    private readonly AntagonistStateMachine _ctx;
    private readonly StateMachine _sm;



    public AntagonistRoarState(AntagonistStateMachine ctx, StateMachine sm)
    {
        _ctx = ctx;
        _sm = sm;
    }

    public void Enter()
    {

        _ctx.Anim.SetTrigger("Roar");
    }

    public void Exit() { }






    public void Tick()
    {
        AnimatorStateInfo info = _ctx.Anim.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Roar") && info.normalizedTime >= 1f)
        {
            _sm.ChangeState(_ctx.Idle);
        }
    }

    public void FixedTick()
    {


    }
    public void LateTick() { }
}