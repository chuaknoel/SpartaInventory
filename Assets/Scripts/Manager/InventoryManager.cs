using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("아이템 데이터베이스")]
    [SerializeField] private ItemDatabaseSO itemDatabase;

    // 이벤트
    public static event System.Action OnInventoryChanged;
    public static event System.Action OnEquipmentChanged;

    #region Unity Lifecycle

    private void Awake()
    {
        // DontDestroyOnLoad 제거! 씬별로 생성됨
        if (Instance == null)
        {
            Instance = this;
            ValidateDatabase();
            Debug.Log("InventoryManager 초기화 (현재 씬에서만)");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // 씬이 바뀔 때 Instance 정리
        if (Instance == this)
        {
            Instance = null;
        }
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

    #region Inventory Management (GameManager와 연동)

    /// <summary>
    /// 인벤토리에 아이템 추가 (GameManager를 통해)
    /// </summary>
    public bool AddItem(int itemId, int quantity = 1)
    {
        if (GameManager.Instance?.CurrentPlayer == null)
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

        // GameManager의 CurrentPlayer에 직접 추가
        for (int i = 0; i < quantity; i++)
        {
            GameManager.Instance.CurrentPlayer.inventoryItemIds.Add(itemId);
        }

        // 변경 알림
        OnInventoryChanged?.Invoke();
        GameManager.Instance.NotifyPlayerDataChanged();

        Debug.Log($"아이템 추가: {item.itemName} x{quantity}");
        return true;
    }

    /// <summary>
    /// 인벤토리에서 아이템 제거
    /// </summary>
    public bool RemoveItem(int itemId, int quantity = 1)
    {
        if (GameManager.Instance?.CurrentPlayer == null) return false;

        ItemDataSO item = GetItemData(itemId);
        int removedCount = 0;

        // 지정된 수량만큼 제거
        for (int i = 0; i < quantity; i++)
        {
            if (GameManager.Instance.CurrentPlayer.inventoryItemIds.Remove(itemId))
            {
                removedCount++;
            }
            else
            {
                break;
            }
        }

        if (removedCount > 0)
        {
            OnInventoryChanged?.Invoke();
            GameManager.Instance.NotifyPlayerDataChanged();
            Debug.Log($"아이템 제거: {item?.itemName ?? "알 수 없는 아이템"} x{removedCount}");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 아이템 장착
    /// </summary>
    public bool EquipItem(int itemId)
    {
        if (GameManager.Instance?.CurrentPlayer == null) return false;

        ItemDataSO item = GetItemData(itemId);
        if (item == null || !item.IsEquippable())
        {
            Debug.LogWarning($"장착할 수 없는 아이템입니다: {itemId}");
            return false;
        }

        PlayerData player = GameManager.Instance.CurrentPlayer;

        // 같은 타입의 장비가 이미 장착되어 있으면 해제
        UnequipItemByType(item.itemType);

        // 인벤토리에서 제거하고 장착 목록에 추가
        if (player.inventoryItemIds.Remove(itemId))
        {
            player.equippedItemIds.Add(itemId);
            OnInventoryChanged?.Invoke();
            OnEquipmentChanged?.Invoke();
            GameManager.Instance.NotifyPlayerDataChanged();
            Debug.Log($"아이템 장착: {item.itemName}");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 아이템 장착 해제
    /// </summary>
    public bool UnequipItem(int itemId)
    {
        if (GameManager.Instance?.CurrentPlayer == null) return false;

        PlayerData player = GameManager.Instance.CurrentPlayer;
        ItemDataSO item = GetItemData(itemId);

        if (player.equippedItemIds.Remove(itemId))
        {
            player.inventoryItemIds.Add(itemId);
            OnInventoryChanged?.Invoke();
            OnEquipmentChanged?.Invoke();
            GameManager.Instance.NotifyPlayerDataChanged();
            Debug.Log($"아이템 장착 해제: {item?.itemName ?? "알 수 없는 아이템"}");
            return true;
        }

        return false;
    }

    private void UnequipItemByType(ItemType itemType)
    {
        if (GameManager.Instance?.CurrentPlayer == null) return;

        PlayerData player = GameManager.Instance.CurrentPlayer;

        for (int i = player.equippedItemIds.Count - 1; i >= 0; i--)
        {
            int equippedItemId = player.equippedItemIds[i];
            ItemDataSO equippedItem = GetItemData(equippedItemId);

            if (equippedItem != null && equippedItem.itemType == itemType)
            {
                UnequipItem(equippedItemId);
                break;
            }
        }
    }

    /// <summary>
    /// 아이템 수량 확인
    /// </summary>
    public int GetItemCount(int itemId)
    {
        if (GameManager.Instance?.CurrentPlayer == null) return 0;
        return GameManager.Instance.CurrentPlayer.inventoryItemIds.Count(id => id == itemId);
    }

    /// <summary>
    /// 인벤토리 아이템 수량별 딕셔너리
    /// </summary>
    public Dictionary<int, int> GetInventoryItemCounts()
    {
        if (GameManager.Instance?.CurrentPlayer == null) return new Dictionary<int, int>();

        return GameManager.Instance.CurrentPlayer.inventoryItemIds
            .GroupBy(id => id)
            .ToDictionary(group => group.Key, group => group.Count());
    }

    /// <summary>
    /// 장착된 아이템 ID들
    /// </summary>
    public List<int> GetEquippedItemIds()
    {
        if (GameManager.Instance?.CurrentPlayer == null) return new List<int>();
        return new List<int>(GameManager.Instance.CurrentPlayer.equippedItemIds);
    }

    #endregion

    #region Stats Calculation

    /// <summary>
    /// 장착된 아이템들의 보너스 스탯 계산
    /// </summary>
    public PlayerStats GetEquipmentBonusStats()
    {
        PlayerStats bonusStats = new PlayerStats();

        if (GameManager.Instance?.CurrentPlayer == null) return bonusStats;

        foreach (int itemId in GameManager.Instance.CurrentPlayer.equippedItemIds)
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

    #region Test Methods

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
        var armorItems = GetItemsByType(ItemType.Armor);
        var consumableItems = GetItemsByType(ItemType.Consumable);

        if (weaponItems.Count > 0) AddItem(weaponItems[0].itemId);
        if (armorItems.Count > 0) AddItem(armorItems[0].itemId);
        if (consumableItems.Count > 0) AddItem(consumableItems[0].itemId, 3);

        Debug.Log("테스트 아이템 추가 완료!");
    }

    #endregion
}