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

    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color equippedColor = Color.green;
    [SerializeField] private Color weaponColor = Color.red;
    [SerializeField] private Color armorColor = Color.blue;
    [SerializeField] private Color consumableColor = Color.yellow;

    private ItemData currentItemData;
    private bool isEquipped;
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

    public void SetupSlot(ItemData itemData, bool equipped)
    {
        currentItemData = itemData;
        isEquipped = equipped;

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

        // 아이템 아이콘 설정 (현재는 기본 스프라이트 사용)
        if (itemIconImage != null)
        {
            // TODO: 실제 게임에서는 itemData.iconSprite 사용
            itemIconImage.color = GetItemTypeColor(currentItemData.itemType);
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
            equippedBorderImage.color = equippedColor;
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
        }
    }

    private void UpdateBackgroundColor()
    {
        if (backgroundImage == null) return;

        Color backgroundColor = normalColor;

        if (isEquipped)
        {
            backgroundColor = equippedColor;
            backgroundColor.a = 0.3f; // 반투명
        }
        else
        {
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
                return Color.magenta;
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

    #region Tooltip (미래 확장용)

    public void ShowTooltip()
    {
        // TODO: 툴팁 시스템 구현
        Debug.Log($"아이템 정보: {currentItemData.itemName} - {currentItemData.description}");
    }

    public void HideTooltip()
    {
        // TODO: 툴팁 숨기기
    }

    #endregion

    #region Debug

    [ContextMenu("슬롯 정보 출력")]
    private void PrintSlotInfo()
    {
        if (currentItemData != null)
        {
            Debug.Log($"슬롯 정보: {currentItemData.itemName}, 장착여부: {isEquipped}");
        }
        else
        {
            Debug.Log("슬롯이 비어있습니다.");
        }
    }

    #endregion
}