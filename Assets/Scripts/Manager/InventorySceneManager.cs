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
        // 새로운 GameManager 이벤트 구독
        GameManager.OnPlayerChanged += OnPlayerChanged;
        GameManager.OnPlayerDataUpdated += UpdatePlayerInfo;

        // InventoryManager 이벤트 구독 (있다면)
        if (InventoryManager.Instance != null)
        {
            InventoryManager.OnInventoryChanged += UpdatePlayerInfo;
        }
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
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
        // 처음에는 메인 패널만 표시
        ShowMainHub();

        // GameManager가 없으면 로그인 씬으로
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentPlayer)
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

        // 스탯 UI 새로고침
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
        // 플레이어 데이터 저장 후 로그아웃
        GameManager.Instance.SaveCurrentPlayer();
        GameManager.Instance.LoadLoginScene();
    }

    #endregion

    #region Player Info Update

    private void UpdatePlayerInfo()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentPlayer)
            return;

        PlayerData player = GameManager.Instance.CurrentPlayer; // public 프로퍼티 사용

        // 플레이어 기본 정보
        if (playerNameText != null)
            playerNameText.text = $"Player: {player.playerName}";
        if (levelText != null)
            levelText.text = $"Level: {player.level}";
        if (characterDescText != null)
            characterDescText.text = player.characterDescription;
        if (goldText != null)
            goldText.text = $"Gold: {player.gold:N0}";

        // 경험치 바
        if (expBar != null && expText != null)
        {
            float expProgress = (float)player.currentExp / player.maxExp;
            expBar.value = expProgress;
            expText.text = $"{player.currentExp}/{player.maxExp}";
        }
    }

    #endregion

    #region Test Methods (개발용)

    [ContextMenu("테스트 아이템 추가")]
    private void AddTestItems()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager가 없습니다!");
            return;
        }

        // InventoryManager를 통해 아이템 추가
        InventoryManager.Instance.AddTestItems();
        Debug.Log("테스트 아이템들이 추가되었습니다!");
    }

    [ContextMenu("골드 추가")]
    private void AddTestGold()
    {
        if (GameManager.Instance?.CurrentPlayer != null)
        {
            GameManager.Instance.CurrentPlayer.gold += 1000;
            GameManager.Instance.NotifyPlayerDataChanged();
            Debug.Log("골드 1000 추가!");
        }
    }

    [ContextMenu("레벨업 테스트")]
    private void TestLevelUp()
    {
        if (GameManager.Instance?.CurrentPlayer != null)
        {
            PlayerData player = GameManager.Instance.CurrentPlayer;
            player.level++;
            player.currentExp = 0;
            player.maxExp += 50;

            GameManager.Instance.NotifyPlayerDataChanged();
            Debug.Log($"{player.playerName}이(가) 레벨 {player.level}이 되었습니다!");
        }
    }

    [ContextMenu("데이터 저장 테스트")]
    private void TestSaveData()
    {
        if (GameManager.Instance.SaveCurrentPlayer())
        {
            Debug.Log("플레이어 데이터 저장 완료!");
        }
        else
        {
            Debug.Log("플레이어 데이터 저장 실패!");
        }
    }

    #endregion
}