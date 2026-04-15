using UnityEngine;

public class PlayerLandState : BaseState<PlayerStateMachine.EPlayerState>
{
    private readonly PlayerStateMachine _ctx;

    public PlayerLandState(PlayerStateMachine.EPlayerState key, PlayerStateMachine ctx) : base(key)
    {
        _ctx = ctx;
    }

    public override void EnterState()
    {
        if (_ctx.Fall.DidDoubleJump)
            _ctx.Anim.Play(PlayerAnimations.DoubleJumpLand);
        else
            _ctx.TransitionToState(PlayerStateMachine.EPlayerState.Idle);
    }

    public override void ExitState() { }

    public override void UpdateState()
    {
        if (_ctx.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            _ctx.TransitionToState(_ctx.MoveInput.x != 0
                ? PlayerStateMachine.EPlayerState.Walk
                : PlayerStateMachine.EPlayerState.Idle);
        }
    }

    public override void FixedUpdateState() { }
    public override void LateUpdateState()  { }

    public override PlayerStateMachine.EPlayerState GetNextState() => StateKey;

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }
}