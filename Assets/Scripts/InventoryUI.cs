using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class InventoryUI : MonoBehaviour
{
    [Header("=== INVENTORY UI ===")]

    [Header("UI Components")]
    [SerializeField] private Transform slotContainer; // Content (ScrollView ����)
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private TextMeshProUGUI inventoryTitleText;

    [Header("���͸�")]
    [SerializeField] private bool showEquippedItems = true;
    [SerializeField] private bool showInventoryItems = true;

    [Header("Slot Management")]
    private List<ItemSlot> currentSlots = new List<ItemSlot>();

    private void Start()
    {
        if (inventoryTitleText != null)
        {
            inventoryTitleText.text = "�κ��丮";
        }
    }

    private void OnEnable()
    {
        // InventoryManager �̺�Ʈ ����
        InventoryManager.OnInventoryChanged += RefreshInventory;
        InventoryManager.OnEquipmentChanged += RefreshInventory;
    }

    private void OnDisable()
    {
        // InventoryManager �̺�Ʈ ���� ����
        InventoryManager.OnInventoryChanged -= RefreshInventory;
        InventoryManager.OnEquipmentChanged -= RefreshInventory;
    }

    public void RefreshInventory()
    {
        if (InventoryManager.Instance == null || GameManager.Instance?.CurrentPlayer == null)
        {
            Debug.LogWarning("InventoryManager�� �÷��̾� �����Ͱ� ��� �κ��丮�� ���ΰ�ħ�� �� �����ϴ�.");
            ClearSlots();
            return;
        }

        // ���� ���Ե� ����
        ClearSlots();

        PlayerData player = GameManager.Instance.CurrentPlayer;

        // ������ �����۵� ���� ǥ��
        if (showEquippedItems)
        {
            CreateEquippedItemSlots(player.equippedItemIds);
        }

        // �κ��丮 �����۵� ǥ�� (�ߺ� ���� ���)
        if (showInventoryItems)
        {
            CreateInventoryItemSlots();
        }

        Debug.Log($"�κ��丮 ���ΰ�ħ �Ϸ�: ���� {player.equippedItemIds.Count}��, �κ��丮 {player.inventoryItemIds.Count}��");
    }

    private void ClearSlots()
    {
        // ���� ���Ե� ����
        foreach (ItemSlot slot in currentSlots)
        {
            if (slot != null && slot.gameObject != null)
            {
                DestroyImmediate(slot.gameObject);
            }
        }
        currentSlots.Clear();

        // Ȥ�� ���� �ڽ� ������Ʈ�鵵 ����
        for (int i = slotContainer.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(slotContainer.GetChild(i).gameObject);
        }
    }

    private void CreateEquippedItemSlots(List<int> equippedItemIds)
    {
        foreach (int itemId in equippedItemIds)
        {
            CreateItemSlot(itemId, true, 1); // ������ �������� �׻� 1��
        }
    }

    private void CreateInventoryItemSlots()
    {
        // �κ��丮�� �����۵��� �������� �׷�ȭ�Ͽ� ǥ��
        var itemCounts = InventoryManager.Instance.GetInventoryItemCounts();

        foreach (var kvp in itemCounts)
        {
            CreateItemSlot(kvp.Key, false, kvp.Value);
        }
    }

    private void CreateItemSlot(int itemId, bool isEquipped, int quantity = 1)
    {
        if (itemSlotPrefab == null)
        {
            Debug.LogError("ItemSlot �������� ������� �ʾҽ��ϴ�!");
            return;
        }

        // ������ ������ ��������
        ItemDataSO itemData = InventoryManager.Instance.GetItemData(itemId);
        if (itemData == null)
        {
            Debug.LogWarning($"������ ID {itemId}�� ���� �����͸� ã�� �� �����ϴ�.");
            return;
        }

        // ���� ����
        GameObject slotObj = Instantiate(itemSlotPrefab, slotContainer);
        ItemSlot slot = slotObj.GetComponent<ItemSlot>();

        if (slot != null)
        {
            // ���� ����
            slot.SetupSlot(itemData, isEquipped, quantity);
            currentSlots.Add(slot);
        }
        else
        {
            Debug.LogError("ItemSlot ������Ʈ�� �����տ� �����ϴ�!");
            Destroy(slotObj);
        }
    }

    public void OnItemSlotClicked(ItemDataSO itemData, bool isEquipped)
    {
        if (InventoryManager.Instance == null || itemData == null) return;

        if (isEquipped)
        {
            // ���� ����
            InventoryManager.Instance.UnequipItem(itemData.itemId);
            Debug.Log($"{itemData.itemName} ���� ����");
        }
        else
        {
            // ���� (���� ������ �����۸�)
            if (itemData.IsEquippable())
            {
                InventoryManager.Instance.EquipItem(itemData.itemId);
                Debug.Log($"{itemData.itemName} ����");
            }
            else
            {
                Debug.Log($"{itemData.itemName}��(��) ������ �� ���� �������Դϴ�.");
                // �Һ�ǰ�̶�� ��� ���� �߰� ����
                UseConsumableItem(itemData);
            }
        }
    }

    /// <summary>
    /// �Һ�ǰ ��� ���� (���� Ȯ��)
    /// </summary>
    private void UseConsumableItem(ItemDataSO itemData)
    {
        if (itemData.itemType != ItemType.Consumable) return;

        // TODO: �Һ�ǰ ��� ȿ�� ����
        Debug.Log($"{itemData.itemName}��(��) ����߽��ϴ�!");

        // �ϴ� �κ��丮���� ����
        InventoryManager.Instance.RemoveItem(itemData.itemId, 1);
    }

    /// <summary>
    /// ������ Ÿ�Ժ� ���͸�
    /// </summary>
    public void FilterByItemType(ItemType itemType)
    {
        // TODO: ���͸� ��� ����
        Debug.Log($"{itemType} Ÿ������ ���͸�");
    }

    #region Context Menu Test Methods

    [ContextMenu("�κ��丮 ���ΰ�ħ")]
    public void RefreshInventoryDebug()
    {
        RefreshInventory();
    }

    [ContextMenu("�׽�Ʈ ������ �߰�")]
    public void AddTestItem()
    {
        if (InventoryManager.Instance != null)
        {
            // ���� ������ �߰�
            var allItems = InventoryManager.Instance.GetAllItems();
            if (allItems.Count > 0)
            {
                var randomItem = allItems[Random.Range(0, allItems.Count)];
                if (randomItem != null)
                {
                    InventoryManager.Instance.AddItem(randomItem.itemId);
                }
            }
        }
    }

    [ContextMenu("���� ��� - ���� ������")]
    public void ToggleEquippedItemsFilter()
    {
        showEquippedItems = !showEquippedItems;
        RefreshInventory();
        Debug.Log($"���� ������ ǥ��: {showEquippedItems}");
    }

    [ContextMenu("���� ��� - �κ��丮 ������")]
    public void ToggleInventoryItemsFilter()
    {
        showInventoryItems = !showInventoryItems;
        RefreshInventory();
        Debug.Log($"�κ��丮 ������ ǥ��: {showInventoryItems}");
    }

    #endregion
}