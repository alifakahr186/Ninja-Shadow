using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenuUI : MonoBehaviour
{
    [SerializeField] private AudioClip clickSFX;
    private AudioSource sfxSource;
    public string mainMenuSceneName = "MainMenu";

    void Start()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.volume = 1f;

    }

    public void LoadLevel1()
    {
        PlayClickSFX();
        SceneLoader.targetScene = "Scenes/Level 1";
        SceneManager.LoadScene("LoadingScene");
    }

    public void LoadLevel2()
    {
        PlayClickSFX();
        SceneLoader.targetScene = "Scenes/Level 2";
        SceneManager.LoadScene("LoadingScene");
    }

    public void GoToMainMenu()
    {
        PlayClickSFX();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void PlayClickSFX()
    {
        if (clickSFX != null)
            sfxSource.PlayOneShot(clickSFX);
    }
}
