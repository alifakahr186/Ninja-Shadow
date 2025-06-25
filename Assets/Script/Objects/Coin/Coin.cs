using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private ParticleSystem[] particles; // All particle systems in children
    private SpriteRenderer SR;
    private bool hasTriggered = false; // Prevent double pickup
    [SerializeField] private AudioSource pickupSound;

    private void Awake()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
        SR = GetComponentInChildren<SpriteRenderer>();
    }

    // This will be called from child trigger collider
    public void TriggerPickup()
    {
        if (hasTriggered) return;
        hasTriggered = true;
        if (pickupSound != null)
        {
            pickupSound.Play();
        }
        if (SR != null)
            SR.enabled = false;

        foreach (ParticleSystem ps in particles)
        {
            ps.Play();
        }
        CoinManager.Instance.AddCoins(1);
        StartCoroutine(BreakAfterParticles());
    }

    private IEnumerator BreakAfterParticles()
    {
        float maxDuration = 3f;
        foreach (ParticleSystem ps in particles)
        {
            float lifetime = ps.main.startLifetime.constantMax;
            if (lifetime > maxDuration)
                maxDuration = lifetime;
        }

        yield return new WaitForSeconds(maxDuration);
        Destroy(gameObject);
    }
}
