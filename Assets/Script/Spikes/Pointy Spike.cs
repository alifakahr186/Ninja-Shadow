using UnityEngine;

public class PointySpike : MonoBehaviour
{
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float deathVolume = 1f;
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if collided object has PlayerStats
        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            PlayDeathSound();
            // Deal full damage to kill instantly
            playerStats.KillPlayer(); // assuming maxHealth is less than this
        }
    }

    private void PlayDeathSound()
    {
        if (deathSound != null)
        {
            GameObject temp = new GameObject("TempDeathSound");
            AudioSource source = temp.AddComponent<AudioSource>();
            source.clip = deathSound;
            source.volume = deathVolume;
            source.Play();
            Destroy(temp, deathSound.length); // Auto-cleanup
        }
    }
}
