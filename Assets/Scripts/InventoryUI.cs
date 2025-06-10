using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("=== INVENTORY UI ===")]

    [Header("UI Components")]
    [SerializeField] private Transform slotContainer; // Content (ScrollView 안의)
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private TextMeshProUGUI inventoryTitleText;

    [Header("Slot Management")]
    private List<ItemSlot> currentSlots = new List<ItemSlot>();

    private void Start()
    {
        if (inventoryTitleText != null)
        {
            inventoryTitleText.text = "인벤토리";
        }
    }

    public void RefreshInventory()
    {
        if (GameManager.Instance == null || GameManager.Instance.currentPlayer == null)
        {
            Debug.LogWarning("플레이어 데이터가 없어서 인벤토리를 새로고침할 수 없습니다.");
            return;
        }

        // 기존 슬롯들 제거
        ClearSlots();

        PlayerData player = GameManager.Instance.currentPlayer;

        // 장착된 아이템들 먼저 표시
        CreateEquippedItemSlots(player.equippedItemIds);

        // 인벤토리 아이템들 표시
        CreateInventoryItemSlots(player.inventoryItemIds);

        Debug.Log($"인벤토리 새로고침 완료: 장착 {player.equippedItemIds.Count}개, 인벤토리 {player.inventoryItemIds.Count}개");
    }

    private void ClearSlots()
    {
        // 기존 슬롯들 제거
        foreach (ItemSlot slot in currentSlots)
        {
            if (slot != null && slot.gameObject != null)
            {
                DestroyImmediate(slot.gameObject);
            }
        }
        currentSlots.Clear();

        // 혹시 남은 자식 오브젝트들도 제거
        for (int i = slotContainer.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(slotContainer.GetChild(i).gameObject);
        }
    }

    private void CreateEquippedItemSlots(List<int> equippedItemIds)
    {
        foreach (int itemId in equippedItemIds)
        {
            CreateItemSlot(itemId, true); // 장착된 아이템
        }
    }

    private void CreateInventoryItemSlots(List<int> inventoryItemIds)
    {
        foreach (int itemId in inventoryItemIds)
        {
            CreateItemSlot(itemId, false); // 인벤토리 아이템
        }
    }

    private void CreateItemSlot(int itemId, bool isEquipped)
    {
        if (itemSlotPrefab == null)
        {
            Debug.LogError("ItemSlot 프리팹이 연결되지 않았습니다!");
            return;
        }

        // 아이템 데이터 가져오기
        ItemData itemData = GameManager.Instance.GetItemData(itemId);
        if (itemData == null)
        {
            Debug.LogWarning($"아이템 ID {itemId}에 대한 데이터를 찾을 수 없습니다.");
            return;
        }

        // 슬롯 생성
        GameObject slotObj = Instantiate(itemSlotPrefab, slotContainer);
        ItemSlot slot = slotObj.GetComponent<ItemSlot>();

        if (slot != null)
        {
            // 슬롯 설정
            slot.SetupSlot(itemData, isEquipped);
            currentSlots.Add(slot);
        }
        else
        {
            Debug.LogError("ItemSlot 컴포넌트가 프리팹에 없습니다!");
            Destroy(slotObj);
        }
    }

    public void OnItemSlotClicked(ItemData itemData, bool isEquipped)
    {
        if (GameManager.Instance == null) return;

        if (isEquipped)
        {
            // 장착 해제
            GameManager.Instance.UnequipItem(itemData.itemId);
            Debug.Log($"{itemData.itemName} 장착 해제");
        }
        else
        {
            // 장착 (장비 아이템만)
            if (IsEquippableItem(itemData.itemType))
            {
                GameManager.Instance.EquipItem(itemData.itemId);
                Debug.Log($"{itemData.itemName} 장착");
            }
            else
            {
                Debug.Log($"{itemData.itemName}은(는) 장착할 수 없는 아이템입니다.");
            }
        }

        // 인벤토리 새로고침
        RefreshInventory();
    }

    private bool IsEquippableItem(ItemType itemType)
    {
        return itemType != ItemType.Consumable;
    }

    #region Context Menu Test Methods

    [ContextMenu("인벤토리 새로고침")]
    public void RefreshInventoryDebug()
    {
        RefreshInventory();
    }

    [ContextMenu("테스트 아이템 추가")]
    public void AddTestItem()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddItemToInventory(Random.Range(1, 9));
        }
    }

    #endregion
}