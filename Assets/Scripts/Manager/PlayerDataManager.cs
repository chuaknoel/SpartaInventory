using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public static class PlayerDataManager
{
    private static readonly string SaveDirectory = "PlayerData";
    private static readonly string FileExtension = ".json";

    /// <summary>
    /// 저장 폴더 경로 가져오기
    /// </summary>
    private static string GetSaveDirectoryPath()
    {
        string path = Path.Combine(Application.persistentDataPath, SaveDirectory);

        // 폴더가 없으면 생성
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log($"플레이어 데이터 폴더 생성: {path}");
        }

        return path;
    }

    /// <summary>
    /// 플레이어 ID로 저장 경로 생성
    /// </summary>
    private static string GetSaveFilePath(string playerId)
    {
        return Path.Combine(GetSaveDirectoryPath(), $"{playerId}{FileExtension}");
    }

    /// <summary>
    /// 플레이어 데이터 저장
    /// </summary>
    public static bool SavePlayerData(PlayerData playerData)
    {
        if (playerData == null)
        {
            Debug.LogError("저장할 플레이어 데이터가 null입니다!");
            return false;
        }

        if (string.IsNullOrEmpty(playerData.playerId))
        {
            Debug.LogError("플레이어 ID가 비어있습니다!");
            return false;
        }

        try
        {
            // PlayerData를 JSON으로 직렬화
            string json = JsonUtility.ToJson(playerData, true);
            string filePath = GetSaveFilePath(playerData.playerId);

            // 파일에 저장
            File.WriteAllText(filePath, json);

            Debug.Log($"플레이어 데이터 저장 완료: {playerData.playerName} (ID: {playerData.playerId})");
            Debug.Log($"저장 경로: {filePath}");

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"플레이어 데이터 저장 실패: {e.Message}");
            Debug.LogError($"상세 오류: {e}");
            return false;
        }
    }

    /// <summary>
    /// 플레이어 데이터 로드
    /// </summary>
    public static PlayerData LoadPlayerData(string playerId)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogError("플레이어 ID가 비어있습니다!");
            return null;
        }

        try
        {
            string filePath = GetSaveFilePath(playerId);

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"플레이어 데이터 파일을 찾을 수 없습니다: {playerId}");
                return null;
            }

            // 파일에서 JSON 읽기
            string json = File.ReadAllText(filePath);

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError($"플레이어 데이터 파일이 비어있습니다: {playerId}");
                return null;
            }

            // JSON을 PlayerData로 역직렬화
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

            if (playerData == null)
            {
                Debug.LogError($"플레이어 데이터 역직렬화 실패: {playerId}");
                return null;
            }

            // 혹시 playerId가 제대로 설정되지 않았다면 설정
            if (string.IsNullOrEmpty(playerData.playerId))
            {
                playerData.playerId = playerId;
            }

            // 리스트가 null이면 초기화
            if (playerData.inventoryItemIds == null)
                playerData.inventoryItemIds = new List<int>();
            if (playerData.equippedItemIds == null)
                playerData.equippedItemIds = new List<int>();

            Debug.Log($"플레이어 데이터 로드 완료: {playerData.playerName} (ID: {playerData.playerId})");
            return playerData;
        }
        catch (Exception e)
        {
            Debug.LogError($"플레이어 데이터 로드 실패: {e.Message}");
            Debug.LogError($"상세 오류: {e}");
            return null;
        }
    }

    /// <summary>
    /// 플레이어 데이터 존재 여부 확인
    /// </summary>
    public static bool HasPlayerData(string playerId)
    {
        if (string.IsNullOrEmpty(playerId))
            return false;

        return File.Exists(GetSaveFilePath(playerId));
    }

    /// <summary>
    /// 플레이어 데이터 삭제
    /// </summary>
    public static bool DeletePlayerData(string playerId)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogError("플레이어 ID가 비어있습니다!");
            return false;
        }

        try
        {
            string filePath = GetSaveFilePath(playerId);

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"삭제할 플레이어 데이터가 없습니다: {playerId}");
                return false;
            }

            File.Delete(filePath);
            Debug.Log($"플레이어 데이터 삭제 완료: {playerId}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"플레이어 데이터 삭제 실패: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 저장된 모든 플레이어 ID 목록 가져오기
    /// </summary>
    public static List<string> GetAllPlayerIds()
    {
        List<string> playerIds = new List<string>();

        try
        {
            string saveDir = GetSaveDirectoryPath();

            if (!Directory.Exists(saveDir))
            {
                Debug.Log("플레이어 데이터 폴더가 없습니다.");
                return playerIds;
            }

            string[] files = Directory.GetFiles(saveDir, $"*{FileExtension}");

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (!string.IsNullOrEmpty(fileName))
                {
                    playerIds.Add(fileName);
                }
            }

            Debug.Log($"총 {playerIds.Count}명의 플레이어 데이터가 발견되었습니다.");
        }
        catch (Exception e)
        {
            Debug.LogError($"플레이어 ID 목록 가져오기 실패: {e.Message}");
        }

        return playerIds;
    }

    /// <summary>
    /// 플레이어 데이터 백업
    /// </summary>
    public static bool BackupPlayerData(string playerId)
    {
        if (!HasPlayerData(playerId))
        {
            Debug.LogWarning($"백업할 플레이어 데이터가 없습니다: {playerId}");
            return false;
        }

        try
        {
            string originalPath = GetSaveFilePath(playerId);
            string backupPath = Path.Combine(GetSaveDirectoryPath(), $"{playerId}_backup_{DateTime.Now:yyyyMMdd_HHmmss}{FileExtension}");

            File.Copy(originalPath, backupPath);
            Debug.Log($"플레이어 데이터 백업 완료: {backupPath}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"플레이어 데이터 백업 실패: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 저장 폴더 열기 (에디터에서만)
    /// </summary>
    public static void OpenSaveFolder()
    {
#if UNITY_EDITOR
        string path = GetSaveDirectoryPath();
        if (Directory.Exists(path))
        {
            System.Diagnostics.Process.Start(path);
        }
        else
        {
            Debug.Log($"저장 폴더가 없습니다: {path}");
        }
#endif
    }

    /// <summary>
    /// 플레이어 데이터 정보 출력 (디버그용)
    /// </summary>
    public static void PrintPlayerDataInfo(string playerId)
    {
        PlayerData data = LoadPlayerData(playerId);
        if (data == null)
        {
            Debug.Log($"플레이어 데이터를 찾을 수 없습니다: {playerId}");
            return;
        }

        Debug.Log($"=== {data.playerName}의 데이터 ===");
        Debug.Log($"ID: {data.playerId}");
        Debug.Log($"레벨: {data.level}");
        Debug.Log($"경험치: {data.currentExp}/{data.maxExp}");
        Debug.Log($"골드: {data.gold}");
        Debug.Log($"인벤토리 아이템: {data.inventoryItemIds.Count}개");
        Debug.Log($"장착된 아이템: {data.equippedItemIds.Count}개");
        Debug.Log($"기본 스탯 - 공격: {data.baseAttack}, 방어: {data.baseDefense}, 체력: {data.baseHealth}, 속도: {data.baseSpeed}");
    }
}
