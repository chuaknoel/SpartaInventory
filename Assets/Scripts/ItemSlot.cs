using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    [Header("=== ITEM SLOT ===")]

    [Header("UI Components")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private Image equippedBorderImage;
    [SerializeField] private GameObject equippedMarkObject;
    [SerializeField] private Button slotButton;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI quantityText; // 수량 표시용 (새로 추가)

    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color equippedColor = Color.green;
    [SerializeField] private Color weaponColor = Color.red;
    [SerializeField] private Color armorColor = Color.blue;
    [SerializeField] private Color consumableColor = Color.yellow;
    [SerializeField] private Color accessoryColor = Color.magenta;

    private ItemDataSO currentItemData;
    private bool isEquipped;
    private int quantity;
    private InventoryUI parentInventoryUI;

    private void Awake()
    {
        // 부모 InventoryUI 찾기
        parentInventoryUI = GetComponentInParent<InventoryUI>();

        // 버튼 클릭 이벤트 설정
        if (slotButton != null)
        {
            slotButton.onClick.AddListener(OnSlotClicked);
        }
    }

    public void SetupSlot(ItemDataSO itemData, bool equipped, int itemQuantity = 1)
    {
        currentItemData = itemData;
        isEquipped = equipped;
        quantity = itemQuantity;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (currentItemData == null) return;

        // 아이템 이름 설정
        if (itemNameText != null)
        {
            itemNameText.text = currentItemData.itemName;
        }

        // 수량 텍스트 설정
        if (quantityText != null)
        {
            if (quantity > 1)
            {
                quantityText.text = $"x{quantity}";
                quantityText.gameObject.SetActive(true);
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
        }

        // 아이템 아이콘 설정
        if (itemIconImage != null)
        {
            if (currentItemData.iconSprite != null)
            {
                // 실제 아이콘 스프라이트가 있으면 사용
                itemIconImage.sprite = currentItemData.iconSprite;
                itemIconImage.color = Color.white;
            }
            else
            {
                // 스프라이트가 없으면 타입별 색상으로 표시
                itemIconImage.color = GetItemTypeColor(currentItemData.itemType);
            }
        }

        // 장착 상태에 따른 UI 업데이트
        UpdateEquippedStatus();

        // 배경색 설정
        UpdateBackgroundColor();
    }

    private void UpdateEquippedStatus()
    {
        // 장착 테두리 표시/숨김
        if (equippedBorderImage != null)
        {
            equippedBorderImage.gameObject.SetActive(isEquipped);
            if (isEquipped)
            {
                equippedBorderImage.color = equippedColor;
            }
        }

        // 장착 마크 표시/숨김
        if (equippedMarkObject != null)
        {
            equippedMarkObject.SetActive(isEquipped);
        }

        // 아이템 이름 색상 변경
        if (itemNameText != null)
        {
            itemNameText.color = isEquipped ? equippedColor : Color.white;

            // 장착된 아이템은 굵게 표시
            itemNameText.fontStyle = isEquipped ? FontStyles.Bold : FontStyles.Normal;
        }
    }

    private void UpdateBackgroundColor()
    {
        if (backgroundImage == null) return;

        Color backgroundColor;

        if (isEquipped)
        {
            // 장착된 아이템은 장착 색상
            backgroundColor = equippedColor;
            backgroundColor.a = 0.3f; // 반투명
        }
        else
        {
            // 일반 아이템은 타입별 색상
            backgroundColor = GetItemTypeColor(currentItemData.itemType);
            backgroundColor.a = 0.1f; // 매우 연하게
        }

        backgroundImage.color = backgroundColor;
    }

    private Color GetItemTypeColor(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Weapon:
                return weaponColor;
            case ItemType.Armor:
            case ItemType.Helmet:
            case ItemType.Shield:
            case ItemType.Boots:
                return armorColor;
            case ItemType.Consumable:
                return consumableColor;
            case ItemType.Accessory:
                return accessoryColor;
            default:
                return normalColor;
        }
    }

    private void OnSlotClicked()
    {
        if (currentItemData == null || parentInventoryUI == null) return;

        // 클릭 효과 (간단한 스케일 애니메이션)
        StartCoroutine(ClickAnimation());

        // 부모 InventoryUI에 클릭 이벤트 전달
        parentInventoryUI.OnItemSlotClicked(currentItemData, isEquipped);
    }

    private System.Collections.IEnumerator ClickAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 0.9f;

        // 축소
        float duration = 0.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        // 복원
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    #region Tooltip System (미래 확장용)

    /// <summary>
    /// 마우스 오버시 툴팁 표시
    /// </summary>
    public void OnPointerEnter()
    {
        if (currentItemData == null) return;

        // TODO: 툴팁 시스템 구현
        string tooltipText = $"{currentItemData.itemName}\n{currentItemData.description}";
        if (currentItemData.value > 0 && currentItemData.itemType != ItemType.Consumable)
        {
            string statName = GetStatName(currentItemData.itemType);
            tooltipText += $"\n{statName} +{currentItemData.value}";
        }

        Debug.Log($"툴팁: {tooltipText}");
    }

    /// <summary>
    /// 마우스 나갈때 툴팁 숨김
    /// </summary>
    public void OnPointerExit()
    {
        // TODO: 툴팁 숨기기
    }

    private string GetStatName(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Weapon:
                return "공격력";
            case ItemType.Shield:
            case ItemType.Helmet:
            case ItemType.Armor:
            case ItemType.Boots:
                return "방어력";
            case ItemType.Accessory:
                return "공격력/방어력";
            default:
                return "능력치";
        }
    }

    #endregion

    #region Debug

    [ContextMenu("슬롯 정보 출력")]
    private void PrintSlotInfo()
    {
        if (currentItemData != null)
        {
            Debug.Log($"슬롯 정보: {currentItemData.itemName} x{quantity}, 장착여부: {isEquipped}");
            Debug.Log($"아이템 설명: {currentItemData.description}");
            Debug.Log($"아이템 타입: {currentItemData.itemType}, 값: {currentItemData.value}");
        }
        else
        {
            Debug.Log("슬롯이 비어있습니다.");
        }
    }

    [ContextMenu("강제 시각 업데이트")]
    private void ForceUpdateVisuals()
    {
        UpdateVisuals();
        Debug.Log("시각 요소 강제 업데이트 완료");
    }

    #endregion
}