using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private GameObject NinjaRagdoll;
    [SerializeField] private GameObject deathChunkParticles, deathBloodParticles;


    private GameManager GM;

    private void Start()
    {
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void KillPlayer()
    {
        if (deathChunkParticles != null)
            Instantiate(deathChunkParticles, transform.position, deathChunkParticles.transform.rotation);

        if (deathBloodParticles != null)
            Instantiate(deathBloodParticles, transform.position, deathBloodParticles.transform.rotation);

        if (NinjaRagdoll != null)
        {
            GameObject ragdoll = Instantiate(NinjaRagdoll, transform.position, transform.rotation);
            Destroy(ragdoll, 4f);
        }

        FindAnyObjectByType<PlayerLivesManager>().PlayerDied();
        GM.Respawn();

        Destroy(gameObject);
    }
}
