using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip checkpointSound; // Assign your sound here
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;

        if (collision.CompareTag("Player"))
        {
            GameManager gm = Object.FindAnyObjectByType<GameManager>();
            if (gm != null)
            {
                gm.SetCheckpoint(transform); // Mark this as new respawn point
            }

            // Play sound at this checkpoint position
            if (checkpointSound != null)
            {
                GameObject tempAudio = new GameObject("CheckpointSound");
                tempAudio.transform.position = transform.position;

                AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
                audioSource.clip = checkpointSound;
                audioSource.volume = 1f; //  Set your desired loudness here (1.0 is default)
                audioSource.Play();

                Destroy(tempAudio, checkpointSound.length); // Clean up after playing
            }

            hasTriggered = true; // Prevent retrigger
        }
    }
}
