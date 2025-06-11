using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "Item Database", menuName = "Game/Item Database")]
public class ItemDatabaseSO : ScriptableObject
{
    [Header("������ �����ͺ��̽�")]
    public List<ItemDataSO> allItems = new List<ItemDataSO>();

    [Header("����")]
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
    /// �ڵ����� ID �Ҵ� (Inspector���� ������ �ڵ� ����)
    /// </summary>
    private void AssignIds()
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            if (allItems[i] != null)
            {
                allItems[i].itemId = i + 1; // 1���� ����
            }
        }
    }

    /// <summary>
    /// ID�� ������ ������ ã��
    /// </summary>
    public ItemDataSO GetItemById(int itemId)
    {
        return allItems.FirstOrDefault(item => item != null && item.itemId == itemId);
    }

    /// <summary>
    /// �̸����� ������ ������ ã��
    /// </summary>
    public ItemDataSO GetItemByName(string itemName)
    {
        return allItems.FirstOrDefault(item => item != null && item.itemName == itemName);
    }

    /// <summary>
    /// Ÿ�Ժ� ������ ����Ʈ ��������
    /// </summary>
    public List<ItemDataSO> GetItemsByType(ItemType itemType)
    {
        return allItems.Where(item => item != null && item.itemType == itemType).ToList();
    }

    /// <summary>
    /// ��� ����� ������
    /// </summary>
    public List<ItemDataSO> GetWeapons()
    {
        return GetItemsByType(ItemType.Weapon);
    }

    /// <summary>
    /// ��� �� ������
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
    /// ��� �Һ�ǰ
    /// </summary>
    public List<ItemDataSO> GetConsumables()
    {
        return GetItemsByType(ItemType.Consumable);
    }

    /// <summary>
    /// ���� ������ ��� ������
    /// </summary>
    public List<ItemDataSO> GetEquippableItems()
    {
        return allItems.Where(item => item != null && item.IsEquippable()).ToList();
    }

    /// <summary>
    /// ���� ������ ��������
    /// </summary>
    public ItemDataSO GetRandomItem()
    {
        if (allItems.Count == 0) return null;

        var validItems = allItems.Where(item => item != null).ToList();
        if (validItems.Count == 0) return null;

        return validItems[Random.Range(0, validItems.Count)];
    }

    /// <summary>
    /// Ÿ�Ժ� ���� ������
    /// </summary>
    public ItemDataSO GetRandomItemByType(ItemType itemType)
    {
        var itemsOfType = GetItemsByType(itemType);
        if (itemsOfType.Count == 0) return null;

        return itemsOfType[Random.Range(0, itemsOfType.Count)];
    }

    /// <summary>
    /// �����ͺ��̽� ��ȿ�� �˻�
    /// </summary>
    [ContextMenu("�����ͺ��̽� ����")]
    public void ValidateDatabase()
    {
        Debug.Log("=== ������ �����ͺ��̽� ���� ===");

        int validItems = 0;
        int nullItems = 0;

        // null ������ üũ
        for (int i = 0; i < allItems.Count; i++)
        {
            if (allItems[i] == null)
            {
                nullItems++;
                Debug.LogWarning($"�ε��� {i}�� null ������ �߰�");
            }
            else
            {
                validItems++;
            }
        }

        // �ߺ� ID üũ
        var duplicateIds = allItems
            .Where(item => item != null)
            .GroupBy(item => item.itemId)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key);

        foreach (int duplicateId in duplicateIds)
        {
            Debug.LogError($"�ߺ��� ID �߰�: {duplicateId}");
        }

        // �� �̸� üũ
        var emptyNames = allItems
            .Where(item => item != null && string.IsNullOrEmpty(item.itemName))
            .ToList();

        foreach (var item in emptyNames)
        {
            Debug.LogWarning($"�� �̸��� ������ �߰�: ID {item.itemId}");
        }

        // Ÿ�Ժ� ���
        var typeStats = allItems
            .Where(item => item != null)
            .GroupBy(item => item.itemType)
            .ToDictionary(group => group.Key, group => group.Count());

        Debug.Log($"��ȿ�� ������: {validItems}��, Null ������: {nullItems}��");
        Debug.Log("=== Ÿ�Ժ� ��� ===");
        foreach (var stat in typeStats)
        {
            Debug.Log($"{stat.Key}: {stat.Value}��");
        }
    }

    /// <summary>
    /// ����� ���� ���
    /// </summary>
    private void LogDatabaseInfo()
    {
        if (allItems.Count == 0)
        {
            Debug.Log("������ �����ͺ��̽��� ����ֽ��ϴ�.");
            return;
        }

        Debug.Log($"������ �����ͺ��̽�: �� {allItems.Count}�� ����");
        int validCount = allItems.Count(item => item != null);
        Debug.Log($"��ȿ�� ������: {validCount}��");
    }

    /// <summary>
    /// �׽�Ʈ�� ������ �ڵ� ����
    /// </summary>
    [ContextMenu("�⺻ ������ ���� (������ ����)")]
    public void CreateBasicItems()
    {
#if UNITY_EDITOR
        // �� ����� �����Ϳ����� ��� ����
        Debug.Log("�⺻ �������� �����Ϸ��� �����Ϳ��� �������� �������ּ���.");
        Debug.Log("Create �� Game �� Item Data �޴��� ����ϼ���.");
#endif
    }
}
