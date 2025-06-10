using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("=== INVENTORY UI ===")]

    [Header("UI Components")]
    [SerializeField] private Transform slotContainer; // Content (ScrollView ����)
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private TextMeshProUGUI inventoryTitleText;

    [Header("Slot Management")]
    private List<ItemSlot> currentSlots = new List<ItemSlot>();

    private void Start()
    {
        if (inventoryTitleText != null)
        {
            inventoryTitleText.text = "�κ��丮";
        }
    }

    public void RefreshInventory()
    {
        if (GameManager.Instance == null || GameManager.Instance.currentPlayer == null)
        {
            Debug.LogWarning("�÷��̾� �����Ͱ� ��� �κ��丮�� ���ΰ�ħ�� �� �����ϴ�.");
            return;
        }

        // ���� ���Ե� ����
        ClearSlots();

        PlayerData player = GameManager.Instance.currentPlayer;

        // ������ �����۵� ���� ǥ��
        CreateEquippedItemSlots(player.equippedItemIds);

        // �κ��丮 �����۵� ǥ��
        CreateInventoryItemSlots(player.inventoryItemIds);

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
            CreateItemSlot(itemId, true); // ������ ������
        }
    }

    private void CreateInventoryItemSlots(List<int> inventoryItemIds)
    {
        foreach (int itemId in inventoryItemIds)
        {
            CreateItemSlot(itemId, false); // �κ��丮 ������
        }
    }

    private void CreateItemSlot(int itemId, bool isEquipped)
    {
        if (itemSlotPrefab == null)
        {
            Debug.LogError("ItemSlot �������� ������� �ʾҽ��ϴ�!");
            return;
        }

        // ������ ������ ��������
        ItemData itemData = GameManager.Instance.GetItemData(itemId);
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
            slot.SetupSlot(itemData, isEquipped);
            currentSlots.Add(slot);
        }
        else
        {
            Debug.LogError("ItemSlot ������Ʈ�� �����տ� �����ϴ�!");
            Destroy(slotObj);
        }
    }

    public void OnItemSlotClicked(ItemData itemData, bool isEquipped)
    {
        if (GameManager.Instance == null) return;

        if (isEquipped)
        {
            // ���� ����
            GameManager.Instance.UnequipItem(itemData.itemId);
            Debug.Log($"{itemData.itemName} ���� ����");
        }
        else
        {
            // ���� (��� �����۸�)
            if (IsEquippableItem(itemData.itemType))
            {
                GameManager.Instance.EquipItem(itemData.itemId);
                Debug.Log($"{itemData.itemName} ����");
            }
            else
            {
                Debug.Log($"{itemData.itemName}��(��) ������ �� ���� �������Դϴ�.");
            }
        }

        // �κ��丮 ���ΰ�ħ
        RefreshInventory();
    }

    private bool IsEquippableItem(ItemType itemType)
    {
        return itemType != ItemType.Consumable;
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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddItemToInventory(Random.Range(1, 9));
        }
    }

    #endregion
}