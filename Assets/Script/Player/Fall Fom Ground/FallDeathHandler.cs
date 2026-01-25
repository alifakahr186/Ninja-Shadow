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
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            MonoBehaviour[] scripts = collision.GetComponents<MonoBehaviour>();

            // Step 1: Update death count
            if (livesManager != null)
            {
                livesManager.PlayerDied();
            }

            // Step 2: Disable camera follow
            if (livesManager.virtualCamera != null)
            {
                livesManager.virtualCamera.Follow = null;
            }

            // Step 3: Disable player control (IMPORTANT)
            foreach (var script in scripts)
            {
                script.enabled = false;
            }

            // Step 4: Enable natural fall
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.gravityScale = 4f; // tweak value if needed
                rb.constraints = RigidbodyConstraints2D.None;
            }

            // Step 5: Respawn after short delay
            if (gameManager != null)
            {
                Invoke(nameof(RespawnPlayer), 1.2f);
            }
        }
    }

    void RespawnPlayer()
    {
        gameManager.Respawn();
    }
}
