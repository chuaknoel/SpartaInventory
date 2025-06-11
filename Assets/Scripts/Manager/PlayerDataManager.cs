using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public static class PlayerDataManager
{
    private static readonly string SaveDirectory = "PlayerData";
    private static readonly string FileExtension = ".json";

    /// <summary>
    /// ���� ���� ��� ��������
    /// </summary>
    private static string GetSaveDirectoryPath()
    {
        string path = Path.Combine(Application.persistentDataPath, SaveDirectory);

        // ������ ������ ����
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log($"�÷��̾� ������ ���� ����: {path}");
        }

        return path;
    }

    /// <summary>
    /// �÷��̾� ID�� ���� ��� ����
    /// </summary>
    private static string GetSaveFilePath(string playerId)
    {
        return Path.Combine(GetSaveDirectoryPath(), $"{playerId}{FileExtension}");
    }

    /// <summary>
    /// �÷��̾� ������ ����
    /// </summary>
    public static bool SavePlayerData(PlayerData playerData)
    {
        if (playerData == null)
        {
            Debug.LogError("������ �÷��̾� �����Ͱ� null�Դϴ�!");
            return false;
        }

        if (string.IsNullOrEmpty(playerData.playerId))
        {
            Debug.LogError("�÷��̾� ID�� ����ֽ��ϴ�!");
            return false;
        }

        try
        {
            // PlayerData�� JSON���� ����ȭ
            string json = JsonUtility.ToJson(playerData, true);
            string filePath = GetSaveFilePath(playerData.playerId);

            // ���Ͽ� ����
            File.WriteAllText(filePath, json);

            Debug.Log($"�÷��̾� ������ ���� �Ϸ�: {playerData.playerName} (ID: {playerData.playerId})");
            Debug.Log($"���� ���: {filePath}");

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"�÷��̾� ������ ���� ����: {e.Message}");
            Debug.LogError($"�� ����: {e}");
            return false;
        }
    }

    /// <summary>
    /// �÷��̾� ������ �ε�
    /// </summary>
    public static PlayerData LoadPlayerData(string playerId)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogError("�÷��̾� ID�� ����ֽ��ϴ�!");
            return null;
        }

        try
        {
            string filePath = GetSaveFilePath(playerId);

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"�÷��̾� ������ ������ ã�� �� �����ϴ�: {playerId}");
                return null;
            }

            // ���Ͽ��� JSON �б�
            string json = File.ReadAllText(filePath);

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError($"�÷��̾� ������ ������ ����ֽ��ϴ�: {playerId}");
                return null;
            }

            // JSON�� PlayerData�� ������ȭ
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

            if (playerData == null)
            {
                Debug.LogError($"�÷��̾� ������ ������ȭ ����: {playerId}");
                return null;
            }

            // Ȥ�� playerId�� ����� �������� �ʾҴٸ� ����
            if (string.IsNullOrEmpty(playerData.playerId))
            {
                playerData.playerId = playerId;
            }

            // ����Ʈ�� null�̸� �ʱ�ȭ
            if (playerData.inventoryItemIds == null)
                playerData.inventoryItemIds = new List<int>();
            if (playerData.equippedItemIds == null)
                playerData.equippedItemIds = new List<int>();

            Debug.Log($"�÷��̾� ������ �ε� �Ϸ�: {playerData.playerName} (ID: {playerData.playerId})");
            return playerData;
        }
        catch (Exception e)
        {
            Debug.LogError($"�÷��̾� ������ �ε� ����: {e.Message}");
            Debug.LogError($"�� ����: {e}");
            return null;
        }
    }

    /// <summary>
    /// �÷��̾� ������ ���� ���� Ȯ��
    /// </summary>
    public static bool HasPlayerData(string playerId)
    {
        if (string.IsNullOrEmpty(playerId))
            return false;

        return File.Exists(GetSaveFilePath(playerId));
    }

    /// <summary>
    /// �÷��̾� ������ ����
    /// </summary>
    public static bool DeletePlayerData(string playerId)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogError("�÷��̾� ID�� ����ֽ��ϴ�!");
            return false;
        }

        try
        {
            string filePath = GetSaveFilePath(playerId);

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"������ �÷��̾� �����Ͱ� �����ϴ�: {playerId}");
                return false;
            }

            File.Delete(filePath);
            Debug.Log($"�÷��̾� ������ ���� �Ϸ�: {playerId}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"�÷��̾� ������ ���� ����: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// ����� ��� �÷��̾� ID ��� ��������
    /// </summary>
    public static List<string> GetAllPlayerIds()
    {
        List<string> playerIds = new List<string>();

        try
        {
            string saveDir = GetSaveDirectoryPath();

            if (!Directory.Exists(saveDir))
            {
                Debug.Log("�÷��̾� ������ ������ �����ϴ�.");
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

            Debug.Log($"�� {playerIds.Count}���� �÷��̾� �����Ͱ� �߰ߵǾ����ϴ�.");
        }
        catch (Exception e)
        {
            Debug.LogError($"�÷��̾� ID ��� �������� ����: {e.Message}");
        }

        return playerIds;
    }

    /// <summary>
    /// �÷��̾� ������ ���
    /// </summary>
    public static bool BackupPlayerData(string playerId)
    {
        if (!HasPlayerData(playerId))
        {
            Debug.LogWarning($"����� �÷��̾� �����Ͱ� �����ϴ�: {playerId}");
            return false;
        }

        try
        {
            string originalPath = GetSaveFilePath(playerId);
            string backupPath = Path.Combine(GetSaveDirectoryPath(), $"{playerId}_backup_{DateTime.Now:yyyyMMdd_HHmmss}{FileExtension}");

            File.Copy(originalPath, backupPath);
            Debug.Log($"�÷��̾� ������ ��� �Ϸ�: {backupPath}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"�÷��̾� ������ ��� ����: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// ���� ���� ���� (�����Ϳ�����)
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
            Debug.Log($"���� ������ �����ϴ�: {path}");
        }
#endif
    }

    /// <summary>
    /// �÷��̾� ������ ���� ��� (����׿�)
    /// </summary>
    public static void PrintPlayerDataInfo(string playerId)
    {
        PlayerData data = LoadPlayerData(playerId);
        if (data == null)
        {
            Debug.Log($"�÷��̾� �����͸� ã�� �� �����ϴ�: {playerId}");
            return;
        }

        Debug.Log($"=== {data.playerName}�� ������ ===");
        Debug.Log($"ID: {data.playerId}");
        Debug.Log($"����: {data.level}");
        Debug.Log($"����ġ: {data.currentExp}/{data.maxExp}");
        Debug.Log($"���: {data.gold}");
        Debug.Log($"�κ��丮 ������: {data.inventoryItemIds.Count}��");
        Debug.Log($"������ ������: {data.equippedItemIds.Count}��");
        Debug.Log($"�⺻ ���� - ����: {data.baseAttack}, ���: {data.baseDefense}, ü��: {data.baseHealth}, �ӵ�: {data.baseSpeed}");
    }
}
