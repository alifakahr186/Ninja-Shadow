using System.Collections;
using UnityEngine;

public class Jar : MonoBehaviour
{
    public GameObject goldBagPrefab;
    public Transform spawnPoint;

    private ParticleSystem[] particles;
    private SpriteRenderer SR;
    private ParticleSystem smokeParticle;

    private bool hasTriggered = false;

    [SerializeField] private AudioSource breakSound;

    private void Awake()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
        SR = GetComponentInChildren<SpriteRenderer>();
        Transform smokeTransform = transform.Find("Wood Smoke");
        if (smokeTransform != null)
        {
            smokeParticle = smokeTransform.GetComponent<ParticleSystem>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;
        Destroy(collision.gameObject);
        if (collision.CompareTag("Shuriken"))
        {
            hasTriggered = true;
            if (breakSound != null)
            {
                breakSound.Play();
            }
            if (particles.Length > 0)
            {
                //  GoldBag ko turant spawn karo
                if (goldBagPrefab != null)
                {
                    Instantiate(goldBagPrefab, spawnPoint != null ? spawnPoint.position : transform.position, Quaternion.identity);
                }

                StartCoroutine(BreakAfterParticles());
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator BreakAfterParticles()
    {
        foreach (ParticleSystem ps in particles)
        {
            ps.Play();
        }
        if (smokeParticle != null)
        {
            smokeParticle.Play();
        }

        SR.enabled = false;

        float maxDuration = 3f;
        foreach (ParticleSystem ps in particles)
        {
            float lifetime = ps.main.startLifetime.constantMax;
            if (lifetime > maxDuration)
                maxDuration = lifetime;
        }

        if (smokeParticle != null)
        {
            float smokeLifetime = smokeParticle.main.startLifetime.constantMax;
            if (smokeLifetime > maxDuration)
            {
                maxDuration = smokeLifetime;
            }
        }
        yield return new WaitForSeconds(maxDuration);

        Destroy(gameObject);
    }
}
