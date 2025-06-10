using UnityEngine;

public class LoginSceneManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private LoginPanel loginPanel;
    [SerializeField] private RegisterPanel registerPanel;

    private void Start()
    {
        ShowLoginPanel();
    }

    public void ShowLoginPanel()
    {
        loginPanel.gameObject.SetActive(true);
        registerPanel.gameObject.SetActive(false);
    }

    public void ShowRegisterPanel()
    {
        loginPanel.gameObject.SetActive(false);
        registerPanel.gameObject.SetActive(true);
    }

    public void Login(string playerId)
    {
        PlayerData playerData = DataManager.LoadPlayerData(playerId);
        if (playerData != null)
        {
            GameManager.Instance.SetPlayer(playerData);
            GameManager.Instance.LoadInventoryScene();
        }
        else
        {
            Debug.Log("No player data found!");
        }
    }

    public void Register(string playerId)
    {
        if (!DataManager.HasPlayerData(playerId))
        {
            PlayerData newPlayer = new PlayerData(playerId);
            DataManager.SavePlayerData(newPlayer);

            Debug.Log($"'{playerId}' 계정이 생성되었습니다! 로그인해주세요.");

            // 가입 완료 후 로그인 패널로 이동
            ShowLoginPanel();
        }
        else
        {
            Debug.Log("Player already exists!");
        }
    }
}