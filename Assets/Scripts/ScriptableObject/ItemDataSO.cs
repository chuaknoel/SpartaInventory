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
        /// <summary>
        /// 장착 가능한 아이템인지 확인
        /// </summary>
    }
    public bool IsEquippable()
    {
        return itemType != ItemType.Consumable;
    }

    /// <summary>
    /// 소비 가능한 아이템인지 확인
    /// </summary>
    public bool IsConsumable()
    {
        return itemType == ItemType.Consumable;
    }

    /// <summary>
    /// 아이템 사용 (소비품용)
    /// </summary>
    public bool UseItem(PlayerData player)
    {
        if (!IsConsumable()) return false;

        switch (itemName.ToLower())
        {
            case "체력물약":
            case "hp포션":
                // 체력 회복 로직 (나중에 PlayerStats 구현시)
                Debug.Log($"{itemName} 사용: HP +{value}");
                return true;

            case "마나물약":
            case "mp포션":
                // 마나 회복 로직
                Debug.Log($"{itemName} 사용: MP +{value}");
                return true;

            case "경험치물약":
                player.currentExp += value;
                Debug.Log($"{itemName} 사용: 경험치 +{value}");
                return true;

            default:
                Debug.Log($"{itemName} 사용됨");
                return true;
        }
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
    /// 디버그용 정보 출력
    /// </summary>
    public override string ToString()
    {
        return $"{itemName} (ID: {itemId}, Type: {itemType}, Value: {value})";
    }
}