using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static GameManager Instance { get; private set; }

    [Header("Current Player")]
    public PlayerData currentPlayer;

    [Header("Item Database")]
    [SerializeField] private List<ItemData> itemDatabase = new List<ItemData>();

    // UI ������Ʈ�� �̺�Ʈ
    public static event System.Action OnPlayerDataChanged;
    public static event System.Action OnInventoryChanged;

    #region Unity Lifecycle

    private void Awake()
    {
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeItemDatabase();
            Debug.Log("GameManager �ʱ�ȭ �Ϸ�");
        }
        else
        {
            Debug.Log("GameManager �ߺ� ���� ���� - �ı�");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // ������ �� �α��� ������ �̵�
        if (SceneManager.GetActiveScene().name != "LoginScene")
        {
            Debug.Log("LoginScene���� �̵�");
            SceneManager.LoadScene("LoginScene");
        }
    }

    #endregion

    #region Player Management

    /// <summary>
    /// ���� �÷��̾� ����
    /// </summary>
    public void SetCurrentPlayer(PlayerData player)
    {
        currentPlayer = player;
        OnPlayerDataChanged?.Invoke();
        Debug.Log($"�÷��̾� ����: {player.playerName}");
    }

    /// <summary>
    /// �÷��̾� ������ ������Ʈ �˸�
    /// </summary>
    public void UpdatePlayerData()
    {
        OnPlayerDataChanged?.Invoke();
    }

    /// <summary>
    /// ���� �÷��̾ �ִ��� Ȯ��
    /// </summary>
    public bool HasCurrentPlayer()
    {
        return currentPlayer != null;
    }

    #endregion

    #region Scene Management

    /// <summary>
    /// �κ��丮 ������ �̵�
    /// </summary>
    public void LoadInventoryScene()
    {
        if (currentPlayer == null)
        {
            Debug.LogError("�÷��̾� �����Ͱ� ��� �κ��丮 ������ �̵��� �� �����ϴ�!");
            return;
        }

        Debug.Log("�κ��丮 ������ �̵�");
        SceneManager.LoadScene("InventoryScene");
    }

    /// <summary>
    /// �α��� ������ ���ư���
    /// </summary>
    public void LoadLoginScene()
    {
        currentPlayer = null;
        Debug.Log("�α��� ������ ���ư���");
        SceneManager.LoadScene("LoginScene");
    }

    #endregion

    #region Item Database

    /// <summary>
    /// ������ �����ͺ��̽� �ʱ�ȭ
    /// </summary>
    private void InitializeItemDatabase()
    {
        itemDatabase.Clear();

        // ���� �����۵� �߰�
        AddSampleItems();

        Debug.Log($"������ �����ͺ��̽� �ʱ�ȭ �Ϸ�: {itemDatabase.Count}�� ������");
    }

    /// <summary>
    /// ���� �����۵� �߰�
    /// </summary>
    private void AddSampleItems()
    {
        itemDatabase.Add(new ItemData(1, "��", "�⺻���� ��", ItemType.Weapon, 10));
        itemDatabase.Add(new ItemData(2, "����", "���� ����", ItemType.Shield, 8));
        itemDatabase.Add(new ItemData(3, "����", "ö�� ����", ItemType.Helmet, 15));
        itemDatabase.Add(new ItemData(4, "����", "���� ����", ItemType.Armor, 25));
        itemDatabase.Add(new ItemData(5, "����", "ü�� ȸ�� ����", ItemType.Consumable, 5));
        itemDatabase.Add(new ItemData(6, "��������", "���� ȸ�� ����", ItemType.Consumable, 7));
        itemDatabase.Add(new ItemData(7, "��ȭ", "���� ��ȭ", ItemType.Boots, 12));
        itemDatabase.Add(new ItemData(8, "����", "������ ����", ItemType.Accessory, 50));
    }

    /// <summary>
    /// ������ ID�� ������ ������ ��������
    /// </summary>
    public ItemData GetItemData(int itemId)
    {
        return itemDatabase.Find(item => item.itemId == itemId);
    }

    /// <summary>
    /// ��� ������ ������ ��������
    /// </summary>
    public List<ItemData> GetAllItems()
    {
        return new List<ItemData>(itemDatabase);
    }

    #endregion

    #region Inventory Management

    /// <summary>
    /// �κ��丮�� ������ �߰�
    /// </summary>
    public void AddItemToInventory(int itemId)
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("�÷��̾ ��� �������� �߰��� �� �����ϴ�.");
            return;
        }

        ItemData item = GetItemData(itemId);
        if (item == null)
        {
            Debug.LogWarning($"������ ID {itemId}�� ã�� �� �����ϴ�.");
            return;
        }

        currentPlayer.inventoryItemIds.Add(itemId);
        OnInventoryChanged?.Invoke();
        Debug.Log($"������ �߰�: {item.itemName}");
    }

    /// <summary>
    /// �κ��丮���� ������ ����
    /// </summary>
    public void RemoveItemFromInventory(int itemId)
    {
        if (currentPlayer == null) return;

        ItemData item = GetItemData(itemId);
        if (currentPlayer.inventoryItemIds.Remove(itemId))
        {
            OnInventoryChanged?.Invoke();
            Debug.Log($"������ ����: {item?.itemName ?? "�� �� ���� ������"}");
        }
    }

    /// <summary>
    /// ������ ����
    /// </summary>
    public void EquipItem(int itemId)
    {
        if (currentPlayer == null) return;

        ItemData item = GetItemData(itemId);
        if (item == null)
        {
            Debug.LogWarning($"�����Ϸ��� ������ ID {itemId}�� ã�� �� �����ϴ�.");
            return;
        }

        // �Һ�ǰ�� ������ �� ����
        if (item.itemType == ItemType.Consumable)
        {
            Debug.Log($"{item.itemName}�� ������ �� ���� �Һ�ǰ�Դϴ�.");
            return;
        }

        // ���� Ÿ���� ��� �̹� �����Ǿ� ������ ����
        UnequipItemByType(item.itemType);

        // �κ��丮���� �����ϰ� ���� ��Ͽ� �߰�
        if (currentPlayer.inventoryItemIds.Remove(itemId))
        {
            currentPlayer.equippedItemIds.Add(itemId);
            OnInventoryChanged?.Invoke();
            Debug.Log($"������ ����: {item.itemName}");
        }
        else
        {
            Debug.LogWarning($"�κ��丮�� {item.itemName}�� ��� ������ �� �����ϴ�.");
        }
    }

    /// <summary>
    /// ������ ���� ����
    /// </summary>
    public void UnequipItem(int itemId)
    {
        if (currentPlayer == null) return;

        ItemData item = GetItemData(itemId);
        if (currentPlayer.equippedItemIds.Remove(itemId))
        {
            currentPlayer.inventoryItemIds.Add(itemId);
            OnInventoryChanged?.Invoke();
            Debug.Log($"������ ���� ����: {item?.itemName ?? "�� �� ���� ������"}");
        }
    }

    /// <summary>
    /// Ư�� Ÿ���� ������ ������ ����
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

    #endregion

    #region Stats Calculation

    /// <summary>
    /// �÷��̾��� ���� ���� �������� (�⺻ ���� + ��� ���ʽ�)
    /// </summary>
    public PlayerStats GetCalculatedStats()
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("�÷��̾ ��� �⺻ ������ ��ȯ�մϴ�.");
            return new PlayerStats();
        }

        // �⺻ ���� (������ ����)
        PlayerStats stats = new PlayerStats
        {
            baseAttack = currentPlayer.baseAttack + (currentPlayer.level - 1) * 3,
            baseDefense = currentPlayer.baseDefense + (currentPlayer.level - 1) * 2,
            baseHealth = currentPlayer.baseHealth + (currentPlayer.level - 1) * 15,
            baseSpeed = currentPlayer.baseSpeed + (currentPlayer.level - 1) * 1
        };

        // ������ �����۵��� ���ʽ� ���
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
    /// ������ ���ʽ��� ���ȿ� �߰�
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
                // �׼������� ���ݷ°� ���� ��ο� ���ʽ�
                stats.bonusAttack += item.value / 2;
                stats.bonusDefense += item.value / 2;
                break;
        }
    }

    #endregion

    #region Debug & Test Methods

    [ContextMenu("�׽�Ʈ ������ �߰�")]
    public void AddTestItems()
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("�÷��̾ ��� �׽�Ʈ �������� �߰��� �� �����ϴ�.");
            return;
        }

        // ��� �������� �ϳ��� �߰�
        for (int i = 1; i <= 8; i++)
        {
            AddItemToInventory(i);
        }

        Debug.Log("��� �׽�Ʈ �������� �߰��Ǿ����ϴ�!");
    }

    [ContextMenu("���� ���� ���")]
    public void PrintCurrentState()
    {
        if (currentPlayer == null)
        {
            Debug.Log("���� �÷��̾� ����");
            return;
        }

        Debug.Log($"=== {currentPlayer.playerName}�� ���� ���� ===");
        Debug.Log($"����: {currentPlayer.level}");
        Debug.Log($"���: {currentPlayer.gold}");
        Debug.Log($"�κ��丮 ������ ��: {currentPlayer.inventoryItemIds.Count}");
        Debug.Log($"������ ������ ��: {currentPlayer.equippedItemIds.Count}");

        PlayerStats stats = GetCalculatedStats();
        Debug.Log($"�� ���ݷ�: {stats.TotalAttack} (�⺻: {stats.baseAttack} + ���ʽ�: {stats.bonusAttack})");
        Debug.Log($"�� ����: {stats.TotalDefense} (�⺻: {stats.baseDefense} + ���ʽ�: {stats.bonusDefense})");
    }

    #endregion
}