using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 2f;
    public float damage = 1f;
    public LayerMask hitLayers;
    [SerializeField] private GameObject wallHitDebrisPrefab;
    [SerializeField] private GameObject bloodParticlePrefab;
 
    private float[] attackDetails = new float[2];


    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & hitLayers) != 0)
        {
            // 1. Hit point detection
            Vector2 hitPoint = collision.ClosestPoint(transform.position);

            if (collision.CompareTag("Enemy"))
            {
                if (bloodParticlePrefab != null)
                    Instantiate(bloodParticlePrefab, hitPoint, Quaternion.identity);
                //  Send damage to enemy
                attackDetails[0] = damage;
                attackDetails[1] = transform.position.x;
                collision.transform.parent?.SendMessage("Damage", attackDetails, SendMessageOptions.DontRequireReceiver);
            }
            else // wall or other stuff
            {
                if (wallHitDebrisPrefab != null)
                    Instantiate(wallHitDebrisPrefab, hitPoint, Quaternion.identity);
            }


            // 3. Destroy shuriken
            Destroy(gameObject);
        }
    }
}
