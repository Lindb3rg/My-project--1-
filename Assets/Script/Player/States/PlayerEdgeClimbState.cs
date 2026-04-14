using UnityEngine;

public class PlayerEdgeClimbState : IState
{
    private readonly PlayerStateMachine _ctx;
    private readonly StateMachine _sm;
    private bool _isRepositioning;
    private Vector3 _targetPosition;
    private float _climbSpeed;
    private float _maxClimbSpeed = 2f;

    public PlayerEdgeClimbState(PlayerStateMachine ctx, StateMachine sm)
    {
        _ctx = ctx;
        _sm = sm;
    }

    public void Enter()
    {

        _ctx.Rb.useGravity = false;
        _ctx.Rb.linearVelocity = Vector3.zero;
        _ctx.Anim.applyRootMotion = true;

        _ctx.transform.position = new Vector3(
            _ctx.transform.position.x,
            _ctx.EdgePosition.y - _ctx.ClimbLedgeOffset,
            _ctx.transform.position.z
        );
        _ctx.Anim.SetBool("isRunning", false);
        _ctx.Anim.SetTrigger("edgeClimbTrigger");
        _ctx.Anim.SetFloat("climbSpeed", 1.2f);

    }

    public void Exit()
    {
        _ctx.Anim.applyRootMotion = false;
        _ctx.Anim.SetFloat("climbSpeed", 1.2f);
        // Correct position BEFORE re-enabling gravity
        _ctx.transform.position = new Vector3(
            _ctx.transform.position.x,
            _ctx.EdgePosition.y + _ctx.ClimbLedgeOffset,
            _ctx.transform.position.z
        );

        // Zero velocity BEFORE re-enabling gravity
        _ctx.Rb.linearVelocity = Vector3.zero;

        // Re-enable gravity last
        _ctx.Rb.useGravity = true;

        if (_ctx.MeshChild != null)
        {
            _ctx.MeshChild.localPosition = Vector3.zero;
            _ctx.MeshChild.localRotation = Quaternion.identity;
        }
    }



    public void Tick()
    {
        if (_ctx.JumpPressed)
        {
            Debug.Log(_ctx.JumpPressed);
            _climbSpeed += 0.2f;  // increase by 0.2 each press
            _climbSpeed = Mathf.Clamp(_climbSpeed, 1f, _maxClimbSpeed);
            _ctx.Anim.SetFloat("climbSpeed", _climbSpeed);
        }
        AnimatorStateInfo stateInfo = _ctx.Anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("edge_climb") && stateInfo.normalizedTime >= 0.8f)
        {
            _sm.ChangeState(_ctx.Idle);
        }

    }

    public void FixedTick()
    {


    }
    public void LateTick() { }
}