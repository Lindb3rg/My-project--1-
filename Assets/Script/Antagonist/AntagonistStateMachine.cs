using UnityEngine;
using UnityEngine.InputSystem;
using Divers;

public class AntagonistStateMachine : CharacterStateMachine
{

    AudioSource audioSource;
    public AudioClip audioClip;
    public float roarCooldown = 5f;
    private float _roarTimer;

    public AntagonistIdleState Idle { get; private set; }
    public AntagonistRoarState Roar { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        _sm = new StateMachine();


        Idle = new AntagonistIdleState(this, _sm);
        Roar = new AntagonistRoarState(this, _sm);
        audioSource = GetComponent<AudioSource>();

    }

    void Start() => _sm.ChangeState(Idle);

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _sm.ChangeState(Roar);
        }
    }

    protected override void Update()
    {
        base.Update();
        _sm.Tick();
    }

    public void RoarSound()
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void TriggerRoar()
    {
        if (Time.time >= _roarTimer)
    {
        _roarTimer = Time.time + roarCooldown;
        _sm.ChangeState(Roar);
    }
    }




}