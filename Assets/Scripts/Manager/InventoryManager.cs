using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("������ �����ͺ��̽�")]
    [SerializeField] private ItemDatabaseSO itemDatabase;

    // �̺�Ʈ
    public static event System.Action OnInventoryChanged;
    public static event System.Action OnEquipmentChanged;

    #region Unity Lifecycle

    private void Awake()
    {
        // DontDestroyOnLoad ����! ������ ������
        if (Instance == null)
        {
            Instance = this;
            ValidateDatabase();
            Debug.Log("InventoryManager �ʱ�ȭ (���� ��������)");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // ���� �ٲ� �� Instance ����
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
            Debug.LogError("ItemDatabase�� �Ҵ���� �ʾҽ��ϴ�!");
        }
        else
        {
            Debug.Log($"�κ��丮 �Ŵ��� �ʱ�ȭ �Ϸ� - {itemDatabase.allItems.Count}�� ������ �ε��");
        }
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

    #region Inventory Management (GameManager�� ����)

    /// <summary>
    /// �κ��丮�� ������ �߰� (GameManager�� ����)
    /// </summary>
    public bool AddItem(int itemId, int quantity = 1)
    {
        if (GameManager.Instance?.CurrentPlayer == null)
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

        // GameManager�� CurrentPlayer�� ���� �߰�
        for (int i = 0; i < quantity; i++)
        {
            GameManager.Instance.CurrentPlayer.inventoryItemIds.Add(itemId);
        }

        // ���� �˸�
        OnInventoryChanged?.Invoke();
        GameManager.Instance.NotifyPlayerDataChanged();

        Debug.Log($"������ �߰�: {item.itemName} x{quantity}");
        return true;
    }

    /// <summary>
    /// �κ��丮���� ������ ����
    /// </summary>
    public bool RemoveItem(int itemId, int quantity = 1)
    {
        if (GameManager.Instance?.CurrentPlayer == null) return false;

        ItemDataSO item = GetItemData(itemId);
        int removedCount = 0;

        // ������ ������ŭ ����
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
            Debug.Log($"������ ����: {item?.itemName ?? "�� �� ���� ������"} x{removedCount}");
            return true;
        }

        return false;
    }

    /// <summary>
    /// ������ ����
    /// </summary>
    public bool EquipItem(int itemId)
    {
        if (GameManager.Instance?.CurrentPlayer == null) return false;

        ItemDataSO item = GetItemData(itemId);
        if (item == null || !item.IsEquippable())
        {
            Debug.LogWarning($"������ �� ���� �������Դϴ�: {itemId}");
            return false;
        }

        PlayerData player = GameManager.Instance.CurrentPlayer;

        // ���� Ÿ���� ��� �̹� �����Ǿ� ������ ����
        UnequipItemByType(item.itemType);

        // �κ��丮���� �����ϰ� ���� ��Ͽ� �߰�
        if (player.inventoryItemIds.Remove(itemId))
        {
            player.equippedItemIds.Add(itemId);
            OnInventoryChanged?.Invoke();
            OnEquipmentChanged?.Invoke();
            GameManager.Instance.NotifyPlayerDataChanged();
            Debug.Log($"������ ����: {item.itemName}");
            return true;
        }

        return false;
    }

    /// <summary>
    /// ������ ���� ����
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
            Debug.Log($"������ ���� ����: {item?.itemName ?? "�� �� ���� ������"}");
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
    /// ������ ���� Ȯ��
    /// </summary>
    public int GetItemCount(int itemId)
    {
        if (GameManager.Instance?.CurrentPlayer == null) return 0;
        return GameManager.Instance.CurrentPlayer.inventoryItemIds.Count(id => id == itemId);
    }

    /// <summary>
    /// �κ��丮 ������ ������ ��ųʸ�
    /// </summary>
    public Dictionary<int, int> GetInventoryItemCounts()
    {
        if (GameManager.Instance?.CurrentPlayer == null) return new Dictionary<int, int>();

        return GameManager.Instance.CurrentPlayer.inventoryItemIds
            .GroupBy(id => id)
            .ToDictionary(group => group.Key, group => group.Count());
    }

    /// <summary>
    /// ������ ������ ID��
    /// </summary>
    public List<int> GetEquippedItemIds()
    {
        if (GameManager.Instance?.CurrentPlayer == null) return new List<int>();
        return new List<int>(GameManager.Instance.CurrentPlayer.equippedItemIds);
    }

    #endregion

    #region Stats Calculation

    /// <summary>
    /// ������ �����۵��� ���ʽ� ���� ���
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
        var armorItems = GetItemsByType(ItemType.Armor);
        var consumableItems = GetItemsByType(ItemType.Consumable);

        if (weaponItems.Count > 0) AddItem(weaponItems[0].itemId);
        if (armorItems.Count > 0) AddItem(armorItems[0].itemId);
        if (consumableItems.Count > 0) AddItem(consumableItems[0].itemId, 3);

        Debug.Log("�׽�Ʈ ������ �߰� �Ϸ�!");
    }

    #endregion
}