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

    private void OnEnable()
    {
        // GameManager와 InventoryManager 이벤트 구독
        GameManager.OnPlayerChanged += OnPlayerChanged;
        GameManager.OnPlayerDataUpdated += RefreshStats;
        InventoryManager.OnEquipmentChanged += RefreshStats;
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        GameManager.OnPlayerChanged -= OnPlayerChanged;
        GameManager.OnPlayerDataUpdated -= RefreshStats;
        InventoryManager.OnEquipmentChanged -= RefreshStats;
    }

    private void OnPlayerChanged(PlayerData newPlayer)
    {
        RefreshStats();
    }

    public void RefreshStats()
    {
        if (GameManager.Instance?.CurrentPlayer == null)
        {
            Debug.LogWarning("플레이어 데이터가 없어서 스탯을 새로고침할 수 없습니다.");
            ClearStats();
            return;
        }

        PlayerData player = GameManager.Instance.CurrentPlayer;
        PlayerStats stats = GameManager.Instance.GetPlayerTotalStats();

        UpdatePlayerInfo(player);
        UpdateStatsInfo(stats);
        UpdateEquippedItemsInfo();

        Debug.Log("스탯 UI 새로고침 완료");
    }

    private void ClearStats()
    {
        if (playerNameText != null) playerNameText.text = "플레이어 없음";
        if (attackText != null) attackText.text = "공격력: 0";
        if (defenseText != null) defenseText.text = "방어력: 0";
        if (healthText != null) healthText.text = "체력: 0";
        if (speedText != null) speedText.text = "속도: 0";
        if (equippedItemsText != null) equippedItemsText.text = "장착한 아이템: 없음";
    }

    private void UpdatePlayerInfo(PlayerData player)
    {
        if (playerNameText != null)
        {
            playerNameText.text = $"{player.playerName}의 스탯 (레벨 {player.level})";
        }
    }

    private void UpdateStatsInfo(PlayerStats stats)
    {
        // 공격력
        if (attackText != null)
        {
            if (stats.bonusAttack > 0)
            {
                attackText.text = $"공격력: {stats.baseAttack} + <color=green>{stats.bonusAttack}</color> = <color=yellow>{stats.TotalAttack}</color>";
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
                defenseText.text = $"방어력: {stats.baseDefense} + <color=green>{stats.bonusDefense}</color> = <color=yellow>{stats.TotalDefense}</color>";
            }
            else
            {
                defenseText.text = $"방어력: {stats.TotalDefense}";
            }
        }

        // 체력
        if (healthText != null)
        {
            if (stats.bonusHealth > 0)
            {
                healthText.text = $"체력: {stats.baseHealth} + <color=green>{stats.bonusHealth}</color> = <color=yellow>{stats.TotalHealth}</color>";
            }
            else
            {
                healthText.text = $"체력: {stats.TotalHealth}";
            }
        }

        // 속도
        if (speedText != null)
        {
            if (stats.bonusSpeed > 0)
            {
                speedText.text = $"속도: {stats.baseSpeed} + <color=green>{stats.bonusSpeed}</color> = <color=yellow>{stats.TotalSpeed}</color>";
            }
            else
            {
                speedText.text = $"속도: {stats.TotalSpeed}";
            }
        }
    }

    private void UpdateEquippedItemsInfo()
    {
        if (equippedItemsText == null) return;

        if (InventoryManager.Instance == null)
        {
            equippedItemsText.text = "장착한 아이템: 시스템 오류";
            return;
        }

        List<int> equippedItemIds = InventoryManager.Instance.GetEquippedItemIds();

        if (equippedItemIds.Count == 0)
        {
            equippedItemsText.text = "장착한 아이템: 없음";
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("장착한 아이템:");

        // 아이템 타입별로 분류해서 표시
        Dictionary<ItemType, List<ItemDataSO>> equippedByType = new Dictionary<ItemType, List<ItemDataSO>>();

        foreach (int itemId in equippedItemIds)
        {
            ItemDataSO item = InventoryManager.Instance.GetItemData(itemId);
            if (item != null)
            {
                if (!equippedByType.ContainsKey(item.itemType))
                {
                    equippedByType[item.itemType] = new List<ItemDataSO>();
                }
                equippedByType[item.itemType].Add(item);
            }
        }

        // 타입별로 표시
        foreach (var kvp in equippedByType)
        {
            string typeColor = GetItemTypeColorString(kvp.Key);
            sb.AppendLine($"<color={typeColor}>[{GetItemTypeDisplayName(kvp.Key)}]</color>");

            foreach (ItemDataSO item in kvp.Value)
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
        if (GameManager.Instance?.CurrentPlayer == null) return;

        PlayerStats stats = GameManager.Instance.GetPlayerTotalStats();
        PlayerData player = GameManager.Instance.CurrentPlayer;

        Debug.Log("=== 상세 스탯 정보 ===");
        Debug.Log($"플레이어: {player.playerName} (레벨 {player.level})");
        Debug.Log($"기본 공격력: {stats.baseAttack}");
        Debug.Log($"장비 보너스 공격력: +{stats.bonusAttack}");
        Debug.Log($"총 공격력: {stats.TotalAttack}");
        Debug.Log($"기본 방어력: {stats.baseDefense}");
        Debug.Log($"장비 보너스 방어력: +{stats.bonusDefense}");
        Debug.Log($"총 방어력: {stats.TotalDefense}");
        Debug.Log($"체력: {stats.TotalHealth}");
        Debug.Log($"속도: {stats.TotalSpeed}");

        // 장착된 아이템 상세 정보
        if (InventoryManager.Instance != null)
        {
            var equippedIds = InventoryManager.Instance.GetEquippedItemIds();
            Debug.Log($"장착된 아이템 ({equippedIds.Count}개):");
            foreach (int itemId in equippedIds)
            {
                ItemDataSO item = InventoryManager.Instance.GetItemData(itemId);
                if (item != null)
                {
                    Debug.Log($"  - {item.itemName} ({item.itemType}, +{item.value})");
                }
            }
        }
    }

    #endregion

    #region Level Up Effect (미래 확장용)

    /// <summary>
    /// 레벨업시 시각 효과
    /// </summary>
    public void PlayLevelUpEffect()
    {
        // TODO: 레벨업 파티클 효과, 사운드 등
        Debug.Log("레벨업 효과 재생!");

        // 간단한 텍스트 깜빡임 효과
        if (playerNameText != null)
        {
            StartCoroutine(BlinkText(playerNameText, Color.yellow, 1.0f));
        }
    }

    private System.Collections.IEnumerator BlinkText(TextMeshProUGUI text, Color blinkColor, float duration)
    {
        Color originalColor = text.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            text.color = Color.Lerp(originalColor, blinkColor, Mathf.PingPong(elapsed * 4, 1));
            elapsed += Time.deltaTime;
            yield return null;
        }

        text.color = originalColor;
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
        if (GameManager.Instance?.CurrentPlayer != null)
        {
            PlayerData player = GameManager.Instance.CurrentPlayer;
            player.level++;
            player.currentExp = 0;
            player.maxExp += 100;

            // 기본 스탯 증가
            player.baseAttack += 3;
            player.baseDefense += 2;
            player.baseHealth += 15;
            player.baseSpeed += 1;

            GameManager.Instance.NotifyPlayerDataChanged();
            PlayLevelUpEffect();

            Debug.Log($"{player.playerName}이(가) 레벨 {player.level}이 되었습니다!");
        }
    }

    [ContextMenu("레벨업 효과 테스트")]
    public void TestLevelUpEffect()
    {
        PlayLevelUpEffect();
    }

    #endregion
}