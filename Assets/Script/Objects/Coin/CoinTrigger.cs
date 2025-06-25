using UnityEngine;

public class CoinTrigger : MonoBehaviour
{
    private Coin parentCoin;

    private void Awake()
    {
        parentCoin = GetComponentInParent<Coin>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && parentCoin != null)
        {
            parentCoin.TriggerPickup();
        }
    }
}
