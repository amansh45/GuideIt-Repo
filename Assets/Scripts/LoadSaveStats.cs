using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public static class LoadSaveStats
{
    public static void SavePlayerData(List<PlayerStatistics.Task> tasksList, List<PlayerStatistics.Chapter> chaptersList, List<PlayerStatistics.Upgrade> upgradesList,
        int firstActiveTaskIndex, int secondActiveTaskIndex, int totalTasksCompleted, int highestChapter, int highestLevel, int playerCoins, float musicVolume, float sfxVolume)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerData.config";

        FileStream stream = new FileStream(path, FileMode.Create);
        PlayerData data = new PlayerData();
        data.PopulatePlayerData(tasksList, chaptersList, upgradesList, firstActiveTaskIndex, secondActiveTaskIndex, totalTasksCompleted,
            highestChapter, highestLevel, playerCoins, musicVolume, sfxVolume );


        var jsonString = JsonConvert.SerializeObject(data);

        Debug.Log(jsonString);

        formatter.Serialize(stream, jsonString);
        stream.Close();
    }

    public static PlayerData LoadPlayerData()
    {
        string path = Application.persistentDataPath + "/playerData.config";
        Debug.Log(path);
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            string jsonString = formatter.Deserialize(stream) as string;

            PlayerData data = JsonConvert.DeserializeObject<PlayerData>(jsonString);

            stream.Close();

            return data;
        } else
        {
            return null;
        }
    }


}
