using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public string playerId;
    public int level;
    public int gold;
    public List<int> inventoryItemIds = new List<int>();
    public List<int> equippedItemIds = new List<int>();

    public PlayerData(string id)
    {
        playerId = id;
        level = 1;
        gold = 1000;

        // 시작 아이템들 (아이템 ID로 관리)
        inventoryItemIds.Add(1); // 검
        inventoryItemIds.Add(2); // 방패
        inventoryItemIds.Add(3); // 갑옷
        inventoryItemIds.Add(4); // 물약
    }

    public void EquipItem(int itemId)
    {
        if (inventoryItemIds.Contains(itemId))
        {
            inventoryItemIds.Remove(itemId);
            equippedItemIds.Add(itemId);
        }
    }

    public void UnequipItem(int itemId)
    {
        if (equippedItemIds.Contains(itemId))
        {
            equippedItemIds.Remove(itemId);
            inventoryItemIds.Add(itemId);
        }
    }

    public bool IsEquipped(int itemId)
    {
        return equippedItemIds.Contains(itemId);
    }
}
