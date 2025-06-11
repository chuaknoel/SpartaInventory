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
    [SerializeField] private TextMeshProUGUI quantityText; // ���� ǥ�ÿ� (���� �߰�)

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
        // �θ� InventoryUI ã��
        parentInventoryUI = GetComponentInParent<InventoryUI>();

        // ��ư Ŭ�� �̺�Ʈ ����
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

        // ������ �̸� ����
        if (itemNameText != null)
        {
            itemNameText.text = currentItemData.itemName;
        }

        // ���� �ؽ�Ʈ ����
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

        // ������ ������ ����
        if (itemIconImage != null)
        {
            if (currentItemData.iconSprite != null)
            {
                // ���� ������ ��������Ʈ�� ������ ���
                itemIconImage.sprite = currentItemData.iconSprite;
                itemIconImage.color = Color.white;
            }
            else
            {
                // ��������Ʈ�� ������ Ÿ�Ժ� �������� ǥ��
                itemIconImage.color = GetItemTypeColor(currentItemData.itemType);
            }
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
            if (isEquipped)
            {
                equippedBorderImage.color = equippedColor;
            }
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

            // ������ �������� ���� ǥ��
            itemNameText.fontStyle = isEquipped ? FontStyles.Bold : FontStyles.Normal;
        }
    }

    private void UpdateBackgroundColor()
    {
        if (backgroundImage == null) return;

        Color backgroundColor;

        if (isEquipped)
        {
            // ������ �������� ���� ����
            backgroundColor = equippedColor;
            backgroundColor.a = 0.3f; // ������
        }
        else
        {
            // �Ϲ� �������� Ÿ�Ժ� ����
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
                return accessoryColor;
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

    #region Tooltip System (�̷� Ȯ���)

    /// <summary>
    /// ���콺 ������ ���� ǥ��
    /// </summary>
    public void OnPointerEnter()
    {
        if (currentItemData == null) return;

        // TODO: ���� �ý��� ����
        string tooltipText = $"{currentItemData.itemName}\n{currentItemData.description}";
        if (currentItemData.value > 0 && currentItemData.itemType != ItemType.Consumable)
        {
            string statName = GetStatName(currentItemData.itemType);
            tooltipText += $"\n{statName} +{currentItemData.value}";
        }

        Debug.Log($"����: {tooltipText}");
    }

    /// <summary>
    /// ���콺 ������ ���� ����
    /// </summary>
    public void OnPointerExit()
    {
        // TODO: ���� �����
    }

    private string GetStatName(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Weapon:
                return "���ݷ�";
            case ItemType.Shield:
            case ItemType.Helmet:
            case ItemType.Armor:
            case ItemType.Boots:
                return "����";
            case ItemType.Accessory:
                return "���ݷ�/����";
            default:
                return "�ɷ�ġ";
        }
    }

    #endregion

    #region Debug

    [ContextMenu("���� ���� ���")]
    private void PrintSlotInfo()
    {
        if (currentItemData != null)
        {
            Debug.Log($"���� ����: {currentItemData.itemName} x{quantity}, ��������: {isEquipped}");
            Debug.Log($"������ ����: {currentItemData.description}");
            Debug.Log($"������ Ÿ��: {currentItemData.itemType}, ��: {currentItemData.value}");
        }
        else
        {
            Debug.Log("������ ����ֽ��ϴ�.");
        }
    }

    [ContextMenu("���� �ð� ������Ʈ")]
    private void ForceUpdateVisuals()
    {
        UpdateVisuals();
        Debug.Log("�ð� ��� ���� ������Ʈ �Ϸ�");
    }

    #endregion
}