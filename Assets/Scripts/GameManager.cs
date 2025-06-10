using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerData CurrentPlayer { get; private set; } // Header Á¦°Å

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayer(PlayerData player)
    {
        CurrentPlayer = player;
    }

    public void LoadInventoryScene()
    {
        SceneManager.LoadScene("InventoryScene");
    }

    public void LoadLoginScene()
    {
        SceneManager.LoadScene("LoginScene");
    }

    public void SaveCurrentPlayer()
    {
        if (CurrentPlayer != null)
        {
            DataManager.SavePlayerData(CurrentPlayer);
        }
    }
}