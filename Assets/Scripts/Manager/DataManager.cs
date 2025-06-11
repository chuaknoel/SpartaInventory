using UnityEngine;
using System.IO;

public static class DataManager
{
    /// <summary>
    /// �÷��̾� ID�� ���� ��� ����
    /// </summary>
    private static string GetSavePath(string playerId)
    {
        return Path.Combine(Application.persistentDataPath, $"{playerId}_data.json");
    }

    /// <summary>
    /// �÷��̾� ������ ����
    /// </summary>
    public static void SavePlayerData(PlayerData playerData)
    {
        try
        {
            string json = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(GetSavePath(playerData.playerId), json);
            Debug.Log($"�÷��̾� ������ ���� �Ϸ�: {playerData.playerName} (ID: {playerData.playerId})");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"�÷��̾� ������ ���� ����: {e.Message}");
        }
    }

    /// <summary>
    /// �÷��̾� ������ �ε�
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

                // Ȥ�� playerId�� ����� �������� �ʾҴٸ� ����
                if (string.IsNullOrEmpty(data.playerId))
                {
                    data.playerId = playerId;
                }

                Debug.Log($"�÷��̾� ������ �ε� �Ϸ�: {data.playerName} (ID: {data.playerId})");
                return data;
            }
            else
            {
                Debug.LogWarning($"�÷��̾� ������ ������ ã�� �� �����ϴ�: {playerId}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"�÷��̾� ������ �ε� ����: {e.Message}");
        }
        return null;
    }

    /// <summary>
    /// �÷��̾� ������ ���� ���� Ȯ��
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
    /// �÷��̾� ������ ����
    /// </summary>
    public static bool DeletePlayerData(string playerId)
    {
        try
        {
            string path = GetSavePath(playerId);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"�÷��̾� ������ ���� �Ϸ�: {playerId}");
                return true;
            }
            else
            {
                Debug.LogWarning($"������ �÷��̾� �����Ͱ� �����ϴ�: {playerId}");
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"�÷��̾� ������ ���� ����: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// ����� ��� �÷��̾� ID ��� ��������
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
            Debug.LogError($"�÷��̾� ID ��� �������� ����: {e.Message}");
            return new string[0];
        }
    }
}