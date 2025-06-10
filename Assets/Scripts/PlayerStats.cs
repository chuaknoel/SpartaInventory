using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class PlayerStats
{
    [Header("�⺻ ����")]
    public int baseAttack;
    public int baseDefense;
    public int baseHealth;
    public int baseSpeed;

    [Header("��� ���ʽ�")]
    public int bonusAttack;
    public int bonusDefense;
    public int bonusHealth;
    public int bonusSpeed;

    // ���� ���� (�б� ���� ������Ƽ)
    public int TotalAttack => baseAttack + bonusAttack;
    public int TotalDefense => baseDefense + bonusDefense;
    public int TotalHealth => baseHealth + bonusHealth;
    public int TotalSpeed => baseSpeed + bonusSpeed;

    // �⺻ ������
    public PlayerStats()
    {
        baseAttack = 0;
        baseDefense = 0;
        baseHealth = 0;
        baseSpeed = 0;
        bonusAttack = 0;
        bonusDefense = 0;
        bonusHealth = 0;
        bonusSpeed = 0;
    }

    // ���� ����
    public void ResetBonusStats()
    {
        bonusAttack = 0;
        bonusDefense = 0;
        bonusHealth = 0;
        bonusSpeed = 0;
    }

    // ������ ���ʽ� �߰�
    public void AddItemBonus(ItemData item)
    {
        if (item == null) return;

        switch (item.itemType)
        {
            case ItemType.Weapon:
                bonusAttack += item.value;
                break;
            case ItemType.Shield:
            case ItemType.Armor:
            case ItemType.Helmet:
            case ItemType.Boots:
                bonusDefense += item.value;
                break;
            case ItemType.Accessory:
                bonusAttack += item.value / 2;
                bonusDefense += item.value / 2;
                break;
        }
    }
}