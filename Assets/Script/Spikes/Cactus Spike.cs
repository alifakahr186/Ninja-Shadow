using UnityEngine;

public class CactusSpike : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitEffect; // Assign in inspector
    [SerializeField] private AudioSource spikeHitSound;

    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float deathVolume = 1f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if collided object i8s player
        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            PlayDeathSound();
            // Instant kill
            playerStats.KillPlayer();
        }

        // Check if collided with a shuriken
        if (other.CompareTag("Shuriken"))
        {
            if (spikeHitSound != null)
            {
                spikeHitSound.Play();
            }
            // Spawn hit effect at shuriken's position
            if (hitEffect != null)
            {
                Instantiate(hitEffect, other.transform.position, Quaternion.identity);
            }


            // Destroy the shuriken
            Destroy(other.gameObject);
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
