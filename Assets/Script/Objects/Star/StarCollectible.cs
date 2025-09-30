using UnityEngine;

public class StarCollectible : MonoBehaviour
{
    public GameObject uiStarImage;
    [SerializeField] private AudioSource starCollectSound;

    private SpriteRenderer spriteRenderer;
    private Collider2D starCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        starCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Disable visual + collision immediately
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            if (starCollider != null) starCollider.enabled = false;

            if (starCollectSound != null && starCollectSound.clip != null)
            {
                starCollectSound.Play();
            }

            if (uiStarImage != null)
            {
                uiStarImage.SetActive(true);
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.CollectStar();
            }

            Destroy(gameObject, starCollectSound.clip.length);
        }
    }
}
