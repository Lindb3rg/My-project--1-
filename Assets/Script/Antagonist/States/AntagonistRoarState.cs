using UnityEngine;

public class AntagonistRoarState : BaseState<AntagonistStateMachine.EAntagonistState>
{
    private readonly AntagonistStateMachine _ctx;



    public AntagonistRoarState(AntagonistStateMachine.EAntagonistState key, AntagonistStateMachine context) : base(key)
    {
        _ctx = context;
    }

    public override void EnterState()
    {

        _ctx.Anim.SetTrigger("Roar");
    }

    public override void ExitState() { }


    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {


    }
    public override void LateUpdateState() { }

    public override AntagonistStateMachine.EAntagonistState GetNextState() => StateKey;

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }
}