using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }

    [Header("Current Player")]
    public PlayerData currentPlayer;

    [Header("Item Database")]
    [SerializeField] private List<ItemData> itemDatabase = new List<ItemData>();

    // UI 업데이트용 이벤트
    public static event System.Action OnPlayerDataChanged;
    public static event System.Action OnInventoryChanged;

    #region Unity Lifecycle

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeItemDatabase();
            Debug.Log("GameManager 초기화 완료");
        }
        else
        {
            Debug.Log("GameManager 중복 생성 방지 - 파괴");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 시작할 때 로그인 씬으로 이동
        if (SceneManager.GetActiveScene().name != "LoginScene")
        {
            Debug.Log("LoginScene으로 이동");
            SceneManager.LoadScene("LoginScene");
        }
    }

    #endregion

    #region Player Management

    /// <summary>
    /// 현재 플레이어 설정
    /// </summary>
    public void SetCurrentPlayer(PlayerData player)
    {
        currentPlayer = player;
        OnPlayerDataChanged?.Invoke();
        Debug.Log($"플레이어 설정: {player.playerName}");
    }

    /// <summary>
    /// 플레이어 데이터 업데이트 알림
    /// </summary>
    public void UpdatePlayerData()
    {
        OnPlayerDataChanged?.Invoke();
    }

    /// <summary>
    /// 현재 플레이어가 있는지 확인
    /// </summary>
    public bool HasCurrentPlayer()
    {
        return currentPlayer != null;
    }

    #endregion

    #region Scene Management

    /// <summary>
    /// 인벤토리 씬으로 이동
    /// </summary>
    public void LoadInventoryScene()
    {
        if (currentPlayer == null)
        {
            Debug.LogError("플레이어 데이터가 없어서 인벤토리 씬으로 이동할 수 없습니다!");
            return;
        }

        Debug.Log("인벤토리 씬으로 이동");
        SceneManager.LoadScene("InventoryScene");
    }

    /// <summary>
    /// 로그인 씬으로 돌아가기
    /// </summary>
    public void LoadLoginScene()
    {
        currentPlayer = null;
        Debug.Log("로그인 씬으로 돌아가기");
        SceneManager.LoadScene("LoginScene");
    }

    #endregion

    #region Item Database

    /// <summary>
    /// 아이템 데이터베이스 초기화
    /// </summary>
    private void InitializeItemDatabase()
    {
        itemDatabase.Clear();

        // 샘플 아이템들 추가
        AddSampleItems();

        Debug.Log($"아이템 데이터베이스 초기화 완료: {itemDatabase.Count}개 아이템");
    }

    /// <summary>
    /// 샘플 아이템들 추가
    /// </summary>
    private void AddSampleItems()
    {
        itemDatabase.Add(new ItemData(1, "검", "기본적인 검", ItemType.Weapon, 10));
        itemDatabase.Add(new ItemData(2, "방패", "나무 방패", ItemType.Shield, 8));
        itemDatabase.Add(new ItemData(3, "투구", "철제 투구", ItemType.Helmet, 15));
        itemDatabase.Add(new ItemData(4, "갑옷", "가죽 갑옷", ItemType.Armor, 25));
        itemDatabase.Add(new ItemData(5, "물약", "체력 회복 물약", ItemType.Consumable, 5));
        itemDatabase.Add(new ItemData(6, "마나물약", "마나 회복 물약", ItemType.Consumable, 7));
        itemDatabase.Add(new ItemData(7, "장화", "가죽 장화", ItemType.Boots, 12));
        itemDatabase.Add(new ItemData(8, "반지", "마법의 반지", ItemType.Accessory, 50));
    }

    /// <summary>
    /// 아이템 ID로 아이템 데이터 가져오기
    /// </summary>
    public ItemData GetItemData(int itemId)
    {
        return itemDatabase.Find(item => item.itemId == itemId);
    }

    /// <summary>
    /// 모든 아이템 데이터 가져오기
    /// </summary>
    public List<ItemData> GetAllItems()
    {
        return new List<ItemData>(itemDatabase);
    }

    #endregion

    #region Inventory Management

    /// <summary>
    /// 인벤토리에 아이템 추가
    /// </summary>
    public void AddItemToInventory(int itemId)
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("플레이어가 없어서 아이템을 추가할 수 없습니다.");
            return;
        }

        ItemData item = GetItemData(itemId);
        if (item == null)
        {
            Debug.LogWarning($"아이템 ID {itemId}를 찾을 수 없습니다.");
            return;
        }

        currentPlayer.inventoryItemIds.Add(itemId);
        OnInventoryChanged?.Invoke();
        Debug.Log($"아이템 추가: {item.itemName}");
    }

    /// <summary>
    /// 인벤토리에서 아이템 제거
    /// </summary>
    public void RemoveItemFromInventory(int itemId)
    {
        if (currentPlayer == null) return;

        ItemData item = GetItemData(itemId);
        if (currentPlayer.inventoryItemIds.Remove(itemId))
        {
            OnInventoryChanged?.Invoke();
            Debug.Log($"아이템 제거: {item?.itemName ?? "알 수 없는 아이템"}");
        }
    }

    /// <summary>
    /// 아이템 장착
    /// </summary>
    public void EquipItem(int itemId)
    {
        if (currentPlayer == null) return;

        ItemData item = GetItemData(itemId);
        if (item == null)
        {
            Debug.LogWarning($"장착하려는 아이템 ID {itemId}를 찾을 수 없습니다.");
            return;
        }

        // 소비품은 장착할 수 없음
        if (item.itemType == ItemType.Consumable)
        {
            Debug.Log($"{item.itemName}은 장착할 수 없는 소비품입니다.");
            return;
        }

        // 같은 타입의 장비가 이미 장착되어 있으면 해제
        UnequipItemByType(item.itemType);

        // 인벤토리에서 제거하고 장착 목록에 추가
        if (currentPlayer.inventoryItemIds.Remove(itemId))
        {
            currentPlayer.equippedItemIds.Add(itemId);
            OnInventoryChanged?.Invoke();
            Debug.Log($"아이템 장착: {item.itemName}");
        }
        else
        {
            Debug.LogWarning($"인벤토리에 {item.itemName}이 없어서 장착할 수 없습니다.");
        }
    }

    /// <summary>
    /// 아이템 장착 해제
    /// </summary>
    public void UnequipItem(int itemId)
    {
        if (currentPlayer == null) return;

        ItemData item = GetItemData(itemId);
        if (currentPlayer.equippedItemIds.Remove(itemId))
        {
            currentPlayer.inventoryItemIds.Add(itemId);
            OnInventoryChanged?.Invoke();
            Debug.Log($"아이템 장착 해제: {item?.itemName ?? "알 수 없는 아이템"}");
        }
    }

    /// <summary>
    /// 특정 타입의 장착된 아이템 해제
    /// </summary>
    private void UnequipItemByType(ItemType itemType)
    {
        for (int i = currentPlayer.equippedItemIds.Count - 1; i >= 0; i--)
        {
            int equippedItemId = currentPlayer.equippedItemIds[i];
            ItemData equippedItem = GetItemData(equippedItemId);

            if (equippedItem != null && equippedItem.itemType == itemType)
            {
                UnequipItem(equippedItemId);
                break; // 같은 타입은 하나만 장착되므로 하나만 해제
            }
        }
    }

    /// <summary>
    /// 아이템이 장착되어 있는지 확인
    /// </summary>
    public bool IsItemEquipped(int itemId)
    {
        if (currentPlayer == null) return false;
        return currentPlayer.equippedItemIds.Contains(itemId);
    }

    #endregion

    #region Stats Calculation

    /// <summary>
    /// 플레이어의 계산된 스탯 가져오기 (기본 스탯 + 장비 보너스)
    /// </summary>
    public PlayerStats GetCalculatedStats()
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("플레이어가 없어서 기본 스탯을 반환합니다.");
            return new PlayerStats();
        }

        // 기본 스탯 (레벨에 따른)
        PlayerStats stats = new PlayerStats
        {
            baseAttack = currentPlayer.baseAttack + (currentPlayer.level - 1) * 3,
            baseDefense = currentPlayer.baseDefense + (currentPlayer.level - 1) * 2,
            baseHealth = currentPlayer.baseHealth + (currentPlayer.level - 1) * 15,
            baseSpeed = currentPlayer.baseSpeed + (currentPlayer.level - 1) * 1
        };

        // 장착된 아이템들의 보너스 계산
        foreach (int itemId in currentPlayer.equippedItemIds)
        {
            ItemData item = GetItemData(itemId);
            if (item != null)
            {
                AddItemBonusToStats(item, stats);
            }
        }

        return stats;
    }

    /// <summary>
    /// 아이템 보너스를 스탯에 추가
    /// </summary>
    private void AddItemBonusToStats(ItemData item, PlayerStats stats)
    {
        switch (item.itemType)
        {
            case ItemType.Weapon:
                stats.bonusAttack += item.value;
                break;

            case ItemType.Shield:
            case ItemType.Helmet:
            case ItemType.Armor:
            case ItemType.Boots:
                stats.bonusDefense += item.value;
                break;

            case ItemType.Accessory:
                // 액세서리는 공격력과 방어력 모두에 보너스
                stats.bonusAttack += item.value / 2;
                stats.bonusDefense += item.value / 2;
                break;
        }
    }

    #endregion

    #region Debug & Test Methods

    [ContextMenu("테스트 아이템 추가")]
    public void AddTestItems()
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("플레이어가 없어서 테스트 아이템을 추가할 수 없습니다.");
            return;
        }

        // 모든 아이템을 하나씩 추가
        for (int i = 1; i <= 8; i++)
        {
            AddItemToInventory(i);
        }

        Debug.Log("모든 테스트 아이템이 추가되었습니다!");
    }

    [ContextMenu("현재 상태 출력")]
    public void PrintCurrentState()
    {
        if (currentPlayer == null)
        {
            Debug.Log("현재 플레이어 없음");
            return;
        }

        Debug.Log($"=== {currentPlayer.playerName}의 현재 상태 ===");
        Debug.Log($"레벨: {currentPlayer.level}");
        Debug.Log($"골드: {currentPlayer.gold}");
        Debug.Log($"인벤토리 아이템 수: {currentPlayer.inventoryItemIds.Count}");
        Debug.Log($"장착된 아이템 수: {currentPlayer.equippedItemIds.Count}");

        PlayerStats stats = GetCalculatedStats();
        Debug.Log($"총 공격력: {stats.TotalAttack} (기본: {stats.baseAttack} + 보너스: {stats.bonusAttack})");
        Debug.Log($"총 방어력: {stats.TotalDefense} (기본: {stats.baseDefense} + 보너스: {stats.bonusDefense})");
    }

    #endregion
}