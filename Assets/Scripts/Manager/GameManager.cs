using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("현재 플레이어")]
    [SerializeField] private PlayerData currentPlayer;

    [Header("씬 설정")]
    [SerializeField] private string loginSceneName = "LoginScene";
    [SerializeField] private string inventorySceneName = "InventoryScene";

    [Header("디버그")]
    [SerializeField] private bool showDebugLogs = true;

    // 이벤트
    public static event System.Action<PlayerData> OnPlayerChanged;
    public static event System.Action OnPlayerDataUpdated;

    // 프로퍼티
    public PlayerData CurrentPlayer => currentPlayer;
    public bool HasCurrentPlayer => currentPlayer != null;

    #region Unity Lifecycle

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            if (showDebugLogs)
                Debug.Log("GameManager 중복 생성 방지 - 파괴");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 시작시 로그인 씬으로 이동
        if (SceneManager.GetActiveScene().name != loginSceneName)
        {
            if (showDebugLogs)
                Debug.Log($"{loginSceneName}으로 이동");
            LoadLoginScene();
        }
    }

    #endregion

    #region Initialization

    private void Initialize()
    {
        if (showDebugLogs)
            Debug.Log("GameManager 초기화 완료");
    }

    #endregion

    #region Player Management

    /// <summary>
    /// 현재 플레이어 설정
    /// </summary>
    public void SetCurrentPlayer(PlayerData player)
    {
        if (player == null)
        {
            Debug.LogError("null 플레이어를 설정하려고 했습니다!");
            return;
        }

        currentPlayer = player;
        OnPlayerChanged?.Invoke(currentPlayer);

        if (showDebugLogs)
            Debug.Log($"플레이어 설정: {player.playerName} (ID: {player.playerId})");
    }

    /// <summary>
    /// 플레이어 데이터 업데이트 알림
    /// </summary>
    public void NotifyPlayerDataChanged()
    {
        OnPlayerDataUpdated?.Invoke();

        if (showDebugLogs)
            Debug.Log("플레이어 데이터 업데이트 알림");
    }

    /// <summary>
    /// 현재 플레이어 제거 (로그아웃)
    /// </summary>
    public void ClearCurrentPlayer()
    {
        if (currentPlayer != null && showDebugLogs)
            Debug.Log($"{currentPlayer.playerName} 로그아웃");

        currentPlayer = null;
        OnPlayerChanged?.Invoke(null);
    }

    /// <summary>
    /// 플레이어 데이터 저장
    /// </summary>
    public bool SaveCurrentPlayer()
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("저장할 플레이어가 없습니다.");
            return false;
        }

        bool success = PlayerDataManager.SavePlayerData(currentPlayer);

        if (success && showDebugLogs)
            Debug.Log($"{currentPlayer.playerName}의 데이터 저장 완료");

        return success;
    }

    #endregion

    #region Scene Management

    /// <summary>
    /// 로그인 씬으로 이동
    /// </summary>
    public void LoadLoginScene()
    {
        // 현재 플레이어 데이터 저장
        if (currentPlayer != null)
        {
            SaveCurrentPlayer();
        }

        ClearCurrentPlayer();

        if (showDebugLogs)
            Debug.Log("로그인 씬으로 돌아가기");

        SceneManager.LoadScene(loginSceneName);
    }

    /// <summary>
    /// 인벤토리 씬으로 이동
    /// </summary>
    public void LoadInventoryScene()
    {
        if (currentPlayer == null)
        {
            Debug.LogError("플레이어 데이터가 없어서 인벤토리 씬으로 이동할 수 없습니다!");
            LoadLoginScene();
            return;
        }

        if (showDebugLogs)
            Debug.Log("인벤토리 씬으로 이동");

        SceneManager.LoadScene(inventorySceneName);
    }

    /// <summary>
    /// 특정 씬으로 이동
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("씬 이름이 비어있습니다!");
            return;
        }

        if (showDebugLogs)
            Debug.Log($"{sceneName} 씬으로 이동");

        SceneManager.LoadScene(sceneName);
    }

    #endregion

    #region Player Stats (통합 계산)

    /// <summary>
    /// 플레이어의 최종 스탯 계산 (기본 스탯 + 장비 보너스)
    /// </summary>
    public PlayerStats GetPlayerTotalStats()
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("플레이어가 없어서 기본 스탯을 반환합니다.");
            return new PlayerStats();
        }

        // 기본 스탯 (레벨에 따른)
        PlayerStats totalStats = GetPlayerBaseStats();

        // 장비 보너스 스탯
        if (InventoryManager.Instance != null)
        {
            PlayerStats equipmentBonus = InventoryManager.Instance.GetEquipmentBonusStats();
            totalStats.bonusAttack = equipmentBonus.bonusAttack;
            totalStats.bonusDefense = equipmentBonus.bonusDefense;
            totalStats.bonusHealth = equipmentBonus.bonusHealth;
            totalStats.bonusSpeed = equipmentBonus.bonusSpeed;
        }

        return totalStats;
    }

    /// <summary>
    /// 플레이어의 기본 스탯 (레벨 보정 포함)
    /// </summary>
    public PlayerStats GetPlayerBaseStats()
    {
        if (currentPlayer == null)
            return new PlayerStats();

        // 레벨에 따른 기본 스탯 증가
        int levelBonus = currentPlayer.level - 1;

        return new PlayerStats
        {
            baseAttack = currentPlayer.baseAttack + (levelBonus * 3),
            baseDefense = currentPlayer.baseDefense + (levelBonus * 2),
            baseHealth = currentPlayer.baseHealth + (levelBonus * 15),
            baseSpeed = currentPlayer.baseSpeed + (levelBonus * 1)
        };
    }

    #endregion

    #region Application Events

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && currentPlayer != null)
        {
            SaveCurrentPlayer();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && currentPlayer != null)
        {
            SaveCurrentPlayer();
        }
    }

    private void OnApplicationQuit()
    {
        if (currentPlayer != null)
        {
            SaveCurrentPlayer();
        }
    }

    #endregion

    #region Debug & Utility

    /// <summary>
    /// 게임 상태 출력
    /// </summary>
    [ContextMenu("게임 상태 출력")]
    public void PrintGameState()
    {
        Debug.Log("=== 게임 상태 ===");
        Debug.Log($"현재 씬: {SceneManager.GetActiveScene().name}");

        if (currentPlayer != null)
        {
            Debug.Log($"현재 플레이어: {currentPlayer.playerName} (레벨 {currentPlayer.level})");

            PlayerStats stats = GetPlayerTotalStats();
            Debug.Log($"총 공격력: {stats.TotalAttack} (기본: {stats.baseAttack} + 보너스: {stats.bonusAttack})");
            Debug.Log($"총 방어력: {stats.TotalDefense} (기본: {stats.baseDefense} + 보너스: {stats.bonusDefense})");
        }
        else
        {
            Debug.Log("현재 플레이어: 없음");
        }

        Debug.Log($"InventoryManager: {(InventoryManager.Instance != null ? "활성" : "비활성")}");
    }

    /// <summary>
    /// 플레이어 데이터 강제 저장
    /// </summary>
    [ContextMenu("플레이어 데이터 저장")]
    public void ForceSavePlayer()
    {
        if (SaveCurrentPlayer())
        {
            Debug.Log("플레이어 데이터 저장 완료!");
        }
        else
        {
            Debug.Log("플레이어 데이터 저장 실패!");
        }
    }

    /// <summary>
    /// 테스트용 플레이어 생성
    /// </summary>
    [ContextMenu("테스트 플레이어 생성")]
    public void CreateTestPlayer()
    {
        PlayerData testPlayer = new PlayerData("TestPlayer", "테스트플레이어");
        testPlayer.level = 5;
        testPlayer.gold = 5000;
        testPlayer.currentExp = 50;
        testPlayer.maxExp = 200;

        SetCurrentPlayer(testPlayer);
        Debug.Log("테스트 플레이어 생성 완료!");
    }




    #endregion
}