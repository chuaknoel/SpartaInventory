using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class InventoryUI : MonoBehaviour
{
    [Header("=== INVENTORY UI ===")]

    [Header("UI Components")]
    [SerializeField] private Transform slotContainer; // Content (ScrollView 안의)
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private TextMeshProUGUI inventoryTitleText;

    [Header("필터링")]
    [SerializeField] private bool showEquippedItems = true;
    [SerializeField] private bool showInventoryItems = true;

    [Header("Slot Management")]
    private List<ItemSlot> currentSlots = new List<ItemSlot>();

    private void Start()
    {
        if (inventoryTitleText != null)
        {
            inventoryTitleText.text = "인벤토리";
        }
    }

    private void OnEnable()
    {
        // InventoryManager 이벤트 구독
        InventoryManager.OnInventoryChanged += RefreshInventory;
        InventoryManager.OnEquipmentChanged += RefreshInventory;
    }

    private void OnDisable()
    {
        // InventoryManager 이벤트 구독 해제
        InventoryManager.OnInventoryChanged -= RefreshInventory;
        InventoryManager.OnEquipmentChanged -= RefreshInventory;
    }

    public void RefreshInventory()
    {
        if (InventoryManager.Instance == null || GameManager.Instance?.CurrentPlayer == null)
        {
            Debug.LogWarning("InventoryManager나 플레이어 데이터가 없어서 인벤토리를 새로고침할 수 없습니다.");
            ClearSlots();
            return;
        }

        // 기존 슬롯들 제거
        ClearSlots();

        PlayerData player = GameManager.Instance.CurrentPlayer;

        // 장착된 아이템들 먼저 표시
        if (showEquippedItems)
        {
            CreateEquippedItemSlots(player.equippedItemIds);
        }

        // 인벤토리 아이템들 표시 (중복 수량 고려)
        if (showInventoryItems)
        {
            CreateInventoryItemSlots();
        }

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
            CreateItemSlot(itemId, true, 1); // 장착된 아이템은 항상 1개
        }
    }

    private void CreateInventoryItemSlots()
    {
        // 인벤토리의 아이템들을 종류별로 그룹화하여 표시
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
            Debug.LogError("ItemSlot 프리팹이 연결되지 않았습니다!");
            return;
        }

        // 아이템 데이터 가져오기
        ItemDataSO itemData = InventoryManager.Instance.GetItemData(itemId);
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
            slot.SetupSlot(itemData, isEquipped, quantity);
            currentSlots.Add(slot);
        }
        else
        {
            Debug.LogError("ItemSlot 컴포넌트가 프리팹에 없습니다!");
            Destroy(slotObj);
        }
    }

    public void OnItemSlotClicked(ItemDataSO itemData, bool isEquipped)
    {
        if (InventoryManager.Instance == null || itemData == null) return;

        if (isEquipped)
        {
            // 장착 해제
            InventoryManager.Instance.UnequipItem(itemData.itemId);
            Debug.Log($"{itemData.itemName} 장착 해제");
        }
        else
        {
            // 장착 (장착 가능한 아이템만)
            if (itemData.IsEquippable())
            {
                InventoryManager.Instance.EquipItem(itemData.itemId);
                Debug.Log($"{itemData.itemName} 장착");
            }
            else
            {
                Debug.Log($"{itemData.itemName}는(은) 장착할 수 없는 아이템입니다.");
                // 소비품이라면 사용 로직 추가 가능
                UseConsumableItem(itemData);
            }
        }
    }

    /// <summary>
    /// 소비품 사용 로직 (추후 확장)
    /// </summary>
    private void UseConsumableItem(ItemDataSO itemData)
    {
        if (itemData.itemType != ItemType.Consumable) return;

        // TODO: 소비품 사용 효과 구현
        Debug.Log($"{itemData.itemName}을(를) 사용했습니다!");

        // 일단 인벤토리에서 제거
        InventoryManager.Instance.RemoveItem(itemData.itemId, 1);
    }

    /// <summary>
    /// 아이템 타입별 필터링
    /// </summary>
    public void FilterByItemType(ItemType itemType)
    {
        // TODO: 필터링 기능 구현
        Debug.Log($"{itemType} 타입으로 필터링");
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
        if (InventoryManager.Instance != null)
        {
            // 랜덤 아이템 추가
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

    [ContextMenu("필터 토글 - 장착 아이템")]
    public void ToggleEquippedItemsFilter()
    {
        showEquippedItems = !showEquippedItems;
        RefreshInventory();
        Debug.Log($"장착 아이템 표시: {showEquippedItems}");
    }

    [ContextMenu("필터 토글 - 인벤토리 아이템")]
    public void ToggleInventoryItemsFilter()
    {
        showInventoryItems = !showInventoryItems;
        RefreshInventory();
        Debug.Log($"인벤토리 아이템 표시: {showInventoryItems}");
    }

    #endregion
}