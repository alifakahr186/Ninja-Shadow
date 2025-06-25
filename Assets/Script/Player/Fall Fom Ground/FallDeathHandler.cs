using UnityEngine;

public class FallDeathHandler : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerLivesManager livesManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        livesManager = FindAnyObjectByType<PlayerLivesManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Step 1: Call death logic so deathCount updates
            if (livesManager != null)
            {
                livesManager.PlayerDied();
            }

            // Step 2: Disable camera follow
            if (livesManager.virtualCamera != null)
            {
                livesManager.virtualCamera.Follow = null;
            }

            // Step 3: Destroy the old player object
            Destroy(collision.gameObject);

            // Step 4: Respawn logic
            if (gameManager != null)
            {
                gameManager.Respawn();
            }
        }
    }
}
