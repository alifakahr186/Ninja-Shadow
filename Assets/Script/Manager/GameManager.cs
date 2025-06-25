using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private GameObject player;
    [SerializeField] private float respawnTime;

    [SerializeField] private GameObject respawnParticleEffect; 
    [SerializeField] private GameObject respawnParticleEffect2; 

    [SerializeField] private AudioSource respawnSound;
    private float respawnTimeStart;
    private bool respawn;

    private CinemachineCamera CVC;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        CVC = GameObject.Find("PlayerCamera").GetComponent<CinemachineCamera>();
        respawnPoint = startPoint;
    }

    private void Update()
    {
        CheckRespawn();
    }

    public void Respawn()
    {
        respawnTimeStart = Time.time;
        respawn = true;
        // Hide UI immediately on death
        UIManager.Instance.HidePlayerUI();
    }

    private void CheckRespawn()
    {
        if (Time.time >= respawnTimeStart + respawnTime && respawn)
        {
            //Spawn player
            var playerTemp = Instantiate(player, respawnPoint.position, Quaternion.identity);

            // Spawn particle effect
            if (respawnParticleEffect != null)
            {
                Instantiate(respawnParticleEffect, respawnPoint.position, Quaternion.identity);
            }
            if (respawnParticleEffect2 != null)
            {
                Instantiate(respawnParticleEffect2, respawnPoint.position, Quaternion.identity);
            }

            if (respawnSound != null)
            {
                GameObject tempAudio = new GameObject("RespawnSound");
                tempAudio.transform.position = respawnPoint.position;

                AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
                tempSource.clip = respawnSound.clip;
                tempSource.volume = respawnSound.volume;
                tempSource.spatialBlend = 0f; // Set to 0 for 2D sound, 1 for 3D
                tempSource.Play();

                Destroy(tempAudio, respawnSound.clip.length);
            }

            //  Update camera
            CVC.Follow = playerTemp.transform;

            respawn = false;
            StartCoroutine(ShowPlayerUIAfterDelay());

            // Notify all enemies
            BasicEnemyController[] enemies = Object.FindObjectsByType<BasicEnemyController>(FindObjectsSortMode.None);
            foreach (var enemy in enemies)
            {
                enemy.OnPlayerRespawned();
            }
        }
    }

    private IEnumerator ShowPlayerUIAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        UIManager.Instance.ShowPlayerUI();
    }

    public void SetCheckpoint(Transform newCheckpoint)
    {
        respawnPoint = newCheckpoint;
    }

    public Transform GetStartPoint()
    {
        return startPoint;
    }
}
