using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private AudioClip loadingMusic;
    private AudioSource audioSource;
    void Start()
    {
        // Add AudioSource and configure
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = loadingMusic;
        audioSource.playOnAwake = false;
        audioSource.loop = false; // Optional: set true if the music is short and may loop
        audioSource.volume = 0.7f;
        audioSource.Play();

        StartCoroutine(LoadLevelAsync());
    }
    IEnumerator LoadLevelAsync()
    {
        yield return new WaitForSeconds(3f);

        string sceneToLoad = SceneLoader.targetScene;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            yield break;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneToLoad))
        {
            yield break;
        }

        if (audioSource.isPlaying)
            audioSource.Stop();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

    }

}
