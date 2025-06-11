using UnityEngine;

// 아이템 타입 열거형 (공통 사용)
[System.Serializable]
public enum ItemType
{
    Weapon,      // 무기
    Shield,      // 방패  
    Helmet,      // 투구
    Armor,       // 갑옷
    Boots,       // 신발
    Accessory,   // 액세서리
    Consumable   // 소비품
}

// 기존 ItemData 클래스 (호환성을 위해 유지하지만 사용 권장하지 않음)
// 새로운 시스템에서는 ItemDataSO를 사용하세요
[System.Serializable]
public class ItemData
{
    public int itemId;
    public string itemName;
    public string description;
    public ItemType itemType;
    public int value;           // 공격력/방어력 보너스 또는 가격
    public Sprite iconSprite;   // 아이템 아이콘 (옵션)

    // 생성자
    public ItemData(int id, string name, string desc, ItemType type, int val)
    {
        itemId = id;
        itemName = name;
        description = desc;
        itemType = type;
        value = val;
        iconSprite = null;
    }

    // 기본 생성자
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
    /// ItemDataSO로 변환 (호환성을 위함)
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