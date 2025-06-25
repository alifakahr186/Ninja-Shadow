using UnityEngine;

public class StarCollectible : MonoBehaviour
{
    public GameObject collectEffect; // Optional particle effect
    public GameObject uiStarImage;   // Assign UI Image (top-left star slot)


    private bool collected = false;
    [SerializeField] private AudioSource starCollectSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected) return;

        if (collision.CompareTag("Player"))
        {
            collected = true;
            if (starCollectSound != null)
            {
                starCollectSound.Play();
            }
            // Spawn FX
            if (collectEffect != null)
            {
                GameObject effect = Instantiate(collectEffect, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }

            // Show UI version
            if (uiStarImage != null)
                uiStarImage.SetActive(true);

            // Kill the star
            Destroy(gameObject);
        }
    }
}
