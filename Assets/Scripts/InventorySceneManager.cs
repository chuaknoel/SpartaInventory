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
        // GameManager 이벤트 구독
        GameManager.OnPlayerDataChanged += UpdatePlayerInfo;
        GameManager.OnInventoryChanged += UpdatePlayerInfo;
    }

    private void OnDisable()
    {
        // GameManager 이벤트 구독 해제
        GameManager.OnPlayerDataChanged -= UpdatePlayerInfo;
        GameManager.OnInventoryChanged -= UpdatePlayerInfo;
    }

    private void InitializeUI()
    {
        // 처음에는 메인 허브만 표시
        ShowMainHub();

        // GameManager가 없으면 로그인 씬으로
        if (GameManager.Instance == null || GameManager.Instance.currentPlayer == null)
        {
            Debug.LogWarning("플레이어 데이터가 없습니다. 로그인 씬으로 돌아갑니다.");
            GameManager.Instance?.LoadLoginScene();
            return;
        }
    }

    private void SetupButtons()
    {
        // 메뉴 버튼들
        inventoryButton.onClick.AddListener(OpenInventory);
        statsButton.onClick.AddListener(OpenStats);
        logoutButton.onClick.AddListener(Logout);

        // 닫기 버튼들
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

        // 인벤토리 UI 새로고침
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

        // 스탯 UI 새로고침
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

        // 플레이어 기본 정보
        playerNameText.text = $"Player: {player.playerName}";
        levelText.text = $"Level: {player.level}";
        characterDescText.text = player.characterDescription;
        goldText.text = $"Gold: {player.gold:N0}";

        // 경험치 바
        float expProgress = (float)player.currentExp / player.maxExp;
        expBar.value = expProgress;
        expText.text = $"{player.currentExp}/{player.maxExp}";
    }

    #endregion

    #region Test Methods (개발용)

    [ContextMenu("테스트 아이템 추가")]
    private void AddTestItems()
    {
        if (GameManager.Instance == null) return;

        // 테스트용 아이템들 추가
        GameManager.Instance.AddItemToInventory(1); // 검
        GameManager.Instance.AddItemToInventory(2); // 방패
        GameManager.Instance.AddItemToInventory(3); // 투구
        GameManager.Instance.AddItemToInventory(4); // 갑옷
        GameManager.Instance.AddItemToInventory(5); // 물약
        GameManager.Instance.AddItemToInventory(6); // 마나물약

        Debug.Log("테스트 아이템들이 추가되었습니다!");
    }

    [ContextMenu("골드 추가")]
    private void AddTestGold()
    {
        if (GameManager.Instance?.currentPlayer != null)
        {
            GameManager.Instance.currentPlayer.gold += 1000;
            UpdatePlayerInfo();
            Debug.Log("골드 1000 추가!");
        }
    }

    #endregion
}