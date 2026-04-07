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
                // Bitwise AND with NOT removes the specific constraint
                rb.constraints &= ~RigidbodyConstraints.FreezePositionZ;
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
                // Bitwise OR adds the constraint back
                rb.constraints |= RigidbodyConstraints.FreezePositionZ;
            }
        }
    }
}