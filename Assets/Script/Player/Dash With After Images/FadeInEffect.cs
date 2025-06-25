using System.Collections;
using UnityEngine;

public class FadeInEffect : MonoBehaviour
{
    public float fadeDuration = 2f;
    private SpriteRenderer[] renderers;
    private float timer;

    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        SetAlpha(0f); // Make invisible at start
    }

    private void Start()
    {
        timer = 0f;
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        while (timer < fadeDuration)
        {
            float alpha = timer / fadeDuration;
            SetAlpha(alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        SetAlpha(1f); // Fully visible at the end
        Destroy(this); // Optional: remove the script once done
    }

    private void SetAlpha(float alpha)
    {
        foreach (var r in renderers)
        {
            if (r != null)
            {
                Color c = r.color;
                c.a = alpha;
                r.color = c;
            }
        }
    }
}
