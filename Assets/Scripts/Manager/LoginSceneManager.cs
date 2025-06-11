using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginSceneManager : MonoBehaviour
{
    [Header("=== LOGIN SCENE MANAGER ===")]

    [Header("Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;

    [Header("Login Panel UI")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button goToRegisterButton;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Test Buttons")]
    [SerializeField] private Button testButton1;
    [SerializeField] private Button testButton2;
    [SerializeField] private Button testButton3;

    private void Start()
    {
        SetupUI();
        SetupButtons();
        ShowLoginPanel(); // ���۽� �α��� �г� ǥ��
    }

    private void SetupUI()
    {
        // �⺻ ���� �ؽ�Ʈ
        if (statusText != null)
        {
            statusText.text = "�÷��̾� �̸��� �Է��ϰ� �α����ϼ���.";
            statusText.color = Color.white;
        }

        // �Է� �ʵ� ��Ŀ��
        if (playerNameInput != null)
        {
            playerNameInput.text = "";
            playerNameInput.Select();
            playerNameInput.ActivateInputField();
        }
    }

    private void SetupButtons()
    {
        // �α��� ��ư
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginButtonClicked);
        }

        // ȸ���������� �̵� ��ư
        if (goToRegisterButton != null)
        {
            goToRegisterButton.onClick.AddListener(ShowRegisterPanel);
        }

        // �׽�Ʈ ��ư��
        if (testButton1 != null)
        {
            testButton1.onClick.AddListener(() => CreateTestPlayer("�׽�Ʈ�÷��̾�1"));
        }

        if (testButton2 != null)
        {
            testButton2.onClick.AddListener(() => CreateTestPlayer("�׽�Ʈ�÷��̾�2"));
        }

        if (testButton3 != null)
        {
            testButton3.onClick.AddListener(() => CreateTestPlayer("�׽�Ʈ�÷��̾�3"));
        }

        // ����Ű�� �α���
        if (playerNameInput != null)
        {
            playerNameInput.onSubmit.AddListener((value) => OnLoginButtonClicked());
        }
    }

    #region Panel Management

    /// <summary>
    /// �α��� �г� ǥ��
    /// </summary>
    public void ShowLoginPanel()
    {
        if (loginPanel != null) loginPanel.SetActive(true);
        if (registerPanel != null) registerPanel.SetActive(false);

        Debug.Log("�α��� �г� ǥ��");
    }

    /// <summary>
    /// ȸ������ �г� ǥ��
    /// </summary>
    public void ShowRegisterPanel()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        if (registerPanel != null) registerPanel.SetActive(true);

        Debug.Log("ȸ������ �г� ǥ��");
    }

    #endregion

    #region Login System (���ο� ���)

    /// <summary>
    /// ���� �÷��̾�� �α���
    /// </summary>
    public void Login(string playerId)
    {
        ShowStatus("�α��� ��...", Color.yellow);

        if (PlayerDataManager.HasPlayerData(playerId))
        {
            PlayerData playerData = PlayerDataManager.LoadPlayerData(playerId);
            if (playerData != null)
            {
                // GameManager�� �÷��̾� ����
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetCurrentPlayer(playerData);
                    ShowStatus($"{playerData.playerName}���� �α��� ����!", Color.green);

                    // ��� ��ٸ� �� �κ��丮 ������ �̵�
                    Invoke(nameof(LoadInventoryScene), 1.0f);
                }
                else
                {
                    ShowStatus("GameManager�� ã�� �� �����ϴ�!", Color.red);
                }
            }
            else
            {
                ShowStatus("�÷��̾� ������ �ε忡 �����߽��ϴ�!", Color.red);
            }
        }
        else
        {
            ShowStatus("�������� �ʴ� �÷��̾��Դϴ�!", Color.red);
        }
    }

    /// <summary>
    /// �� �÷��̾� ���
    /// </summary>
    public void Register(string playerId)
    {
        ShowStatus("��� ��...", Color.yellow);

        if (PlayerDataManager.HasPlayerData(playerId))
        {
            ShowStatus("�̹� �����ϴ� �÷��̾� ID�Դϴ�!", Color.red);
            return;
        }

        if (string.IsNullOrEmpty(playerId) || playerId.Length < 2)
        {
            ShowStatus("�÷��̾� ID�� 2���� �̻��̾�� �մϴ�!", Color.red);
            return;
        }

        try
        {
            // ���ο� �÷��̾� ������ ����
            PlayerData newPlayer = new PlayerData(playerId, playerId);

            // ���� ������ �߰� (���߿� InventoryManager�� ���� ó�� ����)
            newPlayer.inventoryItemIds.Add(1); // �⺻ ����
            newPlayer.inventoryItemIds.Add(5); // �⺻ ����

            // ������ ����
            if (PlayerDataManager.SavePlayerData(newPlayer))
            {
                // GameManager�� �÷��̾� ����
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetCurrentPlayer(newPlayer);
                    ShowStatus($"{playerId} ��� �� �α��� ����!", Color.green);

                    // ��� ��ٸ� �� �κ��丮 ������ �̵�
                    Invoke(nameof(LoadInventoryScene), 1.0f);
                }
                else
                {
                    ShowStatus("GameManager�� ã�� �� �����ϴ�!", Color.red);
                }
            }
            else
            {
                ShowStatus("�÷��̾� ������ ���忡 �����߽��ϴ�!", Color.red);
            }
        }
        catch (System.Exception e)
        {
            ShowStatus($"��� ����: {e.Message}", Color.red);
            Debug.LogError($"�÷��̾� ��� ����: {e}");
        }
    }

    #endregion

    #region Legacy Methods (���� ȣȯ��)

    private void OnLoginButtonClicked()
    {
        if (playerNameInput == null) return;

        string playerName = playerNameInput.text.Trim();

        // �Է� ����
        if (string.IsNullOrEmpty(playerName))
        {
            ShowStatus("�÷��̾� �̸��� �Է����ּ���!", Color.red);
            return;
        }

        if (playerName.Length < 2)
        {
            ShowStatus("�÷��̾� �̸��� 2���� �̻��̾�� �մϴ�!", Color.red);
            return;
        }

        if (playerName.Length > 20)
        {
            ShowStatus("�÷��̾� �̸��� 20���� ���Ͽ��� �մϴ�!", Color.red);
            return;
        }

        // ���� �÷��̾� üũ �� �α��� �Ǵ� ����
        if (PlayerDataManager.HasPlayerData(playerName))
        {
            Login(playerName);
        }
        else
        {
            Register(playerName);
        }
    }

    private void CreateTestPlayer(string testName)
    {
        if (PlayerDataManager.HasPlayerData(testName))
        {
            // �̹� �����ϸ� �α���
            Login(testName);
        }
        else
        {
            // ������ ���� ����
            Register(testName);
        }
    }

    #endregion

    #region Utility Methods

    private void LoadInventoryScene()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadInventoryScene();
        }
    }

    private void ShowStatus(string message, Color color)
    {
        //if (statusText != null)
        //{
        //    statusText.text = message;
        //    statusText.color = color;
        //}

        Debug.Log($"LoginScene: {message}");
    }

    #endregion

    #region Input Validation

    private bool IsValidPlayerName(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        if (name.Length < 2 || name.Length > 20) return false;

        // Ư������ üũ (���û���)
        foreach (char c in name)
        {
            if (!char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c))
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region Context Menu Methods

    [ContextMenu("�׽�Ʈ �α���")]
    private void TestLogin()
    {
        CreateTestPlayer("TestPlayer");
    }

    [ContextMenu("GameManager Ȯ��")]
    private void CheckGameManager()
    {
        if (GameManager.Instance != null)
        {
            Debug.Log("GameManager ���� �۵�");
            Debug.Log($"���� �÷��̾�: {(GameManager.Instance.HasCurrentPlayer ? GameManager.Instance.CurrentPlayer.playerName : "����")}");
        }
        else
        {
            Debug.LogWarning("GameManager�� �����ϴ�!");
        }
    }

    [ContextMenu("����� �÷��̾� ���")]
    private void ShowSavedPlayers()
    {
        var playerIds = PlayerDataManager.GetAllPlayerIds();
        Debug.Log($"����� �÷��̾� ({playerIds.Count}��):");
        foreach (string id in playerIds)
        {
            Debug.Log($"  - {id}");
        }
    }

    [ContextMenu("���� ���� ����")]
    private void OpenSaveFolder()
    {
        PlayerDataManager.OpenSaveFolder();
    }

    #endregion
}