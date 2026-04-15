using UnityEngine;

public class ResetState : EnvironmentInteractionState
{

    public ResetState(
    EnvironmentInteractionContext context,
    EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate
    ) : base(context, estate)

    {

    }

    public override void EnterState(){}
    public override void UpdateState(){}
    public override void FixedUpdateState(){}
    public override void LateUpdateState(){}
    public override void ExitState(){}
    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {

        return StateKey;

    }
    public override void OnTriggerEnter(Collider other){}
    public override void OnTriggerStay(Collider other){}
    public override void OnTriggerExit(Collider other){}



}