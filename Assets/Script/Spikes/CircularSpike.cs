using UnityEngine;

public class CircularSpike : MonoBehaviour
{
    public float rotationSpeed = 200f;
    public float moveSpeed = 2f;
    public float moveDistance = 3f;

    private Vector3 startPos;
    private bool movingRight = true;
    private Rigidbody2D rb;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float deathVolume = 1f;

    [SerializeField] private AudioClip movementSound;
[SerializeField] private float movementVolume = 0.6f;
    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() // Physics update ke liye FixedUpdate use karo
    {
        // Apne axis par rotation
        transform.Rotate(Vector3.forward * rotationSpeed * Time.fixedDeltaTime);

        // MoveDirection
        Vector2 moveDir = movingRight ? Vector2.right : Vector2.left;
        Vector2 newPos = rb.position + moveDir * moveSpeed * Time.fixedDeltaTime;

        // Wall Collision ke bagair move karo
        rb.MovePosition(newPos);

        // Check distance and flip direction
        if (movingRight && rb.position.x >= startPos.x + moveDistance)
            movingRight = false;
        else if (!movingRight && rb.position.x <= startPos.x - moveDistance)
            movingRight = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            PlayDeathSound();
            playerStats.KillPlayer();
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
