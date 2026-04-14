using UnityEngine;

public class PlayerLandState : IState
{
    private readonly PlayerStateMachine _ctx;
    private readonly StateMachine _sm;

    public PlayerLandState(PlayerStateMachine ctx, StateMachine sm)
    {
        _ctx = ctx;
        _sm = sm;
    }

    public void Enter()
    {
        // Play correct landing animation based on whether double jumped
        Debug.Log("Entered Jump Land state");
        if (_ctx.Air.DidDoubleJump)
        {
            _ctx.Anim.Play(PlayerAnimations.DoubleJumpLand);
            _ctx.Rb.linearVelocity = new Vector3(
            _ctx.FacingDirection * _ctx.RollSpeed,
                0f,
                0f
            );


        }
        else
            _sm.ChangeState(_ctx.Idle);
    }

    public void Exit() { }

    public void Tick()
    {
        // Wait for animation to finish then transition
        if (_ctx.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            if (_ctx.MoveInput.x != 0)
                _sm.ChangeState(_ctx.Move);
            else
                _sm.ChangeState(_ctx.Idle);
        }
    }

    public void FixedTick() { }
    public void LateTick() { }
}