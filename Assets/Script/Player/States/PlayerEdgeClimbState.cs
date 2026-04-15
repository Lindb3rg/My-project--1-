using UnityEngine;

public class PlayerEdgeClimbState : BaseState<PlayerStateMachine.EPlayerState>
{
    private readonly PlayerStateMachine _ctx;
    private float _climbSpeed;
    private float _maxClimbSpeed = 2f;

    public PlayerEdgeClimbState(PlayerStateMachine.EPlayerState key, PlayerStateMachine ctx) : base(key)
    {
        _ctx = ctx;
    }

    public override void EnterState()
    {
        _climbSpeed = 1.2f;
        _ctx.Rb.useGravity      = false;
        _ctx.Rb.linearVelocity  = Vector3.zero;
        _ctx.Anim.applyRootMotion = true;

        _ctx.transform.position = new Vector3(
            _ctx.transform.position.x,
            _ctx.EdgePosition.y - _ctx.ClimbLedgeOffset,
            _ctx.transform.position.z
        );

        _ctx.Anim.SetBool("isRunning", false);
        _ctx.Anim.SetTrigger("edgeClimbTrigger");
        _ctx.Anim.SetFloat("climbSpeed", _climbSpeed);
    }

    public override void ExitState()
    {
        _ctx.Anim.applyRootMotion = false;
        _ctx.Anim.SetFloat("climbSpeed", _climbSpeed);

        _ctx.transform.position = new Vector3(
            _ctx.transform.position.x,
            _ctx.EdgePosition.y + _ctx.ClimbLedgeOffset,
            _ctx.transform.position.z
        );

        _ctx.Rb.linearVelocity = Vector3.zero;
        _ctx.Rb.useGravity     = true;

        if (_ctx.MeshChild != null)
        {
            _ctx.MeshChild.localPosition = Vector3.zero;
            _ctx.MeshChild.localRotation = Quaternion.identity;
        }
    }

    public override void UpdateState()
    {
        if (_ctx.JumpPressed)
        {
            _climbSpeed = Mathf.Clamp(_climbSpeed + 0.2f, 1f, _maxClimbSpeed);
            _ctx.Anim.SetFloat("climbSpeed", _climbSpeed);
        }

        AnimatorStateInfo stateInfo = _ctx.Anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("edge_climb") && stateInfo.normalizedTime >= 0.8f)
            _ctx.TransitionToState(PlayerStateMachine.EPlayerState.Idle);
    }

    public override void FixedUpdateState() { }
    public override void LateUpdateState()  { }

    public override PlayerStateMachine.EPlayerState GetNextState() => StateKey;

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }
}