using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashManager : MonoBehaviour
{
    public GameObject playButton;
    [SerializeField] private AudioClip splashMusic;
    [SerializeField] private float musicDuration = 5f;
    private AudioSource audioSource;
    void Start()
    {
        playButton.SetActive(false);
        Invoke("ShowPlayButton", 3.5f);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = splashMusic;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.Play();

        Invoke("StopSplashMusic", musicDuration); // Stop music after fixed time
    }


    void ShowPlayButton()
    {
        playButton.SetActive(true);
    }

    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("MainMenu"); 
    }

    void StopSplashMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
