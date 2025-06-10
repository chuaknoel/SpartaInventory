using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class StatsUI : MonoBehaviour
{
    [Header("STATS UI")]

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI equippedItemsText;

    public void RefreshStats()
    {
        if (GameManager.Instance == null || GameManager.Instance.currentPlayer == null)
        {
            Debug.LogWarning("플레이어 데이터가 없어서 스탯을 새로고침할 수 없습니다.");
            return;
        }

        PlayerData player = GameManager.Instance.currentPlayer;
        PlayerStats stats = GameManager.Instance.GetCalculatedStats();

        UpdatePlayerInfo(player);
        UpdateStatsInfo(stats);
        UpdateEquippedItemsInfo(player.equippedItemIds);

        Debug.Log("스탯 UI 새로고침 완료");
    }

    private void UpdatePlayerInfo(PlayerData player)
    {
        if (playerNameText != null)
        {
            playerNameText.text = $"{player.playerName}의 스탯";
        }
    }

    private void UpdateStatsInfo(PlayerStats stats)
    {
        // 공격력
        if (attackText != null)
        {
            if (stats.bonusAttack > 0)
            {
                attackText.text = $"공격력: {stats.baseAttack} + {stats.bonusAttack} = <color=green>{stats.TotalAttack}</color>";
            }
            else
            {
                attackText.text = $"공격력: {stats.TotalAttack}";
            }
        }

        // 방어력
        if (defenseText != null)
        {
            if (stats.bonusDefense > 0)
            {
                defenseText.text = $"방어력: {stats.baseDefense} + {stats.bonusDefense} = <color=green>{stats.TotalDefense}</color>";
            }
            else
            {
                defenseText.text = $"방어력: {stats.TotalDefense}";
            }
        }

        // 체력
        if (healthText != null)
        {
            healthText.text = $"체력: {stats.TotalHealth}";
        }

        // 속도
        if (speedText != null)
        {
            speedText.text = $"속도: {stats.TotalSpeed}";
        }
    }

    private void UpdateEquippedItemsInfo(List<int> equippedItemIds)
    {
        if (equippedItemsText == null) return;

        if (equippedItemIds.Count == 0)
        {
            equippedItemsText.text = "장착한 아이템: 없음";
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("장착한 아이템:");

        // 아이템 타입별로 분류해서 표시
        Dictionary<ItemType, List<ItemData>> equippedByType = new Dictionary<ItemType, List<ItemData>>();

        foreach (int itemId in equippedItemIds)
        {
            ItemData item = GameManager.Instance.GetItemData(itemId);
            if (item != null)
            {
                if (!equippedByType.ContainsKey(item.itemType))
                {
                    equippedByType[item.itemType] = new List<ItemData>();
                }
                equippedByType[item.itemType].Add(item);
            }
        }

        // 타입별로 표시
        foreach (var kvp in equippedByType)
        {
            string typeColor = GetItemTypeColorString(kvp.Key);
            sb.AppendLine($"<color={typeColor}>[{GetItemTypeDisplayName(kvp.Key)}]</color>");

            foreach (ItemData item in kvp.Value)
            {
                sb.AppendLine($"  • {item.itemName} (+{item.value})");
            }
        }

        equippedItemsText.text = sb.ToString();
    }

    private string GetItemTypeDisplayName(ItemType itemType)
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

    private string GetItemTypeColorString(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Weapon: return "#FF6B6B"; // 빨간색
            case ItemType.Shield:
            case ItemType.Helmet:
            case ItemType.Armor:
            case ItemType.Boots: return "#4ECDC4"; // 청록색
            case ItemType.Accessory: return "#45B7D1"; // 파란색
            case ItemType.Consumable: return "#F9CA24"; // 노란색
            default: return "white";
        }
    }

    #region Bonus Calculation Display

    public void ShowDetailedStats()
    {
        if (GameManager.Instance?.currentPlayer == null) return;

        PlayerStats stats = GameManager.Instance.GetCalculatedStats();

        Debug.Log("=== 상세 스탯 정보 ===");
        Debug.Log($"기본 공격력: {stats.baseAttack}");
        Debug.Log($"장비 보너스 공격력: +{stats.bonusAttack}");
        Debug.Log($"총 공격력: {stats.TotalAttack}");
        Debug.Log($"기본 방어력: {stats.baseDefense}");
        Debug.Log($"장비 보너스 방어력: +{stats.bonusDefense}");
        Debug.Log($"총 방어력: {stats.TotalDefense}");
        Debug.Log($"체력: {stats.TotalHealth}");
        Debug.Log($"속도: {stats.TotalSpeed}");
    }

    #endregion

    #region Context Menu Methods

    [ContextMenu("스탯 새로고침")]
    public void RefreshStatsDebug()
    {
        RefreshStats();
    }

    [ContextMenu("상세 스탯 표시")]
    public void ShowDetailedStatsDebug()
    {
        ShowDetailedStats();
    }

    [ContextMenu("레벨업 테스트")]
    public void TestLevelUp()
    {
        if (GameManager.Instance?.currentPlayer != null)
        {
            GameManager.Instance.currentPlayer.level++;
            GameManager.Instance.currentPlayer.currentExp = 0;
            GameManager.Instance.currentPlayer.maxExp += 100;

            RefreshStats();
            Debug.Log("레벨업!");
        }
    }

    #endregion
}
