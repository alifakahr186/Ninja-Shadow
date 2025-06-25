using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip clickSFX; 

    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Start()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = menuMusic;
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = 0.7f;
        musicSource.Play();

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.volume = 1f;
    }

    public void OnStoryButtonClicked()
    {
        // Play click sound instantly
        if (clickSFX != null)
            sfxSource.PlayOneShot(clickSFX);

        // Stop music and load next scene after short delay (optional fade buffer)
        musicSource.Stop();
        Invoke("LoadLevelMenu", 0.2f); // delay just enough for SFX to play
    }

    private void LoadLevelMenu()
    {
        SceneManager.LoadScene("LevelMenu");
    }
}
