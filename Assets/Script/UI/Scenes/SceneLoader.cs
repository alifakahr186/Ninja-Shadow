using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static string targetScene;

    public static void LoadScene(string sceneName)
    {
        targetScene = sceneName;
        SceneManager.LoadScene("Level 1");
    }
}
