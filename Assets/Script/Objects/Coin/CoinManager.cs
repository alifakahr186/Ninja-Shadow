using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    private int totalCoins = 0;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // So it doesn't get destroyed between scenes
            LoadCoins();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        SaveCoins();
        UIManager.Instance.UpdateCoinUI(totalCoins);
    }

    public int GetCoins() => totalCoins;

    private void SaveCoins()
    {
        PlayerPrefs.SetInt("CoinCount", totalCoins);
    }

    private void LoadCoins()
    {
        totalCoins = PlayerPrefs.GetInt("CoinCount", 0);
    }

    public void ResetCoins()
    {
        totalCoins = 0;
        SaveCoins();
        UIManager.Instance.UpdateCoinUI(totalCoins);
    }
}
