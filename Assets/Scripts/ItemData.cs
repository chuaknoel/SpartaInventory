using UnityEngine;

// ������ Ÿ�� ������ (���� ���)
[System.Serializable]
public enum ItemType
{
    Weapon,      // ����
    Shield,      // ����  
    Helmet,      // ����
    Armor,       // ����
    Boots,       // �Ź�
    Accessory,   // �׼�����
    Consumable   // �Һ�ǰ
}

// ���� ItemData Ŭ���� (ȣȯ���� ���� ���������� ��� �������� ����)
// ���ο� �ý��ۿ����� ItemDataSO�� ����ϼ���
[System.Serializable]
public class ItemData
{
    public int itemId;
    public string itemName;
    public string description;
    public ItemType itemType;
    public int value;           // ���ݷ�/���� ���ʽ� �Ǵ� ����
    public Sprite iconSprite;   // ������ ������ (�ɼ�)

    // ������
    public ItemData(int id, string name, string desc, ItemType type, int val)
    {
        itemId = id;
        itemName = name;
        description = desc;
        itemType = type;
        value = val;
        iconSprite = null;
    }

    // �⺻ ������
    public ItemData()
    {
        itemId = 0;
        itemName = "";
        description = "";
        itemType = ItemType.Consumable;
        value = 0;
        iconSprite = null;
    }

    /// <summary>
    /// ItemDataSO�� ��ȯ (ȣȯ���� ����)
    /// </summary>
    public ItemDataSO ToItemDataSO()
    {
        var so = ScriptableObject.CreateInstance<ItemDataSO>();
        so.itemId = this.itemId;
        so.itemName = this.itemName;
        so.description = this.description;
        so.itemType = this.itemType;
        so.value = this.value;
        so.iconSprite = this.iconSprite;
        return so;
    }
}