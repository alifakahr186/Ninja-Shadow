using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}
