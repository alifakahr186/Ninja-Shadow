using UnityEngine;

public class WoodDisguiseSpikeDetector : MonoBehaviour
{
    private PlayerDisguiseController dummy;

    public void Init(PlayerDisguiseController dummyRef)
    {
        dummy = dummyRef;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Cactus") && dummy != null)
        {
            dummy.OnSpikeTouched();
        }
    }
}
