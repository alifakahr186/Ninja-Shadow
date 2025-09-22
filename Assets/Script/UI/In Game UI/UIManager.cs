using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("UI Fade-In Control")]
    public CanvasGroup uiCanvasGroup;
    public float fadeInDelay = 2f;
    public float fadeInDuration = 1f;

    public GameObject levelCompletePanel;

    [Header("Panels & Music")]
    public GameObject settingsPanel;
    public AudioSource musicSource;
    public GameObject musicOnButton;
    public GameObject musicOffButton;

    [Header("UI References")]
    public GameObject playerUI;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI livesText;

    [Header("Star UI")]
    public Image[] starIcons; 
    private int currentStarIndex = 0;

    [Header("Scene Info")]
    public string mainMenuSceneName = "MainMenu";

    public static UIManager Instance;

    private bool isPaused = false;
    private bool isMusicOn = true;
    private bool isUIHidden = false;
    private bool waitingForTouchToShowUI = false; 
    private float touchBlockTimer = 0f;

    [Header("Setting Sound Effects")]
    [SerializeField] private AudioClip clickSFX;
    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }
    private void Start()
    {
        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = 0;
            uiCanvasGroup.interactable = false;
            uiCanvasGroup.blocksRaycasts = false;
            StartCoroutine(FadeInUI());
        }
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.volume = 1f; 


    }
    private void Update()
    {
        if (touchBlockTimer > 0f)
        {
            touchBlockTimer -= Time.unscaledDeltaTime;
            return; // Ignore touch while blocking
        }

        if (waitingForTouchToShowUI && !isPaused && isUIHidden)
        {
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
            {
                ShowUI();
            }
        }
    }
    private IEnumerator FadeInUI()
    {
        yield return new WaitForSeconds(fadeInDelay);

        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            float alpha = Mathf.Clamp01(elapsedTime / fadeInDuration);
            uiCanvasGroup.alpha = alpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        uiCanvasGroup.alpha = 1f;
        uiCanvasGroup.interactable = true;
        uiCanvasGroup.blocksRaycasts = true;

    }

    public void UpdateCoinUI(int count)
    {
        coinText.text = count.ToString();
    }

    public void UpdateLivesUI(int lives)
    {
        livesText.text = lives.ToString();
    }

    public void CollectStar()
    {
        if (currentStarIndex < starIcons.Length)
        {
            starIcons[currentStarIndex].enabled = true;
            currentStarIndex++;
        }
    }

    public void ResetStars()
    {
        currentStarIndex = 0;
        foreach (Image star in starIcons)
        {
            star.enabled = false;
        }
    }

    public void ShowLevelCompleteUI()
    {
        Time.timeScale = 0f;
        levelCompletePanel.SetActive(true);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void OpenSettings()
    {
        if (clickSFX != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clickSFX);
        }

        settingsPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }


    public void ResumeGame()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        if (isUIHidden)
        {
            waitingForTouchToShowUI = true;
            touchBlockTimer = 0.2f; // Block touch input for 0.2 seconds
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        ResetStars();

        //  Reset to startPoint before reload
        if (GameManager.Instance != null)
        {
            Transform start = GameManager.Instance.GetStartPoint();
            GameManager.Instance.SetCheckpoint(start);
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneLoader.playEntryAnimation = true;

        SceneLoader.targetScene = currentSceneName;

        SceneManager.LoadScene("LoadingScene");
    }


    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        musicSource.mute = !isMusicOn;
        musicOnButton.SetActive(isMusicOn);
        musicOffButton.SetActive(!isMusicOn);
    }

    public void ToggleUI()
    {
        isUIHidden = !isUIHidden;
        playerUI.SetActive(!isUIHidden);
        waitingForTouchToShowUI = false; 
    }

    public void ShowUI()
    {
        isUIHidden = false;
        playerUI.SetActive(true);
        waitingForTouchToShowUI = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void HidePlayerUI()
    {
        playerUI.SetActive(false);
    }

    public void ShowPlayerUI()
    {
        playerUI.SetActive(true);
    }
}