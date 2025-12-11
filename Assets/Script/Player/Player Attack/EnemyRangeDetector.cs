using UnityEngine;

public class EnemyRangeDetector : MonoBehaviour
{
    public bool isEnemyInRange = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // Make sure your enemies are tagged as "Enemy"
        {
            isEnemyInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            isEnemyInRange = false;
        }
    }
}
