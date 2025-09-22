using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static string targetScene;
    public static bool playEntryAnimation;

    public static void LoadScene(string sceneName)
    {
        targetScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }
}
