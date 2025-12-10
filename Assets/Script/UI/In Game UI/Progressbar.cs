using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Progressbar : MonoBehaviour
{
    public static Progressbar Instance;  // Singleton for easy access

    [SerializeField] private UnityEngine.UI.Image disguiseProgressBar;
    [SerializeField] private UnityEngine.UI.Image dashProgressBar;

    private Coroutine disguiseCoroutine;
    private Coroutine dashCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persistent across respawns/deaths
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDisguiseProgress(float duration)
    {
        if (disguiseCoroutine != null)
            StopCoroutine(disguiseCoroutine);

        disguiseCoroutine = StartCoroutine(DisguiseProgressRoutine(duration));
    }

    public void StartDashProgress(float duration)
    {
        if (dashCoroutine != null)
            StopCoroutine(dashCoroutine);

        dashCoroutine = StartCoroutine(DashProgressRoutine(duration));
    }

    private IEnumerator DisguiseProgressRoutine(float duration)
    {
        disguiseProgressBar.fillAmount = 1f;
        disguiseProgressBar.gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            disguiseProgressBar.fillAmount = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }
        disguiseProgressBar.fillAmount = 0f;
        disguiseProgressBar.gameObject.SetActive(false);
    }

    private IEnumerator DashProgressRoutine(float duration)
    {
        dashProgressBar.fillAmount = 1f;
        dashProgressBar.gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            dashProgressBar.fillAmount = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }
        dashProgressBar.fillAmount = 0f;
        dashProgressBar.gameObject.SetActive(false);
    }

    // Bonus: Lives UI already yahan pe hai (jaise UIManager.Instance.UpdateLivesUI)
    public void UpdateLivesUI(int lives)
    {
        // Tumhara existing lives UI code yahan
    }
}