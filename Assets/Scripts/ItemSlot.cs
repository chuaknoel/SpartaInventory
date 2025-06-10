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
        // �θ� InventoryUI ã��
        parentInventoryUI = GetComponentInParent<InventoryUI>();

        // ��ư Ŭ�� �̺�Ʈ ����
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

        // ������ �̸� ����
        if (itemNameText != null)
        {
            itemNameText.text = currentItemData.itemName;
        }

        // ������ ������ ���� (����� �⺻ ��������Ʈ ���)
        if (itemIconImage != null)
        {
            // TODO: ���� ���ӿ����� itemData.iconSprite ���
            itemIconImage.color = GetItemTypeColor(currentItemData.itemType);
        }

        // ���� ���¿� ���� UI ������Ʈ
        UpdateEquippedStatus();

        // ���� ����
        UpdateBackgroundColor();
    }

    private void UpdateEquippedStatus()
    {
        // ���� �׵θ� ǥ��/����
        if (equippedBorderImage != null)
        {
            equippedBorderImage.gameObject.SetActive(isEquipped);
            equippedBorderImage.color = equippedColor;
        }

        // ���� ��ũ ǥ��/����
        if (equippedMarkObject != null)
        {
            equippedMarkObject.SetActive(isEquipped);
        }

        // ������ �̸� ���� ����
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
            backgroundColor.a = 0.3f; // ������
        }
        else
        {
            backgroundColor = GetItemTypeColor(currentItemData.itemType);
            backgroundColor.a = 0.1f; // �ſ� ���ϰ�
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

        // Ŭ�� ȿ�� (������ ������ �ִϸ��̼�)
        StartCoroutine(ClickAnimation());

        // �θ� InventoryUI�� Ŭ�� �̺�Ʈ ����
        parentInventoryUI.OnItemSlotClicked(currentItemData, isEquipped);
    }

    private System.Collections.IEnumerator ClickAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 0.9f;

        // ���
        float duration = 0.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        // ����
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

    #region Tooltip (�̷� Ȯ���)

    public void ShowTooltip()
    {
        // TODO: ���� �ý��� ����
        Debug.Log($"������ ����: {currentItemData.itemName} - {currentItemData.description}");
    }

    public void HideTooltip()
    {
        // TODO: ���� �����
    }

    #endregion

    #region Debug

    [ContextMenu("���� ���� ���")]
    private void PrintSlotInfo()
    {
        if (currentItemData != null)
        {
            Debug.Log($"���� ����: {currentItemData.itemName}, ��������: {isEquipped}");
        }
        else
        {
            Debug.Log("������ ����ֽ��ϴ�.");
        }
    }

    #endregion
}