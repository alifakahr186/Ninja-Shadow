using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerLivesManager : MonoBehaviour
{
    public int totalLives = 3;
    public GameObject player;
    public GameObject deathUI;
    public GameObject playerUI;
    public GameObject settingUI;
    public CinemachineCamera virtualCamera;

    private int deathCount = 0;
    private Vector3 respawnPoint;
    private float deathUIDelay = 3f; 

    private void Start()
    {
        respawnPoint = player.transform.position;
        deathUI.SetActive(false);
        UIManager.Instance.UpdateLivesUI(totalLives - deathCount);

        if(settingUI != null)
        {
            settingUI.SetActive(true);
        }
        if (playerUI != null)
        {
            playerUI.SetActive(true);
        }

    }

    public void UpdateRespawnPoint(Vector3 newPoint)
    {
        respawnPoint = newPoint;
    }

    public void PlayerDied()
    {
        deathCount++;
        UIManager.Instance.UpdateLivesUI(totalLives - deathCount);

        if (deathCount < totalLives)
        {
            player.transform.position = respawnPoint;
            player.SetActive(true);
        }
        else
        {
            // Game Over: Disable player, hide UI, and delay death UI
            player.SetActive(false);

            if (playerUI != null)
            {
                playerUI.SetActive(false);
            }
            if(settingUI != null)
            {
                settingUI.SetActive(false);
            }


            StartCoroutine(ShowDeathUIWithDelay());
        }
    }

    private IEnumerator ShowDeathUIWithDelay()
    {
        yield return new WaitForSeconds(deathUIDelay);

        deathUI.SetActive(true);
    }

    public void RestartGame()
    {
        deathCount = 0;
        UIManager.Instance.UpdateLivesUI(totalLives);

        if(settingUI != null)
        {
            settingUI.SetActive(true);
        }
        if (playerUI != null)
        {
            playerUI.SetActive(true);
        }
            
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
