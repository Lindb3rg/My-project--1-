using UnityEngine;


public class AntagonistTriggerZone : MonoBehaviour
{
    private AntagonistStateMachine _antagonist;

    void Awake()
    {
        _antagonist = GetComponentInParent<AntagonistStateMachine>();
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            _antagonist.TriggerRoar();
        }
    }
}