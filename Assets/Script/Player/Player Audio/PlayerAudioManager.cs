using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [Header("")]
    public AudioSource runAudioSource;
    public AudioSource jumpAudioSource;
    public AudioSource dashSound;
    public AudioSource audioSource; // For disguise/revert sounds

    [Header("")]
    public AudioClip disguiseSound;
    public AudioClip revertSound;

    [Header("")]
    public float runMinSpeed = 0.1f;
    public float disguiseVolume = 1f;
    public float revertVolume = 1f;

    private PlayerMovements playerMovements;
    private Rigidbody2D rb;

    private void Start()
    {
        // Auto-find PlayerMovements on same GameObject
        playerMovements = GetComponent<PlayerMovements>();
        if (playerMovements == null)
            playerMovements = FindAnyObjectByType<PlayerMovements>();

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = playerMovements.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleRunningSound();
    }

    public void PlayJumpSound()
    {
        if (jumpAudioSource != null)
            jumpAudioSource.Play();
    }

    public void PlayDashSound()
    {
        if (dashSound != null)
            dashSound.Play();
    }

    public void PlayDisguiseSound()
    {
        if (disguiseSound != null && audioSource != null)
            audioSource.PlayOneShot(disguiseSound, disguiseVolume);
    }

    public void PlayRevertSound()
    {
        if (revertSound != null && audioSource != null)
            audioSource.PlayOneShot(revertSound, revertVolume);
    }

    public void StopRunningSound()
    {
        if (runAudioSource != null && runAudioSource.isPlaying)
            runAudioSource.Stop();
    }


    private void HandleRunningSound()
    {
        if (playerMovements == null || rb == null) return;

        bool shouldPlayRunSound = playerMovements.IsGrounded() &&
                                  Mathf.Abs(rb.linearVelocity.x) > runMinSpeed &&
                                  !playerMovements.IsVanished();

        if (shouldPlayRunSound && !runAudioSource.isPlaying)
            runAudioSource.Play();
        else if (!shouldPlayRunSound && runAudioSource.isPlaying)
            runAudioSource.Stop();
    }

    // Public property getters for Inspector convenience
    public bool IsGrounded() => playerMovements != null && playerMovements.IsGrounded();
    public bool IsVanished() => playerMovements != null && playerMovements.IsVanished();
}