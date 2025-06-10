using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    [Header("기본 정보")]
    public string playerId = "";           // 플레이어 ID (추가됨)
    public string playerName = "";         // 플레이어 이름
    public string characterDescription = "용감한 모험가";
    public int level = 1;

    [Header("경험치")]
    public int currentExp = 0;
    public int maxExp = 100;

    [Header("재화")]
    public int gold = 1000;

    [Header("인벤토리")]
    public List<int> inventoryItemIds = new List<int>();
    public List<int> equippedItemIds = new List<int>();

    [Header("기본 스탯")]
    public int baseAttack = 10;
    public int baseDefense = 5;
    public int baseHealth = 100;
    public int baseSpeed = 10;

    // 생성자 (ID와 이름을 받음)
    public PlayerData(string id, string name)
    {
        playerId = id;
        playerName = name;
        level = 1;
        currentExp = 0;
        maxExp = 100;
        gold = 1000;
        characterDescription = "용감한 모험가";

        inventoryItemIds = new List<int>();
        equippedItemIds = new List<int>();

        baseAttack = 10;
        baseDefense = 5;
        baseHealth = 100;
        baseSpeed = 10;
    }

    // 생성자 (이름만 받음 - 기존 호환성)
    public PlayerData(string name)
    {
        playerId = name;      // ID와 이름을 같게 설정
        playerName = name;
        level = 1;
        currentExp = 0;
        maxExp = 100;
        gold = 1000;
        characterDescription = "용감한 모험가";

        inventoryItemIds = new List<int>();
        equippedItemIds = new List<int>();

        baseAttack = 10;
        baseDefense = 5;
        baseHealth = 100;
        baseSpeed = 10;
    }

    // 기본 생성자
    public PlayerData()
    {
        playerId = "Player";
        playerName = "Player";
        level = 1;
        currentExp = 0;
        maxExp = 100;
        gold = 1000;
        characterDescription = "용감한 모험가";

        inventoryItemIds = new List<int>();
        equippedItemIds = new List<int>();

        baseAttack = 10;
        baseDefense = 5;
        baseHealth = 100;
        baseSpeed = 10;
    }

    // 레벨업 체크
    public bool CanLevelUp()
    {
        return currentExp >= maxExp;
    }

    // 레벨업 실행
    public void LevelUp()
    {
        if (CanLevelUp())
        {
            level++;
            currentExp -= maxExp;
            maxExp += 50; // 레벨당 필요 경험치 증가

            // 레벨업시 스탯 증가
            baseAttack += 3;
            baseDefense += 2;
            baseHealth += 15;
            baseSpeed += 1;

            Debug.Log($"{playerName}이(가) 레벨 {level}이 되었습니다!");
        }
    }

    // 경험치 획득
    public void GainExperience(int exp)
    {
        currentExp += exp;

        // 연속 레벨업 처리
        while (CanLevelUp())
        {
            LevelUp();
        }
    }

    // 골드 변경
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