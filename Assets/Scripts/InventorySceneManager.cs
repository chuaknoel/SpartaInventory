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
        // GameManager �̺�Ʈ ����
        GameManager.OnPlayerDataChanged += UpdatePlayerInfo;
        GameManager.OnInventoryChanged += UpdatePlayerInfo;
    }

    private void OnDisable()
    {
        // GameManager �̺�Ʈ ���� ����
        GameManager.OnPlayerDataChanged -= UpdatePlayerInfo;
        GameManager.OnInventoryChanged -= UpdatePlayerInfo;
    }

    private void InitializeUI()
    {
        // ó������ ���� ��길 ǥ��
        ShowMainHub();

        // GameManager�� ������ �α��� ������
        if (GameManager.Instance == null || GameManager.Instance.currentPlayer == null)
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
        inventoryUI.RefreshInventory();
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
        statsUI.RefreshStats();
    }

    private void CloseStats()
    {
        ShowMainHub();
    }

    private void Logout()
    {
        GameManager.Instance.LoadLoginScene();
    }

    #endregion

    #region Player Info Update

    private void UpdatePlayerInfo()
    {
        if (GameManager.Instance == null || GameManager.Instance.currentPlayer == null)
            return;

        PlayerData player = GameManager.Instance.currentPlayer;

        // �÷��̾� �⺻ ����
        playerNameText.text = $"Player: {player.playerName}";
        levelText.text = $"Level: {player.level}";
        characterDescText.text = player.characterDescription;
        goldText.text = $"Gold: {player.gold:N0}";

        // ����ġ ��
        float expProgress = (float)player.currentExp / player.maxExp;
        expBar.value = expProgress;
        expText.text = $"{player.currentExp}/{player.maxExp}";
    }

    #endregion

    #region Test Methods (���߿�)

    [ContextMenu("�׽�Ʈ ������ �߰�")]
    private void AddTestItems()
    {
        if (GameManager.Instance == null) return;

        // �׽�Ʈ�� �����۵� �߰�
        GameManager.Instance.AddItemToInventory(1); // ��
        GameManager.Instance.AddItemToInventory(2); // ����
        GameManager.Instance.AddItemToInventory(3); // ����
        GameManager.Instance.AddItemToInventory(4); // ����
        GameManager.Instance.AddItemToInventory(5); // ����
        GameManager.Instance.AddItemToInventory(6); // ��������

        Debug.Log("�׽�Ʈ �����۵��� �߰��Ǿ����ϴ�!");
    }

    [ContextMenu("��� �߰�")]
    private void AddTestGold()
    {
        if (GameManager.Instance?.currentPlayer != null)
        {
            GameManager.Instance.currentPlayer.gold += 1000;
            UpdatePlayerInfo();
            Debug.Log("��� 1000 �߰�!");
        }
    }

    #endregion
}