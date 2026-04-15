using UnityEngine;
using Divers;

public class AntagonistStateMachine : CharacterStateMachine<AntagonistStateMachine.EAntagonistState>
{

    public enum EAntagonistState
    {

        Idle,
        Roar

    }
    AudioSource audioSource;
    public AudioClip audioClip;
    public float roarCooldown = 5f;
    private float _roarTimer;

    

    protected override void Awake()
    {
        base.Awake();

    


        States[EAntagonistState.Idle] = new AntagonistIdleState(EAntagonistState.Idle, this);
        audioSource = GetComponent<AudioSource>();
        CurrentState = States[EAntagonistState.Idle];
    }

    protected override void Start() => base.Start();

    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         _sm.ChangeState(Roar);
    //     }
    // }

    protected override void Update()
    {
        base.Update();

    }

    public void RoarSound()
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void TriggerRoar()
    {
    //     if (Time.time >= _roarTimer)
    // {
    //     _roarTimer = Time.time + roarCooldown;
    //     _sm.ChangeState(Roar);
    // }
    }




}