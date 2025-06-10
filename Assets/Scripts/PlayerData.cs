using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    [Header("�⺻ ����")]
    public string playerId = "";           // �÷��̾� ID (�߰���)
    public string playerName = "";         // �÷��̾� �̸�
    public string characterDescription = "�밨�� ���谡";
    public int level = 1;

    [Header("����ġ")]
    public int currentExp = 0;
    public int maxExp = 100;

    [Header("��ȭ")]
    public int gold = 1000;

    [Header("�κ��丮")]
    public List<int> inventoryItemIds = new List<int>();
    public List<int> equippedItemIds = new List<int>();

    [Header("�⺻ ����")]
    public int baseAttack = 10;
    public int baseDefense = 5;
    public int baseHealth = 100;
    public int baseSpeed = 10;

    // ������ (ID�� �̸��� ����)
    public PlayerData(string id, string name)
    {
        playerId = id;
        playerName = name;
        level = 1;
        currentExp = 0;
        maxExp = 100;
        gold = 1000;
        characterDescription = "�밨�� ���谡";

        inventoryItemIds = new List<int>();
        equippedItemIds = new List<int>();

        baseAttack = 10;
        baseDefense = 5;
        baseHealth = 100;
        baseSpeed = 10;
    }

    // ������ (�̸��� ���� - ���� ȣȯ��)
    public PlayerData(string name)
    {
        playerId = name;      // ID�� �̸��� ���� ����
        playerName = name;
        level = 1;
        currentExp = 0;
        maxExp = 100;
        gold = 1000;
        characterDescription = "�밨�� ���谡";

        inventoryItemIds = new List<int>();
        equippedItemIds = new List<int>();

        baseAttack = 10;
        baseDefense = 5;
        baseHealth = 100;
        baseSpeed = 10;
    }

    // �⺻ ������
    public PlayerData()
    {
        playerId = "Player";
        playerName = "Player";
        level = 1;
        currentExp = 0;
        maxExp = 100;
        gold = 1000;
        characterDescription = "�밨�� ���谡";

        inventoryItemIds = new List<int>();
        equippedItemIds = new List<int>();

        baseAttack = 10;
        baseDefense = 5;
        baseHealth = 100;
        baseSpeed = 10;
    }

    // ������ üũ
    public bool CanLevelUp()
    {
        return currentExp >= maxExp;
    }

    // ������ ����
    public void LevelUp()
    {
        if (CanLevelUp())
        {
            level++;
            currentExp -= maxExp;
            maxExp += 50; // ������ �ʿ� ����ġ ����

            // �������� ���� ����
            baseAttack += 3;
            baseDefense += 2;
            baseHealth += 15;
            baseSpeed += 1;

            Debug.Log($"{playerName}��(��) ���� {level}�� �Ǿ����ϴ�!");
        }
    }

    // ����ġ ȹ��
    public void GainExperience(int exp)
    {
        currentExp += exp;

        // ���� ������ ó��
        while (CanLevelUp())
        {
            LevelUp();
        }
    }

    // ��� ����
    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        return false;
    }

    public void EarnGold(int amount)
    {
        gold += amount;
    }
}