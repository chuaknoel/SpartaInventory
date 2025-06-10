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
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginClicked);
        }

        if (goToRegisterButton != null)
        {
            goToRegisterButton.onClick.AddListener(OnGoToRegisterClicked);
        }
    }

    private void SetupInputField()
    {
        if (playerIdInput != null)
        {
            // ����Ű�� �α���
            playerIdInput.onSubmit.AddListener((value) => OnLoginClicked());
        }
    }

    private void OnLoginClicked()
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

        // ������ ���� ���� Ȯ�� �� �α���
        if (DataManager.HasPlayerData(playerId))
        {
            sceneManager.Login(playerId);
        }
        else
        {
            Debug.Log("�������� �ʴ� �÷��̾��Դϴ�. ȸ�������� ���ּ���.");
        }
    }

    private void OnGoToRegisterClicked()
    {
        if (sceneManager != null)
        {
            sceneManager.ShowRegisterPanel();
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