using UnityEngine;

public class PlayerIntroEntrance : MonoBehaviour
{
    public Transform stopPoint; // Point jahan ninja rukega
    public float runSpeed = 5f;

    private GameObject player;
    private PlayerMovements playerMovementScript;
    private bool isAutoRunning = true;

    void Start()
    {
        // Find Player and disable movement control initially
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovementScript = player.GetComponent<PlayerMovements>();

        playerMovementScript.enabled = false; // Disable manual control
    }

    void Update()
    {
        if (!isAutoRunning) return;

        // Move ninja toward stopPoint
        player.transform.position = Vector2.MoveTowards(player.transform.position, stopPoint.position, runSpeed * Time.deltaTime);

        // Flip ninja in right direction (optional)
        if (player.transform.position.x < stopPoint.position.x)
            player.transform.localScale = new Vector3(1, 1, 1); // facing right
        else
            player.transform.localScale = new Vector3(-1, 1, 1); // facing left

        // If reached stopPoint, enable manual control
        if (Vector2.Distance(player.transform.position, stopPoint.position) < 0.1f)
        {
            isAutoRunning = false;
            playerMovementScript.enabled = true;
        }
    }
}
