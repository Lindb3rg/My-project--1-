using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip insideTrack;
    public AudioClip outsideTrack;

    void Start()
    {
        audioSource.clip = outsideTrack;
        audioSource.Play();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.clip = insideTrack;
            audioSource.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.clip = outsideTrack;
            audioSource.Play();
        }
    }
}