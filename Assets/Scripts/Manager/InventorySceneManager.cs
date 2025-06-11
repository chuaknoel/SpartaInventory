using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySceneManager : MonoBehaviour
{
    [Header("=== INVENTORY SCENE MANAGER ===")]

    [Header("Main Panels")]
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private GameObject inventoryWindow;
    [SerializeField] private GameObject statsWindow;

    [Header("Player Info UI (Left Panel)")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider expBar;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI characterDescText;
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("Menu Buttons (Right Panel)")]
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button statsButton;
    [SerializeField] private Button logoutButton;

    [Header("Close Buttons")]
    [SerializeField] private Button inventoryCloseButton;
    [SerializeField] private Button statsCloseButton;

    [Header("UI References")]
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private StatsUI statsUI;

    private void Start()
    {
        InitializeUI();
        SetupButtons();
        UpdatePlayerInfo();
    }

    private void OnEnable()
    {
        // ���ο� GameManager �̺�Ʈ ����
        GameManager.OnPlayerChanged += OnPlayerChanged;
        GameManager.OnPlayerDataUpdated += UpdatePlayerInfo;

        // InventoryManager �̺�Ʈ ���� (�ִٸ�)
        if (InventoryManager.Instance != null)
        {
            InventoryManager.OnInventoryChanged += UpdatePlayerInfo;
        }
    }

    private void OnDisable()
    {
        // �̺�Ʈ ���� ����
        GameManager.OnPlayerChanged -= OnPlayerChanged;
        GameManager.OnPlayerDataUpdated -= UpdatePlayerInfo;

        if (InventoryManager.Instance != null)
        {
            InventoryManager.OnInventoryChanged -= UpdatePlayerInfo;
        }
    }

    private void OnPlayerChanged(PlayerData newPlayer)
    {
        UpdatePlayerInfo();
    }

    private void InitializeUI()
    {
        // ó������ ���� �гθ� ǥ��
        ShowMainHub();

        // GameManager�� ������ �α��� ������
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentPlayer)
        {
            Debug.LogWarning("�÷��̾� �����Ͱ� �����ϴ�. �α��� ������ ���ư��ϴ�.");
            GameManager.Instance?.LoadLoginScene();
            return;
        }
    }

    private void SetupButtons()
    {
        // �޴� ��ư��
        inventoryButton.onClick.AddListener(OpenInventory);
        statsButton.onClick.AddListener(OpenStats);
        logoutButton.onClick.AddListener(Logout);

        // �ݱ� ��ư��
        inventoryCloseButton.onClick.AddListener(CloseInventory);
        statsCloseButton.onClick.AddListener(CloseStats);
    }

    #region UI State Management

    private void ShowMainHub()
    {
        leftPanel.SetActive(true);
        rightPanel.SetActive(true);
        inventoryWindow.SetActive(false);
        statsWindow.SetActive(false);
    }

    private void OpenInventory()
    {
        leftPanel.SetActive(false);
        rightPanel.SetActive(false);
        inventoryWindow.SetActive(true);
        statsWindow.SetActive(false);

        // �κ��丮 UI ���ΰ�ħ
        if (inventoryUI != null)
        {
            inventoryUI.RefreshInventory();
        }
    }

    private void CloseInventory()
    {
        ShowMainHub();
    }

    private void OpenStats()
    {
        leftPanel.SetActive(false);
        rightPanel.SetActive(false);
        inventoryWindow.SetActive(false);
        statsWindow.SetActive(true);

        // ���� UI ���ΰ�ħ
        if (statsUI != null)
        {
            statsUI.RefreshStats();
        }
    }

    private void CloseStats()
    {
        ShowMainHub();
    }

    private void Logout()
    {
        // �÷��̾� ������ ���� �� �α׾ƿ�
        GameManager.Instance.SaveCurrentPlayer();
        GameManager.Instance.LoadLoginScene();
    }

    #endregion

    #region Player Info Update

    private void UpdatePlayerInfo()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentPlayer)
            return;

        PlayerData player = GameManager.Instance.CurrentPlayer; // public ������Ƽ ���

        // �÷��̾� �⺻ ����
        if (playerNameText != null)
            playerNameText.text = $"Player: {player.playerName}";
        if (levelText != null)
            levelText.text = $"Level: {player.level}";
        if (characterDescText != null)
            characterDescText.text = player.characterDescription;
        if (goldText != null)
            goldText.text = $"Gold: {player.gold:N0}";

        // ����ġ ��
        if (expBar != null && expText != null)
        {
            float expProgress = (float)player.currentExp / player.maxExp;
            expBar.value = expProgress;
            expText.text = $"{player.currentExp}/{player.maxExp}";
        }
    }

    #endregion

    #region Test Methods (���߿�)

    [ContextMenu("�׽�Ʈ ������ �߰�")]
    private void AddTestItems()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager�� �����ϴ�!");
            return;
        }

        // InventoryManager�� ���� ������ �߰�
        InventoryManager.Instance.AddTestItems();
        Debug.Log("�׽�Ʈ �����۵��� �߰��Ǿ����ϴ�!");
    }

    [ContextMenu("��� �߰�")]
    private void AddTestGold()
    {
        if (GameManager.Instance?.CurrentPlayer != null)
        {
            GameManager.Instance.CurrentPlayer.gold += 1000;
            GameManager.Instance.NotifyPlayerDataChanged();
            Debug.Log("��� 1000 �߰�!");
        }
    }

    [ContextMenu("������ �׽�Ʈ")]
    private void TestLevelUp()
    {
        if (GameManager.Instance?.CurrentPlayer != null)
        {
            PlayerData player = GameManager.Instance.CurrentPlayer;
            player.level++;
            player.currentExp = 0;
            player.maxExp += 50;

            GameManager.Instance.NotifyPlayerDataChanged();
            Debug.Log($"{player.playerName}��(��) ���� {player.level}�� �Ǿ����ϴ�!");
        }
    }

    [ContextMenu("������ ���� �׽�Ʈ")]
    private void TestSaveData()
    {
        if (GameManager.Instance.SaveCurrentPlayer())
        {
            Debug.Log("�÷��̾� ������ ���� �Ϸ�!");
        }
        else
        {
            Debug.Log("�÷��̾� ������ ���� ����!");
        }
    }

    #endregion
}