using UnityEngine;
using System.IO;

/*Saving Player and Map
  Made by Kerstin*/
public static class SaveSystem
{
    static string playerPersistentPath;
    static string mapPersistentPath;

    public static void SetPaths()
    {
        playerPersistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "PlayerSaveData.json";
        mapPersistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "MapSaveData.json";
    }

    #region Saving
    static void SavePlayer()
    {
        string savePath = playerPersistentPath;
        string playerDataSave = GameObject.FindWithTag("Player").GetComponent<PlayerController>().GetSaveData();

        using StreamWriter writer = new StreamWriter(savePath);
        writer.Write(playerDataSave);
    }

    static void SaveMap(SaveDataScriptableObject mapSaveDataSO)
    {
        string savePath = mapPersistentPath;
        mapSaveDataSO.loadGame = false;
        string mapDataSave = JsonUtility.ToJson(mapSaveDataSO);

        using StreamWriter writer = new StreamWriter(savePath);
        writer.Write(mapDataSave);
    }
    #endregion

    #region Loading
    public static void LoadPlayer() //used individually
    {
        using StreamReader reader = new StreamReader(playerPersistentPath);
        string playerDataSave = reader.ReadToEnd();

        GameObject.FindWithTag("Player").GetComponent<PlayerController>().LoadSaveData(playerDataSave);
    }

    static void LoadMap(SaveDataScriptableObject mapSaveDataSO)
    {
        using StreamReader reader = new StreamReader(mapPersistentPath);
        string mapDataSave = reader.ReadToEnd();

        JsonUtility.FromJsonOverwrite(mapDataSave, mapSaveDataSO);
        mapSaveDataSO.loadGame = true;
    }
    #endregion


    public static void Save(SaveDataScriptableObject mapSaveDataSO, ScriptableInt playerPoints)
    {
        SavePlayer();
        SaveMap(mapSaveDataSO);
    }

    public static void Load(SaveDataScriptableObject mapSaveDataSO, ScriptableInt playerPoints)
    {
        LoadMap(mapSaveDataSO);
    }
}
