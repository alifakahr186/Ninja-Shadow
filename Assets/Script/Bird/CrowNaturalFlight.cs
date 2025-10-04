using UnityEngine;
using System.Collections;

public class CrowNaturalFlight : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Flight Settings")]
    public float minSpeed = 1.5f;
    public float maxSpeed = 3.5f;
    public float waypointDistance = 0.2f;

    [Header("Natural Motion")]
    public float verticalAmplitude = 0.5f;
    public float verticalFrequency = 2f;
    public float circularChance = 0.3f;

    [Header("Randomization")]
    public float randomDirectionTime = 3f;

    [Header("Crow Sound")]
    public float soundDuration = 2f;    // Crow speaks 2 sec
    public float silentDuration = 4f;   // Silent 4 sec
    public float fadeOutDuration = 1f;  // Smooth fade out time

    private Transform targetPoint;
    private float speed;
    private float timeOffset;
    private bool circularMode = false;
    private float nextRandomChange;

    private AudioSource crowAudio;
    private float soundTimer;
    private bool isSounding;
    private bool isPlayerInZone = false;
    private bool isPlayingSound = false; // NEW: Track if sound is playing

    void Start()
    {
        targetPoint = pointB;
        RollNewBehavior();

        crowAudio = GetComponent<AudioSource>();
        if (crowAudio != null)
        {
            crowAudio.enabled = false;
        }
        else
        {
            Debug.LogError("CrowAudio is null! Add AudioSource to Crow!");
        }

        isSounding = false;
        soundTimer = silentDuration;
    }

    void Update()
    {
        // Movement
        if (Time.time > nextRandomChange)
        {
            RollNewBehavior();
        }

        if (circularMode)
            FlyInCircularMotion();
        else
            FlyWithNaturalSway();

        // Sound only if player is in zone
        if (isPlayerInZone)
        {
            HandleCrowSound();
        }
    }

    public void SetPlayerInZone(bool inZone)
    {
        isPlayerInZone = inZone;
        if (inZone && !isSounding && !isPlayingSound)
        {
            if (crowAudio != null)
            {
                crowAudio.enabled = true;
                crowAudio.volume = 1f;
                crowAudio.Play();
                isSounding = true;
                isPlayingSound = true;
                soundTimer = soundDuration;
                StartCoroutine(EnsureMinPlayDuration(soundDuration));
                Debug.Log("Crow sound started!");
            }
            else
            {
                Debug.LogError("CrowAudio is null! Add AudioSource to Crow!");
            }
        }
        else if (!inZone && isSounding)
        {
            StartCoroutine(DelayedFadeOut());
        }
    }

    void HandleCrowSound()
    {
        soundTimer -= Time.deltaTime;

        if (soundTimer <= 0)
        {
            if (isSounding)
            {
                StartCoroutine(FadeOut(crowAudio, fadeOutDuration));
                isSounding = false;
                soundTimer = silentDuration;
            }
            else if (isPlayerInZone)
            {
                if (crowAudio != null && !isPlayingSound)
                {
                    crowAudio.volume = 1f;
                    crowAudio.Play();
                    isSounding = true;
                    isPlayingSound = true;
                    soundTimer = soundDuration;
                    StartCoroutine(EnsureMinPlayDuration(soundDuration));
                    Debug.Log("Crow sound started!");
                }
            }
        }
    }

    IEnumerator EnsureMinPlayDuration(float minDuration)
    {
        yield return new WaitForSeconds(minDuration);
        // Allow stopping only after min duration
        // isPlayingSound will be reset in FadeOut
    }

    IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        if (audioSource == null) yield break;

        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.enabled = false;
        audioSource.volume = startVolume;
        isPlayingSound = false;
        Debug.Log("Crow sound faded out.");
    }

    IEnumerator DelayedFadeOut()
    {
        // Wait for current sound cycle to complete if playing
        if (soundTimer > 0)
        {
            yield return new WaitForSeconds(soundTimer);
        }

        if (isSounding && crowAudio != null)
        {
            StartCoroutine(FadeOut(crowAudio, fadeOutDuration));
            isSounding = false;
            soundTimer = silentDuration;
        }
    }

    void FlyWithNaturalSway()
    {
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        Vector3 move = direction * speed * Time.deltaTime;

        move.y += Mathf.Sin((Time.time + timeOffset) * verticalFrequency) * verticalAmplitude * Time.deltaTime;

        transform.position += move;

        if (Vector3.Distance(transform.position, targetPoint.position) < waypointDistance)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
            Flip(targetPoint == pointB);
        }
    }

    void FlyInCircularMotion()
    {
        Vector3 center = (pointA.position + pointB.position) / 2;
        float radius = Vector3.Distance(pointA.position, pointB.position) / 2;

        transform.RotateAround(center, Vector3.forward, speed * 10f * Time.deltaTime);

        Vector3 dir = (center - transform.position).normalized;
        if (dir.x > 0) Flip(true); else Flip(false);
    }

    void RollNewBehavior()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        timeOffset = Random.Range(0f, 100f);
        circularMode = (Random.value < circularChance);
        nextRandomChange = Time.time + randomDirectionTime;
    }

    void Flip(bool facingRight)
    {
        if (facingRight)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}