using UnityEngine;

public class PlayerIdleState : IState
{
    private readonly PlayerStateMachine _ctx;
    private readonly StateMachine _sm;

    public PlayerIdleState(PlayerStateMachine ctx, StateMachine sm)
    {
        _ctx = ctx;
        _sm = sm;
    }

    public void Enter()
    {

        Debug.Log("Entered IdleState");
        _ctx.Anim.SetBool("inAir", false);
        



    }

    public void Exit()
    {

        

    }




    public void Tick()
    {


        if (_ctx.JumpPressed)
        {
            if (_ctx.CoyoteTimeCounter > 0f) _sm.ChangeState(_ctx.Jump);

        }




        if (_ctx.IsMoving)
        {
            bool movingAwayFromWall = (_ctx.TouchesWall && _ctx.FacingDirection == 1 && _ctx.MoveInput.x < 0)
                                   || (_ctx.TouchesWall && _ctx.FacingDirection == -1 && _ctx.MoveInput.x > 0)
                                   || !_ctx.TouchesWall;  // ← no wall, always free

            if (movingAwayFromWall)
                _sm.ChangeState(_ctx.Move);
        }





        if (!_ctx.IsGrounded)
        {
            _ctx.Anim.SetTrigger("fallTrigger");
            _sm.ChangeState(_ctx.Air);
            return;
        }


    }

    public void FixedTick()
    {
        if (_ctx.MoveInput.x < 0.1 || _ctx.MoveInput.x > -0.1)
            _ctx.Rb.linearVelocity = new Vector3(0f, _ctx.Rb.linearVelocity.y, 0f);

    }
    public void LateTick()
    {

        _ctx.UpdateWallHandIK();
    
     }
}