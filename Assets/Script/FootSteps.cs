using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip footstepClip;

    void FootstepSound()
    {
        audioSource.PlayOneShot(footstepClip);
        Debug.Log("Playing..");
    }
}