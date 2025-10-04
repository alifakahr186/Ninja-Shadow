using UnityEngine;

public class CrowZone : MonoBehaviour
{
    public CrowNaturalFlight crow; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (crow != null)
            {
                crow.SetPlayerInZone(true);
                Debug.Log("Player entered CrowZone!");
            }
            else
            {
                Debug.LogError("Crow reference is null in CrowZone!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (crow != null)
            {
                crow.SetPlayerInZone(false);
                Debug.Log("Player exited CrowZone!");
            }
        }
    }
}