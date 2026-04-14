using UnityEngine;

public class PlayerMoveState : IState
{
    private readonly PlayerStateMachine _ctx;
    private readonly StateMachine _sm;



    public PlayerMoveState(PlayerStateMachine ctx, StateMachine sm)
    {
        _ctx = ctx;
        _sm = sm;
    }

    public void Enter()
    {
        Debug.Log("Entered Move state");
        _ctx.Anim.SetBool("inAir", false);
        

    }

    public void Exit()
    {
        _ctx.Anim.SetBool("isSprinting", false);
        _ctx.Anim.SetBool("isRunning", false);
        _ctx.Anim.SetBool("isWalking", false);
        
    }


    public void Tick()
    {
        if (Mathf.Abs(_ctx.MoveInput.x) < 0.05f)
        {
            _sm.ChangeState(_ctx.Idle);
            return;
        }

        if (!_ctx.IsGrounded)
        {
            _ctx.Anim.SetTrigger("fallTrigger");
            _sm.ChangeState(_ctx.Air);
            return;
        }

        if (_ctx.JumpPressed)
        {
            if (_ctx.CoyoteTimeCounter > 0f) _sm.ChangeState(_ctx.Jump);
        }

        bool pushingIntoWall = (_ctx.TouchesWall && _ctx.FacingDirection == 1 && _ctx.MoveInput.x > 0)
                    || (_ctx.TouchesWall && _ctx.FacingDirection == -1 && _ctx.MoveInput.x < 0);

        if (pushingIntoWall)
        {
            _sm.ChangeState(_ctx.Idle);
            return;
        }

        _ctx.HandleTurning();

        // Snap animation to match FixedTick speed thresholds
        float magnitude = Mathf.Abs(_ctx.MoveInput.x);

        if (magnitude > 0.8f)
        {
            if (_ctx.SprintHeld)
            {
                _ctx.Anim.SetBool("isSprinting", true);
                _ctx.Anim.SetBool("isRunning", true);  // both true when sprinting
                _ctx.Anim.SetBool("isWalking", false);
            }
            else
            {
                _ctx.Anim.SetBool("isSprinting", false);
                _ctx.Anim.SetBool("isRunning", true);
                _ctx.Anim.SetBool("isWalking", false);
            }
        }
        else
        {
            _ctx.Anim.SetBool("isSprinting", false);
            _ctx.Anim.SetBool("isRunning", false);
            _ctx.Anim.SetBool("isWalking", true);
        }
    }

    public void FixedTick()
    {

        float magnitude = Mathf.Abs(_ctx.MoveInput.x);
        float speed = magnitude > 0.8f
            ? (_ctx.SprintHeld ? _ctx.SprintSpeed : _ctx.RunSpeed)
            : _ctx.WalkSpeed;

        _ctx.Rb.linearVelocity = new Vector3(
            _ctx.FacingDirection * speed,
            _ctx.Rb.linearVelocity.y,
            0f
        );
    }
    public void LateTick()
    {

        _ctx.UpdateWallHandIK();
    
     }
    
    
}


