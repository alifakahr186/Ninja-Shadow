using UnityEngine;

public class StoneGateParticleTrigger : MonoBehaviour
{
    private ParticleSystem particleSystema;
    private bool hasTriggered = false; 

    private void Start()
    {
        particleSystema = GetComponentInChildren<ParticleSystem>();

        if (particleSystema != null)
        {
            particleSystema.Stop();
        }
        else
        {
            Debug.LogWarning("Particle System not found in child of " + gameObject.name);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true; 
            if (particleSystema != null)
                particleSystema.Play();
        }
    }

}
