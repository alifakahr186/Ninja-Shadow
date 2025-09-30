using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [Header("Level Info")]
    public string levelSceneName;  

    [Header("Star UI")]
    public GameObject[] starIcons;  
    private void Start()
    {
        LoadStars();
    }

    public void LoadStars()
    {
        // Read saved stars from PlayerPrefs
        string levelKey = "Stars_" + levelSceneName;
        int savedStars = PlayerPrefs.GetInt(levelKey, 0);

        for (int i = 0; i < starIcons.Length; i++)
        {
            if (i < savedStars)
                starIcons[i].SetActive(true);   
            else
                starIcons[i].SetActive(false);  
        }
    }
}
