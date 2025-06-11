using UnityEngine;
using System.IO;

public static class DataManager
{
    /// <summary>
    /// 플레이어 ID로 저장 경로 생성
    /// </summary>
    private static string GetSavePath(string playerId)
    {
        return Path.Combine(Application.persistentDataPath, $"{playerId}_data.json");
    }

    /// <summary>
    /// 플레이어 데이터 저장
    /// </summary>
    public static void SavePlayerData(PlayerData playerData)
    {
        try
        {
            string json = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(GetSavePath(playerData.playerId), json);
            Debug.Log($"플레이어 데이터 저장 완료: {playerData.playerName} (ID: {playerData.playerId})");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"플레이어 데이터 저장 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 플레이어 데이터 로드
    /// </summary>
    public static PlayerData LoadPlayerData(string playerId)
    {
        try
        {
            string path = GetSavePath(playerId);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);

                // 혹시 playerId가 제대로 설정되지 않았다면 설정
                if (string.IsNullOrEmpty(data.playerId))
                {
                    data.playerId = playerId;
                }

                Debug.Log($"플레이어 데이터 로드 완료: {data.playerName} (ID: {data.playerId})");
                return data;
            }
            else
            {
                Debug.LogWarning($"플레이어 데이터 파일을 찾을 수 없습니다: {playerId}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"플레이어 데이터 로드 실패: {e.Message}");
        }
        return null;
    }

    /// <summary>
    /// 플레이어 데이터 존재 여부 확인
    /// </summary>
    public static bool HasPlayerData(string playerId)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            return false;
        }

        return File.Exists(GetSavePath(playerId));
    }

    /// <summary>
    /// 플레이어 데이터 삭제
    /// </summary>
    public static bool DeletePlayerData(string playerId)
    {
        try
        {
            string path = GetSavePath(playerId);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"플레이어 데이터 삭제 완료: {playerId}");
                return true;
            }
            else
            {
                Debug.LogWarning($"삭제할 플레이어 데이터가 없습니다: {playerId}");
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"플레이어 데이터 삭제 실패: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 저장된 모든 플레이어 ID 목록 가져오기
    /// </summary>
    public static string[] GetAllPlayerIds()
    {
        try
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath, "*_data.json");
            string[] playerIds = new string[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileNameWithoutExtension(files[i]);
                playerIds[i] = fileName.Replace("_data", "");
            }

            return playerIds;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"플레이어 ID 목록 가져오기 실패: {e.Message}");
            return new string[0];
        }
    }
}