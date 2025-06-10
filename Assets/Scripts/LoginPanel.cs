using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginPanel : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_InputField playerIdInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button goToRegisterButton;

    private LoginSceneManager sceneManager;

    private void Awake()
    {
        sceneManager = FindObjectOfType<LoginSceneManager>();
    }

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
        goToRegisterButton.onClick.AddListener(OnGoToRegisterClicked);
    }

    private void OnLoginClicked()
    {
        string playerId = playerIdInput.text.Trim();

        if (string.IsNullOrEmpty(playerId))
        {
            Debug.Log("플레이어 ID를 입력해주세요.");
            return;
        }

        if (DataManager.HasPlayerData(playerId))
        {
            sceneManager.Login(playerId);
        }
        else
        {
            Debug.Log("존재하지 않는 플레이어입니다.");
        }
    }

    private void OnGoToRegisterClicked()
    {
        sceneManager.ShowRegisterPanel();
    }
}
