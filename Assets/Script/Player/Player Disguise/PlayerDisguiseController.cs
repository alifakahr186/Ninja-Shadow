using UnityEngine;

public class PlayerDisguiseController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed = 2f;
    private bool isFacingRight = true;
    private PlayerMovements ninjaOwner;

    public void AssignOwner(PlayerMovements owner)
    {
        ninjaOwner = owner;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float input = 0f;

        // Prioritize UI buttons if owner is assigned
        if (ninjaOwner != null)
        {
            if (ninjaOwner.IsUIMoveLeftPressed()) input = -1f;
            else if (ninjaOwner.IsUIMoveRightPressed()) input = 1f;
        }

        // Fallback to keyboard input
        if (input == 0f)
        {
            input = Input.GetAxisRaw("Horizontal");
        }

        rb.linearVelocity = new Vector2(input * moveSpeed, rb.linearVelocity.y);

        if (input > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (input < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void OnSpikeTouched()
    {
        if (ninjaOwner != null)
        {
            ninjaOwner.KillDueToDummyDeath();
        }

        Destroy(gameObject); // Destroy dummy
    }
}
