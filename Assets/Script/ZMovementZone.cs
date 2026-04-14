using UnityEngine;

public class ZMovementZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints &= ~RigidbodyConstraints.FreezePositionZ;
                other.GetComponent<PlayerController>().inZMovementZone = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints |= RigidbodyConstraints.FreezePositionZ;
                other.GetComponent<PlayerController>().inZMovementZone = false;
            }
        }
    }
}