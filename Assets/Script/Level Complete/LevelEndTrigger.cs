using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.Cinemachine;

public class LevelEndTrigger : MonoBehaviour
{
    public GameObject player;
    public GameObject playerUI;
    public Camera mainCamera;
    public GameObject blackScreenImage;
    public GameObject levelCompletePanel;
    public CinemachineCamera cinemachineCamera;
    public AudioClip levelCompleteSound;
    public float levelCompleteVolume = 1.2f;


    public float autoRunSpeed = 5f;
    public float delayBeforeBlackout = 2f;

    private bool levelEnded = false;
    private Vector3 cameraStopPosition;
    public AudioSource[] audioSourcesToMute;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered by: " + other.name); //  Add this

        if (levelEnded) return;

        if (other.CompareTag("Player"))
        {
            if (levelCompleteSound != null)
            {
                GameObject tempSound = new GameObject("LevelCompleteSound");
                tempSound.transform.position = transform.position;

                AudioSource source = tempSound.AddComponent<AudioSource>();
                source.clip = levelCompleteSound;
                source.volume = levelCompleteVolume;
                source.Play();

                Destroy(tempSound, levelCompleteSound.length);
            }
            // Store camera stop position
            cameraStopPosition = mainCamera.transform.position;

            if (cinemachineCamera != null)
            {
                cinemachineCamera.Follow = null;
            }
            // Disable UI buttons
            if (playerUI != null) playerUI.SetActive(false);

            // Stop camera movement
            StartCoroutine(StopCameraAndAutoRun());

            // Make ninja auto-run
            PlayerMovements pm = player.GetComponent<PlayerMovements>();
            if (pm != null)
            {
                pm.enabled = false; // Disable normal controls
                StartCoroutine(AutoRunPlayer(pm));
            }
        }
    }

    private IEnumerator StopCameraAndAutoRun()
    {
        // Lock camera at current position
        while (true)
        {
            mainCamera.transform.position = cameraStopPosition;

            yield return null;
        }
    }

    private IEnumerator AutoRunPlayer(PlayerMovements pm)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Animator anim = player.GetComponent<Animator>();

        if (anim != null)
            anim.SetBool("isRunning", true); // Optional: set animator state

        rb.linearVelocity = new Vector2(autoRunSpeed, rb.linearVelocity.y);

        yield return new WaitForSeconds(delayBeforeBlackout);

        Image blackImageComponent = blackScreenImage.GetComponent<Image>();
        if (blackImageComponent != null)
        {
            blackScreenImage.SetActive(true);
            StartCoroutine(FadeInBlackScreen(blackImageComponent, 1.5f));
        }

        yield return new WaitForSeconds(2f); // Wait before showing panel

        levelCompletePanel.SetActive(true);
        foreach (AudioSource source in audioSourcesToMute)
        {
            if (source != null)
            {
                source.volume = 0f;
                source.mute = true;
            }
        }
    }

    private IEnumerator FadeInBlackScreen(Image blackImage, float duration)
    {
        Color color = blackImage.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            blackImage.color = new Color(color.r, color.g, color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        blackImage.color = new Color(color.r, color.g, color.b, 1f); // Ensure full alpha at the end
    }

}