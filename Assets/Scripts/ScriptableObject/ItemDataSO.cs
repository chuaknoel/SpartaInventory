using UnityEngine;

// ItemType은 ItemData.cs에서 정의된 것을 사용

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item Data")]
public class ItemDataSO : ScriptableObject
{
    [Header("기본 정보")]
    public int itemId;
    public string itemName;
    [TextArea(2, 4)]
    public string description;
    public ItemType itemType;

    [Header("능력치")]
    public int value; // 공격력/방어력 보너스 또는 가격

    [Header("비주얼")]
    public Sprite iconSprite;
    public Color itemColor = Color.white;

    [Header("기타")]
    public bool isStackable = true; // 중복 소지 가능 여부
    public int maxStackSize = 99;   // 최대 스택 크기
    public int sellPrice = 0;       // 판매 가격

    [Header("아이템 설명 (자동 생성)")]
    [TextArea(3, 5)]
    [SerializeField] private string fullDescription;

    private void OnValidate()
    {
        // Inspector에서 수정할 때마다 설명 자동 업데이트
        UpdateFullDescription();
    }

    private void UpdateFullDescription()
    {
        fullDescription = $"[{GetItemTypeDisplayName()}] {itemName}\n";
        fullDescription += $"{description}\n";

        if (itemType != ItemType.Consumable && value > 0)
        {
            string statType = (itemType == ItemType.Weapon || itemType == ItemType.Accessory) ? "공격력" : "방어력";
            fullDescription += $"{statType} +{value}";
        }

        if (sellPrice > 0)
        {
            fullDescription += $"\n판매가: {sellPrice} 골드";
        }
    }

    private string GetItemTypeDisplayName()
    {
        switch (itemType)
        {
            case ItemType.Weapon: return "무기";
            case ItemType.Shield: return "방패";
            case ItemType.Helmet: return "투구";
            case ItemType.Armor: return "갑옷";
            case ItemType.Boots: return "신발";
            case ItemType.Accessory: return "액세서리";
            case ItemType.Consumable: return "소비품";
            default: return "기타";
        }
    }

    /// <summary>
    /// 이 아이템이 장착 가능한지 확인
    /// </summary>
    public bool IsEquippable()
    {
        return itemType != ItemType.Consumable;
    }

    /// <summary>
    /// 디버그용 정보 출력
    /// </summary>
    public override string ToString()
    {
        return $"{itemName} (ID: {itemId}, Type: {itemType}, Value: {value})";
    }
}