using UnityEngine;

// ItemType�� ItemData.cs���� ���ǵ� ���� ���

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item Data")]
public class ItemDataSO : ScriptableObject
{
    [Header("�⺻ ����")]
    public int itemId;
    public string itemName;
    [TextArea(2, 4)]
    public string description;
    public ItemType itemType;

    [Header("�ɷ�ġ")]
    public int value; // ���ݷ�/���� ���ʽ� �Ǵ� ����

    [Header("���־�")]
    public Sprite iconSprite;
    public Color itemColor = Color.white;

    [Header("��Ÿ")]
    public bool isStackable = true; // �ߺ� ���� ���� ����
    public int maxStackSize = 99;   // �ִ� ���� ũ��
    public int sellPrice = 0;       // �Ǹ� ����

    [Header("������ ���� (�ڵ� ����)")]
    [TextArea(3, 5)]
    [SerializeField] private string fullDescription;

    private void OnValidate()
    {
        // Inspector���� ������ ������ ���� �ڵ� ������Ʈ
        UpdateFullDescription();
    }

    private void UpdateFullDescription()
    {
        fullDescription = $"[{GetItemTypeDisplayName()}] {itemName}\n";
        fullDescription += $"{description}\n";

        if (itemType != ItemType.Consumable && value > 0)
        {
            string statType = (itemType == ItemType.Weapon || itemType == ItemType.Accessory) ? "���ݷ�" : "����";
            fullDescription += $"{statType} +{value}";
        }

        if (sellPrice > 0)
        {
            fullDescription += $"\n�ǸŰ�: {sellPrice} ���";
        }
    }

    private string GetItemTypeDisplayName()
    {
        switch (itemType)
        {
            case ItemType.Weapon: return "����";
            case ItemType.Shield: return "����";
            case ItemType.Helmet: return "����";
            case ItemType.Armor: return "����";
            case ItemType.Boots: return "�Ź�";
            case ItemType.Accessory: return "�׼�����";
            case ItemType.Consumable: return "�Һ�ǰ";
            default: return "��Ÿ";
        }
    }

    /// <summary>
    /// �� �������� ���� �������� Ȯ��
    /// </summary>
    public bool IsEquippable()
    {
        return itemType != ItemType.Consumable;
    }

    /// <summary>
    /// ����׿� ���� ���
    /// </summary>
    public override string ToString()
    {
        return $"{itemName} (ID: {itemId}, Type: {itemType}, Value: {value})";
    }
}