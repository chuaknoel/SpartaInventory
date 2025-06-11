using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("���� �÷��̾�")]
    [SerializeField] private PlayerData currentPlayer;

    [Header("�� ����")]
    [SerializeField] private string loginSceneName = "LoginScene";
    [SerializeField] private string inventorySceneName = "InventoryScene";

    [Header("�����")]
    [SerializeField] private bool showDebugLogs = true;

    // �̺�Ʈ
    public static event System.Action<PlayerData> OnPlayerChanged;
    public static event System.Action OnPlayerDataUpdated;

    // ������Ƽ
    public PlayerData CurrentPlayer => currentPlayer;
    public bool HasCurrentPlayer => currentPlayer != null;

    #region Unity Lifecycle

    private void Awake()
    {
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            if (showDebugLogs)
                Debug.Log("GameManager �ߺ� ���� ���� - �ı�");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // ���۽� �α��� ������ �̵�
        if (SceneManager.GetActiveScene().name != loginSceneName)
        {
            if (showDebugLogs)
                Debug.Log($"{loginSceneName}���� �̵�");
            LoadLoginScene();
        }
    }

    #endregion

    #region Initialization

    private void Initialize()
    {
        if (showDebugLogs)
            Debug.Log("GameManager �ʱ�ȭ �Ϸ�");
    }

    #endregion

    #region Player Management

    /// <summary>
    /// ���� �÷��̾� ����
    /// </summary>
    public void SetCurrentPlayer(PlayerData player)
    {
        if (player == null)
        {
            Debug.LogError("null �÷��̾ �����Ϸ��� �߽��ϴ�!");
            return;
        }

        currentPlayer = player;
        OnPlayerChanged?.Invoke(currentPlayer);

        if (showDebugLogs)
            Debug.Log($"�÷��̾� ����: {player.playerName} (ID: {player.playerId})");
    }

    /// <summary>
    /// �÷��̾� ������ ������Ʈ �˸�
    /// </summary>
    public void NotifyPlayerDataChanged()
    {
        OnPlayerDataUpdated?.Invoke();

        if (showDebugLogs)
            Debug.Log("�÷��̾� ������ ������Ʈ �˸�");
    }

    /// <summary>
    /// ���� �÷��̾� ���� (�α׾ƿ�)
    /// </summary>
    public void ClearCurrentPlayer()
    {
        if (currentPlayer != null && showDebugLogs)
            Debug.Log($"{currentPlayer.playerName} �α׾ƿ�");

        currentPlayer = null;
        OnPlayerChanged?.Invoke(null);
    }

    /// <summary>
    /// �÷��̾� ������ ����
    /// </summary>
    public bool SaveCurrentPlayer()
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("������ �÷��̾ �����ϴ�.");
            return false;
        }

        bool success = PlayerDataManager.SavePlayerData(currentPlayer);

        if (success && showDebugLogs)
            Debug.Log($"{currentPlayer.playerName}�� ������ ���� �Ϸ�");

        return success;
    }

    #endregion

    #region Scene Management

    /// <summary>
    /// �α��� ������ �̵�
    /// </summary>
    public void LoadLoginScene()
    {
        // ���� �÷��̾� ������ ����
        if (currentPlayer != null)
        {
            SaveCurrentPlayer();
        }

        ClearCurrentPlayer();

        if (showDebugLogs)
            Debug.Log("�α��� ������ ���ư���");

        SceneManager.LoadScene(loginSceneName);
    }

    /// <summary>
    /// �κ��丮 ������ �̵�
    /// </summary>
    public void LoadInventoryScene()
    {
        if (currentPlayer == null)
        {
            Debug.LogError("�÷��̾� �����Ͱ� ��� �κ��丮 ������ �̵��� �� �����ϴ�!");
            LoadLoginScene();
            return;
        }

        if (showDebugLogs)
            Debug.Log("�κ��丮 ������ �̵�");

        SceneManager.LoadScene(inventorySceneName);
    }

    /// <summary>
    /// Ư�� ������ �̵�
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("�� �̸��� ����ֽ��ϴ�!");
            return;
        }

        if (showDebugLogs)
            Debug.Log($"{sceneName} ������ �̵�");

        SceneManager.LoadScene(sceneName);
    }

    #endregion

    #region Player Stats (���� ���)

    /// <summary>
    /// �÷��̾��� ���� ���� ��� (�⺻ ���� + ��� ���ʽ�)
    /// </summary>
    public PlayerStats GetPlayerTotalStats()
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("�÷��̾ ��� �⺻ ������ ��ȯ�մϴ�.");
            return new PlayerStats();
        }

        // �⺻ ���� (������ ����)
        PlayerStats totalStats = GetPlayerBaseStats();

        // ��� ���ʽ� ����
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
    /// �÷��̾��� �⺻ ���� (���� ���� ����)
    /// </summary>
    public PlayerStats GetPlayerBaseStats()
    {
        if (currentPlayer == null)
            return new PlayerStats();

        // ������ ���� �⺻ ���� ����
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
    /// ���� ���� ���
    /// </summary>
    [ContextMenu("���� ���� ���")]
    public void PrintGameState()
    {
        Debug.Log("=== ���� ���� ===");
        Debug.Log($"���� ��: {SceneManager.GetActiveScene().name}");

        if (currentPlayer != null)
        {
            Debug.Log($"���� �÷��̾�: {currentPlayer.playerName} (���� {currentPlayer.level})");

            PlayerStats stats = GetPlayerTotalStats();
            Debug.Log($"�� ���ݷ�: {stats.TotalAttack} (�⺻: {stats.baseAttack} + ���ʽ�: {stats.bonusAttack})");
            Debug.Log($"�� ����: {stats.TotalDefense} (�⺻: {stats.baseDefense} + ���ʽ�: {stats.bonusDefense})");
        }
        else
        {
            Debug.Log("���� �÷��̾�: ����");
        }

        Debug.Log($"InventoryManager: {(InventoryManager.Instance != null ? "Ȱ��" : "��Ȱ��")}");
    }

    /// <summary>
    /// �÷��̾� ������ ���� ����
    /// </summary>
    [ContextMenu("�÷��̾� ������ ����")]
    public void ForceSavePlayer()
    {
        if (SaveCurrentPlayer())
        {
            Debug.Log("�÷��̾� ������ ���� �Ϸ�!");
        }
        else
        {
            Debug.Log("�÷��̾� ������ ���� ����!");
        }
    }

    /// <summary>
    /// �׽�Ʈ�� �÷��̾� ����
    /// </summary>
    [ContextMenu("�׽�Ʈ �÷��̾� ����")]
    public void CreateTestPlayer()
    {
        PlayerData testPlayer = new PlayerData("TestPlayer", "�׽�Ʈ�÷��̾�");
        testPlayer.level = 5;
        testPlayer.gold = 5000;
        testPlayer.currentExp = 50;
        testPlayer.maxExp = 200;

        SetCurrentPlayer(testPlayer);
        Debug.Log("�׽�Ʈ �÷��̾� ���� �Ϸ�!");
    }




    #endregion
}