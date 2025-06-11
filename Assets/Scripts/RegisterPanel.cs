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
        if (sceneManager == null)
        {
            Debug.LogError("LoginSceneManager�� ã�� �� �����ϴ�!");
        }
    }

    private void Start()
    {
        SetupButtons();
        SetupInputField();
    }

    private void SetupButtons()
    {
        if (registerButton != null)
        {
            registerButton.onClick.AddListener(OnRegisterClicked);
        }

        if (goToLoginButton != null)
        {
            goToLoginButton.onClick.AddListener(OnGoToLoginClicked);
        }
    }

    private void SetupInputField()
    {
        if (playerIdInput != null)
        {
            // ����Ű�� ȸ������
            playerIdInput.onSubmit.AddListener((value) => OnRegisterClicked());
        }
    }

    private void OnRegisterClicked()
    {
        if (sceneManager == null)
        {
            Debug.LogError("LoginSceneManager�� �����ϴ�!");
            return;
        }

        string playerId = playerIdInput.text.Trim();

        if (string.IsNullOrEmpty(playerId))
        {
            Debug.Log("�÷��̾� ID�� �Է����ּ���.");
            return;
        }

        if (playerId.Length < 2)
        {
            Debug.Log("�÷��̾� ID�� 2���� �̻��̾�� �մϴ�.");
            return;
        }

        if (playerId.Length > 20)
        {
            Debug.Log("�÷��̾� ID�� 20���� ���Ͽ��� �մϴ�.");
            return;
        }

        // PlayerDataManager�� ����Ͽ� ������ ���� ���� Ȯ�� �� ���
        if (PlayerDataManager.HasPlayerData(playerId))
        {
            Debug.Log("�̹� �����ϴ� �÷��̾��Դϴ�. �ٸ� ID�� ������ּ���.");
        }
        else
        {
            sceneManager.Register(playerId);
        }
    }

    private void OnGoToLoginClicked()
    {
        if (sceneManager != null)
        {
            sceneManager.ShowLoginPanel();
        }
    }

    private void OnEnable()
    {
        // �г��� Ȱ��ȭ�� �� �Է� �ʵ� ��Ŀ��
        if (playerIdInput != null)
        {
            playerIdInput.text = "";
            playerIdInput.Select();
            playerIdInput.ActivateInputField();
        }
    }
}