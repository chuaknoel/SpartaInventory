using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "Item Database", menuName = "Game/Item Database")]
public class ItemDatabaseSO : ScriptableObject
{
    [Header("아이템 데이터베이스")]
    public List<ItemDataSO> allItems = new List<ItemDataSO>();

    [Header("설정")]
    [SerializeField] private bool autoAssignIds = true;
    [SerializeField] private bool showDebugInfo = false;

    private void OnValidate()
    {
        if (autoAssignIds)
        {
            AssignIds();
        }

        if (showDebugInfo)
        {
            LogDatabaseInfo();
        }
    }

    /// <summary>
    /// 자동으로 ID 할당 (Inspector에서 수정시 자동 실행)
    /// </summary>
    private void AssignIds()
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            if (allItems[i] != null)
            {
                allItems[i].itemId = i + 1; // 1부터 시작
            }
        }
    }

    /// <summary>
    /// ID로 아이템 데이터 찾기
    /// </summary>
    public ItemDataSO GetItemById(int itemId)
    {
        return allItems.FirstOrDefault(item => item != null && item.itemId == itemId);
    }

    /// <summary>
    /// 이름으로 아이템 데이터 찾기
    /// </summary>
    public ItemDataSO GetItemByName(string itemName)
    {
        return allItems.FirstOrDefault(item => item != null && item.itemName == itemName);
    }

    /// <summary>
    /// 타입별 아이템 리스트 가져오기
    /// </summary>
    public List<ItemDataSO> GetItemsByType(ItemType itemType)
    {
        return allItems.Where(item => item != null && item.itemType == itemType).ToList();
    }

    /// <summary>
    /// 모든 무기류 아이템
    /// </summary>
    public List<ItemDataSO> GetWeapons()
    {
        return GetItemsByType(ItemType.Weapon);
    }

    /// <summary>
    /// 모든 방어구 아이템
    /// </summary>
    public List<ItemDataSO> GetArmors()
    {
        var armors = new List<ItemDataSO>();
        armors.AddRange(GetItemsByType(ItemType.Shield));
        armors.AddRange(GetItemsByType(ItemType.Helmet));
        armors.AddRange(GetItemsByType(ItemType.Armor));
        armors.AddRange(GetItemsByType(ItemType.Boots));
        return armors;
    }

    /// <summary>
    /// 모든 소비품
    /// </summary>
    public List<ItemDataSO> GetConsumables()
    {
        return GetItemsByType(ItemType.Consumable);
    }

    /// <summary>
    /// 장착 가능한 모든 아이템
    /// </summary>
    public List<ItemDataSO> GetEquippableItems()
    {
        return allItems.Where(item => item != null && item.IsEquippable()).ToList();
    }

    /// <summary>
    /// 랜덤 아이템 가져오기
    /// </summary>
    public ItemDataSO GetRandomItem()
    {
        if (allItems.Count == 0) return null;

        var validItems = allItems.Where(item => item != null).ToList();
        if (validItems.Count == 0) return null;

        return validItems[Random.Range(0, validItems.Count)];
    }

    /// <summary>
    /// 타입별 랜덤 아이템
    /// </summary>
    public ItemDataSO GetRandomItemByType(ItemType itemType)
    {
        var itemsOfType = GetItemsByType(itemType);
        if (itemsOfType.Count == 0) return null;

        return itemsOfType[Random.Range(0, itemsOfType.Count)];
    }

    /// <summary>
    /// 데이터베이스 유효성 검사
    /// </summary>
    [ContextMenu("데이터베이스 검증")]
    public void ValidateDatabase()
    {
        Debug.Log("=== 아이템 데이터베이스 검증 ===");

        int validItems = 0;
        int nullItems = 0;

        // null 아이템 체크
        for (int i = 0; i < allItems.Count; i++)
        {
            if (allItems[i] == null)
            {
                nullItems++;
                Debug.LogWarning($"인덱스 {i}에 null 아이템 발견");
            }
            else
            {
                validItems++;
            }
        }

        // 중복 ID 체크
        var duplicateIds = allItems
            .Where(item => item != null)
            .GroupBy(item => item.itemId)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key);

        foreach (int duplicateId in duplicateIds)
        {
            Debug.LogError($"중복된 ID 발견: {duplicateId}");
        }

        // 빈 이름 체크
        var emptyNames = allItems
            .Where(item => item != null && string.IsNullOrEmpty(item.itemName))
            .ToList();

        foreach (var item in emptyNames)
        {
            Debug.LogWarning($"빈 이름의 아이템 발견: ID {item.itemId}");
        }

        // 타입별 통계
        var typeStats = allItems
            .Where(item => item != null)
            .GroupBy(item => item.itemType)
            .ToDictionary(group => group.Key, group => group.Count());

        Debug.Log($"유효한 아이템: {validItems}개, Null 아이템: {nullItems}개");
        Debug.Log("=== 타입별 통계 ===");
        foreach (var stat in typeStats)
        {
            Debug.Log($"{stat.Key}: {stat.Value}개");
        }
    }

    /// <summary>
    /// 디버그 정보 출력
    /// </summary>
    private void LogDatabaseInfo()
    {
        if (allItems.Count == 0)
        {
            Debug.Log("아이템 데이터베이스가 비어있습니다.");
            return;
        }

        Debug.Log($"아이템 데이터베이스: 총 {allItems.Count}개 슬롯");
        int validCount = allItems.Count(item => item != null);
        Debug.Log($"유효한 아이템: {validCount}개");
    }

    /// <summary>
    /// 테스트용 아이템 자동 생성
    /// </summary>
    [ContextMenu("기본 아이템 생성 (에디터 전용)")]
    public void CreateBasicItems()
    {
#if UNITY_EDITOR
        // 이 기능은 에디터에서만 사용 가능
        Debug.Log("기본 아이템을 생성하려면 에디터에서 수동으로 생성해주세요.");
        Debug.Log("Create → Game → Item Data 메뉴를 사용하세요.");
#endif
    }
}
