using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("아이템 데이터베이스")]
    [SerializeField] private ItemDatabaseSO itemDatabase;

    [Header("현재 플레이어")]
    private PlayerData currentPlayer;

    // 이벤트
    public static event System.Action OnInventoryChanged;
    public static event System.Action OnEquipmentChanged;

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ValidateDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // GameManager 이벤트 구독
        GameManager.OnPlayerChanged += OnPlayerChanged;
    }

    private void OnDisable()
    {
        // GameManager 이벤트 구독 해제
        GameManager.OnPlayerChanged -= OnPlayerChanged;
    }

    #endregion

    #region Setup & Validation

    private void ValidateDatabase()
    {
        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabase가 할당되지 않았습니다!");
        }
        else
        {
            Debug.Log($"인벤토리 매니저 초기화 완료 - {itemDatabase.allItems.Count}개 아이템 로드됨");
        }
    }

    private void OnPlayerChanged(PlayerData newPlayer)
    {
        currentPlayer = newPlayer;
        OnInventoryChanged?.Invoke();
        OnEquipmentChanged?.Invoke();
    }

    #endregion

    #region Item Database Access

    /// <summary>
    /// ID로 아이템 데이터 가져오기
    /// </summary>
    public ItemDataSO GetItemData(int itemId)
    {
        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabase가 없습니다!");
            return null;
        }

        return itemDatabase.GetItemById(itemId);
    }

    /// <summary>
    /// 모든 아이템 데이터 가져오기
    /// </summary>
    public List<ItemDataSO> GetAllItems()
    {
        if (itemDatabase == null) return new List<ItemDataSO>();
        return new List<ItemDataSO>(itemDatabase.allItems);
    }

    /// <summary>
    /// 타입별 아이템 가져오기
    /// </summary>
    public List<ItemDataSO> GetItemsByType(ItemType itemType)
    {
        if (itemDatabase == null) return new List<ItemDataSO>();
        return itemDatabase.GetItemsByType(itemType);
    }

    #endregion

    #region Inventory Management

    /// <summary>
    /// 인벤토리에 아이템 추가
    /// </summary>
    public bool AddItem(int itemId, int quantity = 1)
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("플레이어가 없어서 아이템을 추가할 수 없습니다.");
            return false;
        }

        ItemDataSO item = GetItemData(itemId);
        if (item == null)
        {
            Debug.LogWarning($"아이템 ID {itemId}를 찾을 수 없습니다.");
            return false;
        }

        // 수량만큼 추가 (나중에 스택 시스템 구현시 활용)
        for (int i = 0; i < quantity; i++)
        {
            currentPlayer.inventoryItemIds.Add(itemId);
        }

        OnInventoryChanged?.Invoke();
        Debug.Log($"아이템 추가: {item.itemName} x{quantity}");
        return true;
    }

    /// <summary>
    /// 인벤토리에서 아이템 제거
    /// </summary>
    public bool RemoveItem(int itemId, int quantity = 1)
    {
        if (currentPlayer == null) return false;

        ItemDataSO item = GetItemData(itemId);
        int removedCount = 0;

        // 지정된 수량만큼 제거
        for (int i = 0; i < quantity; i++)
        {
            if (currentPlayer.inventoryItemIds.Remove(itemId))
            {
                removedCount++;
            }
            else
            {
                break; // 더 이상 제거할 아이템이 없음
            }
        }

        if (removedCount > 0)
        {
            OnInventoryChanged?.Invoke();
            Debug.Log($"아이템 제거: {item?.itemName ?? "알 수 없는 아이템"} x{removedCount}");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 인벤토리에 특정 아이템이 몇 개 있는지 확인
    /// </summary>
    public int GetItemCount(int itemId)
    {
        if (currentPlayer == null) return 0;
        return currentPlayer.inventoryItemIds.Count(id => id == itemId);
    }

    /// <summary>
    /// 인벤토리에 아이템이 있는지 확인
    /// </summary>
    public bool HasItem(int itemId)
    {
        return GetItemCount(itemId) > 0;
    }

    /// <summary>
    /// 인벤토리의 모든 아이템 ID 가져오기 (중복 포함)
    /// </summary>
    public List<int> GetInventoryItemIds()
    {
        if (currentPlayer == null) return new List<int>();
        return new List<int>(currentPlayer.inventoryItemIds);
    }

    /// <summary>
    /// 인벤토리의 고유 아이템들과 수량 가져오기
    /// </summary>
    public Dictionary<int, int> GetInventoryItemCounts()
    {
        if (currentPlayer == null) return new Dictionary<int, int>();

        return currentPlayer.inventoryItemIds
            .GroupBy(id => id)
            .ToDictionary(group => group.Key, group => group.Count());
    }

    #endregion

    #region Equipment Management

    /// <summary>
    /// 아이템 장착
    /// </summary>
    public bool EquipItem(int itemId)
    {
        if (currentPlayer == null) return false;

        ItemDataSO item = GetItemData(itemId);
        if (item == null)
        {
            Debug.LogWarning($"장착하려는 아이템 ID {itemId}를 찾을 수 없습니다.");
            return false;
        }

        if (!item.IsEquippable())
        {
            Debug.Log($"{item.itemName}는 장착할 수 없는 아이템입니다.");
            return false;
        }

        // 같은 타입의 장비가 이미 장착되어 있으면 해제
        UnequipItemByType(item.itemType);

        // 인벤토리에서 제거하고 장착 목록에 추가
        if (currentPlayer.inventoryItemIds.Remove(itemId))
        {
            currentPlayer.equippedItemIds.Add(itemId);
            OnInventoryChanged?.Invoke();
            OnEquipmentChanged?.Invoke();
            Debug.Log($"아이템 장착: {item.itemName}");
            return true;
        }
        else
        {
            Debug.LogWarning($"인벤토리에 {item.itemName}이 없어서 장착할 수 없습니다.");
            return false;
        }
    }

    /// <summary>
    /// 아이템 장착 해제
    /// </summary>
    public bool UnequipItem(int itemId)
    {
        if (currentPlayer == null) return false;

        ItemDataSO item = GetItemData(itemId);
        if (currentPlayer.equippedItemIds.Remove(itemId))
        {
            currentPlayer.inventoryItemIds.Add(itemId);
            OnInventoryChanged?.Invoke();
            OnEquipmentChanged?.Invoke();
            Debug.Log($"아이템 장착 해제: {item?.itemName ?? "알 수 없는 아이템"}");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 특정 타입의 장착된 아이템 해제
    /// </summary>
    private void UnequipItemByType(ItemType itemType)
    {
        if (currentPlayer == null) return;

        for (int i = currentPlayer.equippedItemIds.Count - 1; i >= 0; i--)
        {
            int equippedItemId = currentPlayer.equippedItemIds[i];
            ItemDataSO equippedItem = GetItemData(equippedItemId);

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

    /// <summary>
    /// 장착된 모든 아이템 ID 가져오기
    /// </summary>
    public List<int> GetEquippedItemIds()
    {
        if (currentPlayer == null) return new List<int>();
        return new List<int>(currentPlayer.equippedItemIds);
    }

    /// <summary>
    /// 특정 타입의 장착된 아이템 가져오기
    /// </summary>
    public ItemDataSO GetEquippedItemByType(ItemType itemType)
    {
        if (currentPlayer == null) return null;

        foreach (int itemId in currentPlayer.equippedItemIds)
        {
            ItemDataSO item = GetItemData(itemId);
            if (item != null && item.itemType == itemType)
            {
                return item;
            }
        }

        return null;
    }

    #endregion

    #region Stats Calculation

    /// <summary>
    /// 장착된 아이템들의 보너스 스탯 계산
    /// </summary>
    public PlayerStats GetEquipmentBonusStats()
    {
        PlayerStats bonusStats = new PlayerStats();

        if (currentPlayer == null) return bonusStats;

        foreach (int itemId in currentPlayer.equippedItemIds)
        {
            ItemDataSO item = GetItemData(itemId);
            if (item != null)
            {
                AddItemBonusToStats(item, bonusStats);
            }
        }

        return bonusStats;
    }

    private void AddItemBonusToStats(ItemDataSO item, PlayerStats stats)
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
                stats.bonusAttack += item.value / 2;
                stats.bonusDefense += item.value / 2;
                break;
        }
    }

    #endregion

    #region Utility & Debug

    /// <summary>
    /// 인벤토리 정리 (중복 제거 등)
    /// </summary>
    public void OrganizeInventory()
    {
        if (currentPlayer == null) return;

        // 현재는 단순히 정렬만 (나중에 스택 시스템 구현시 확장)
        currentPlayer.inventoryItemIds.Sort();
        OnInventoryChanged?.Invoke();
        Debug.Log("인벤토리 정리 완료");
    }

    /// <summary>
    /// 인벤토리 상태 출력 (디버그용)
    /// </summary>
    [ContextMenu("인벤토리 상태 출력")]
    public void PrintInventoryStatus()
    {
        if (currentPlayer == null)
        {
            Debug.Log("현재 플레이어 없음");
            return;
        }

        Debug.Log($"=== {currentPlayer.playerName}의 인벤토리 ===");

        var itemCounts = GetInventoryItemCounts();
        Debug.Log($"인벤토리 아이템 ({itemCounts.Count}종류):");
        foreach (var kvp in itemCounts)
        {
            ItemDataSO item = GetItemData(kvp.Key);
            string itemName = item?.itemName ?? "알 수 없음";
            Debug.Log($"  {itemName} x{kvp.Value}");
        }

        Debug.Log($"장착된 아이템 ({currentPlayer.equippedItemIds.Count}개):");
        foreach (int itemId in currentPlayer.equippedItemIds)
        {
            ItemDataSO item = GetItemData(itemId);
            string itemName = item?.itemName ?? "알 수 없음";
            Debug.Log($"  {itemName}");
        }
    }

    /// <summary>
    /// 테스트용 아이템 추가
    /// </summary>
    [ContextMenu("테스트 아이템 추가")]
    public void AddTestItems()
    {
        if (itemDatabase == null || itemDatabase.allItems.Count == 0)
        {
            Debug.LogWarning("아이템 데이터베이스가 비어있습니다!");
            return;
        }

        // 각 타입별로 하나씩 추가
        var weaponItems = GetItemsByType(ItemType.Weapon);
        var shieldItems = GetItemsByType(ItemType.Shield);
        var consumableItems = GetItemsByType(ItemType.Consumable);

        if (weaponItems.Count > 0) AddItem(weaponItems[0].itemId);
        if (shieldItems.Count > 0) AddItem(shieldItems[0].itemId);
        if (consumableItems.Count > 0) AddItem(consumableItems[0].itemId, 3);

        Debug.Log("테스트 아이템 추가 완료!");
    }

    #endregion
}