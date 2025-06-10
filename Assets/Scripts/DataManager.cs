using UnityEngine;
using System.IO;

public static class DataManager
{
    private static string GetSavePath(string playerId)
    {
        return Path.Combine(Application.persistentDataPath, $"{playerId}_data.json");
    }

    public static void SavePlayerData(PlayerData playerData)
    {
        try
        {
            string json = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(GetSavePath(playerData.playerId), json);
            Debug.Log($"Player data saved for {playerData.playerId}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save player data: {e.Message}");
        }
    }

    public static PlayerData LoadPlayerData(string playerId)
    {
        try
        {
            string path = GetSavePath(playerId);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                Debug.Log($"Player data loaded for {playerId}");
                return data;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load player data: {e.Message}");
        }
        return null;
    }

    public static bool HasPlayerData(string playerId)
    {
        return File.Exists(GetSavePath(playerId));
    }
}
