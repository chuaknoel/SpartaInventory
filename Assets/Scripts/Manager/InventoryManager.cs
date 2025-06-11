using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("������ �����ͺ��̽�")]
    [SerializeField] private ItemDatabaseSO itemDatabase;

    [Header("���� �÷��̾�")]
    private PlayerData currentPlayer;

    // �̺�Ʈ
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
        // GameManager �̺�Ʈ ����
        GameManager.OnPlayerChanged += OnPlayerChanged;
    }

    private void OnDisable()
    {
        // GameManager �̺�Ʈ ���� ����
        GameManager.OnPlayerChanged -= OnPlayerChanged;
    }

    #endregion

    #region Setup & Validation

    private void ValidateDatabase()
    {
        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabase�� �Ҵ���� �ʾҽ��ϴ�!");
        }
        else
        {
            Debug.Log($"�κ��丮 �Ŵ��� �ʱ�ȭ �Ϸ� - {itemDatabase.allItems.Count}�� ������ �ε��");
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
    /// ID�� ������ ������ ��������
    /// </summary>
    public ItemDataSO GetItemData(int itemId)
    {
        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabase�� �����ϴ�!");
            return null;
        }

        return itemDatabase.GetItemById(itemId);
    }

    /// <summary>
    /// ��� ������ ������ ��������
    /// </summary>
    public List<ItemDataSO> GetAllItems()
    {
        if (itemDatabase == null) return new List<ItemDataSO>();
        return new List<ItemDataSO>(itemDatabase.allItems);
    }

    /// <summary>
    /// Ÿ�Ժ� ������ ��������
    /// </summary>
    public List<ItemDataSO> GetItemsByType(ItemType itemType)
    {
        if (itemDatabase == null) return new List<ItemDataSO>();
        return itemDatabase.GetItemsByType(itemType);
    }

    #endregion

    #region Inventory Management

    /// <summary>
    /// �κ��丮�� ������ �߰�
    /// </summary>
    public bool AddItem(int itemId, int quantity = 1)
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("�÷��̾ ��� �������� �߰��� �� �����ϴ�.");
            return false;
        }

        ItemDataSO item = GetItemData(itemId);
        if (item == null)
        {
            Debug.LogWarning($"������ ID {itemId}�� ã�� �� �����ϴ�.");
            return false;
        }

        // ������ŭ �߰� (���߿� ���� �ý��� ������ Ȱ��)
        for (int i = 0; i < quantity; i++)
        {
            currentPlayer.inventoryItemIds.Add(itemId);
        }

        OnInventoryChanged?.Invoke();
        Debug.Log($"������ �߰�: {item.itemName} x{quantity}");
        return true;
    }

    /// <summary>
    /// �κ��丮���� ������ ����
    /// </summary>
    public bool RemoveItem(int itemId, int quantity = 1)
    {
        if (currentPlayer == null) return false;

        ItemDataSO item = GetItemData(itemId);
        int removedCount = 0;

        // ������ ������ŭ ����
        for (int i = 0; i < quantity; i++)
        {
            if (currentPlayer.inventoryItemIds.Remove(itemId))
            {
                removedCount++;
            }
            else
            {
                break; // �� �̻� ������ �������� ����
            }
        }

        if (removedCount > 0)
        {
            OnInventoryChanged?.Invoke();
            Debug.Log($"������ ����: {item?.itemName ?? "�� �� ���� ������"} x{removedCount}");
            return true;
        }

        return false;
    }

    /// <summary>
    /// �κ��丮�� Ư�� �������� �� �� �ִ��� Ȯ��
    /// </summary>
    public int GetItemCount(int itemId)
    {
        if (currentPlayer == null) return 0;
        return currentPlayer.inventoryItemIds.Count(id => id == itemId);
    }

    /// <summary>
    /// �κ��丮�� �������� �ִ��� Ȯ��
    /// </summary>
    public bool HasItem(int itemId)
    {
        return GetItemCount(itemId) > 0;
    }

    /// <summary>
    /// �κ��丮�� ��� ������ ID �������� (�ߺ� ����)
    /// </summary>
    public List<int> GetInventoryItemIds()
    {
        if (currentPlayer == null) return new List<int>();
        return new List<int>(currentPlayer.inventoryItemIds);
    }

    /// <summary>
    /// �κ��丮�� ���� �����۵�� ���� ��������
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
    /// ������ ����
    /// </summary>
    public bool EquipItem(int itemId)
    {
        if (currentPlayer == null) return false;

        ItemDataSO item = GetItemData(itemId);
        if (item == null)
        {
            Debug.LogWarning($"�����Ϸ��� ������ ID {itemId}�� ã�� �� �����ϴ�.");
            return false;
        }

        if (!item.IsEquippable())
        {
            Debug.Log($"{item.itemName}�� ������ �� ���� �������Դϴ�.");
            return false;
        }

        // ���� Ÿ���� ��� �̹� �����Ǿ� ������ ����
        UnequipItemByType(item.itemType);

        // �κ��丮���� �����ϰ� ���� ��Ͽ� �߰�
        if (currentPlayer.inventoryItemIds.Remove(itemId))
        {
            currentPlayer.equippedItemIds.Add(itemId);
            OnInventoryChanged?.Invoke();
            OnEquipmentChanged?.Invoke();
            Debug.Log($"������ ����: {item.itemName}");
            return true;
        }
        else
        {
            Debug.LogWarning($"�κ��丮�� {item.itemName}�� ��� ������ �� �����ϴ�.");
            return false;
        }
    }

    /// <summary>
    /// ������ ���� ����
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
            Debug.Log($"������ ���� ����: {item?.itemName ?? "�� �� ���� ������"}");
            return true;
        }

        return false;
    }

    /// <summary>
    /// Ư�� Ÿ���� ������ ������ ����
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
                break; // ���� Ÿ���� �ϳ��� �����ǹǷ� �ϳ��� ����
            }
        }
    }

    /// <summary>
    /// �������� �����Ǿ� �ִ��� Ȯ��
    /// </summary>
    public bool IsItemEquipped(int itemId)
    {
        if (currentPlayer == null) return false;
        return currentPlayer.equippedItemIds.Contains(itemId);
    }

    /// <summary>
    /// ������ ��� ������ ID ��������
    /// </summary>
    public List<int> GetEquippedItemIds()
    {
        if (currentPlayer == null) return new List<int>();
        return new List<int>(currentPlayer.equippedItemIds);
    }

    /// <summary>
    /// Ư�� Ÿ���� ������ ������ ��������
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
    /// ������ �����۵��� ���ʽ� ���� ���
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
    /// �κ��丮 ���� (�ߺ� ���� ��)
    /// </summary>
    public void OrganizeInventory()
    {
        if (currentPlayer == null) return;

        // ����� �ܼ��� ���ĸ� (���߿� ���� �ý��� ������ Ȯ��)
        currentPlayer.inventoryItemIds.Sort();
        OnInventoryChanged?.Invoke();
        Debug.Log("�κ��丮 ���� �Ϸ�");
    }

    /// <summary>
    /// �κ��丮 ���� ��� (����׿�)
    /// </summary>
    [ContextMenu("�κ��丮 ���� ���")]
    public void PrintInventoryStatus()
    {
        if (currentPlayer == null)
        {
            Debug.Log("���� �÷��̾� ����");
            return;
        }

        Debug.Log($"=== {currentPlayer.playerName}�� �κ��丮 ===");

        var itemCounts = GetInventoryItemCounts();
        Debug.Log($"�κ��丮 ������ ({itemCounts.Count}����):");
        foreach (var kvp in itemCounts)
        {
            ItemDataSO item = GetItemData(kvp.Key);
            string itemName = item?.itemName ?? "�� �� ����";
            Debug.Log($"  {itemName} x{kvp.Value}");
        }

        Debug.Log($"������ ������ ({currentPlayer.equippedItemIds.Count}��):");
        foreach (int itemId in currentPlayer.equippedItemIds)
        {
            ItemDataSO item = GetItemData(itemId);
            string itemName = item?.itemName ?? "�� �� ����";
            Debug.Log($"  {itemName}");
        }
    }

    /// <summary>
    /// �׽�Ʈ�� ������ �߰�
    /// </summary>
    [ContextMenu("�׽�Ʈ ������ �߰�")]
    public void AddTestItems()
    {
        if (itemDatabase == null || itemDatabase.allItems.Count == 0)
        {
            Debug.LogWarning("������ �����ͺ��̽��� ����ֽ��ϴ�!");
            return;
        }

        // �� Ÿ�Ժ��� �ϳ��� �߰�
        var weaponItems = GetItemsByType(ItemType.Weapon);
        var shieldItems = GetItemsByType(ItemType.Shield);
        var consumableItems = GetItemsByType(ItemType.Consumable);

        if (weaponItems.Count > 0) AddItem(weaponItems[0].itemId);
        if (shieldItems.Count > 0) AddItem(shieldItems[0].itemId);
        if (consumableItems.Count > 0) AddItem(consumableItems[0].itemId, 3);

        Debug.Log("�׽�Ʈ ������ �߰� �Ϸ�!");
    }

    #endregion
}