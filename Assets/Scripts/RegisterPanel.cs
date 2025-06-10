using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RegisterPanel : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_InputField playerIdInput;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button goToLoginButton;

    private LoginSceneManager sceneManager;

    private void Awake()
    {
        sceneManager = FindObjectOfType<LoginSceneManager>();
    }

    private void Start()
    {
        registerButton.onClick.AddListener(OnRegisterClicked);
        goToLoginButton.onClick.AddListener(OnGoToLoginClicked);
    }

    private void OnRegisterClicked()
    {
        string playerId = playerIdInput.text.Trim();

        if (string.IsNullOrEmpty(playerId))
        {
            Debug.Log("�÷��̾� ID�� �Է����ּ���.");
            return;
        }

        if (DataManager.HasPlayerData(playerId))
        {
            Debug.Log("�̹� �����ϴ� �÷��̾��Դϴ�.");
        }
        else
        {
            sceneManager.Register(playerId);
        }
    }

    private void OnGoToLoginClicked()
    {
        sceneManager.ShowLoginPanel();
    }
}
