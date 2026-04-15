using UnityEngine;

public class AntagonistIdleState : BaseState<AntagonistStateMachine.EAntagonistState>
{
    private readonly AntagonistStateMachine _ctx;

    public AntagonistIdleState(AntagonistStateMachine.EAntagonistState key, AntagonistStateMachine context) : base(key)
    {
        _ctx = context;
    }

    public override void EnterState()
    {
    }

    public override void ExitState() { }




    public override void UpdateState()
    {

    }

    public override void FixedUpdateState()
    {
        if (_ctx.MoveInput.x < 0.1 || _ctx.MoveInput.x > -0.1)
            _ctx.Rb.linearVelocity = new Vector3(0f, _ctx.Rb.linearVelocity.y, 0f);

    }
    public override void LateUpdateState() { }
    public override AntagonistStateMachine.EAntagonistState GetNextState() => StateKey;

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }
}