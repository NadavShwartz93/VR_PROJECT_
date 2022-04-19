using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{

    public static DataManager instance;

    public PlayerData data;
    public string file = "player.txt";
    public bool fileIsLoaded = false;



    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else{
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Save(string fileName)
    {
        string json = JsonUtility.ToJson(data);
        WriteToFile(fileName, json);
    }

    public void Load(string fileName)
    {
        data = new PlayerData();
        string json = ReadFromFile(fileName);
        JsonUtility.FromJsonOverwrite(json, data);
    }


    private void WriteToFile(string fileName, string json)
    {
        string path = GetFilePath(fileName);
        FileStream fileStream = new FileStream(path, FileMode.Create);
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(json);

        }
    }

    private string ReadFromFile(string fileName)
    {
        string path = GetFilePath(fileName);
        Debug.LogWarning("path :" + path);
        if (File.Exists(path))
        {
            using(StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                fileIsLoaded = true; 
                return json;

            }

        }
        else
        {
            Debug.LogWarning("File not found!");
            fileIsLoaded = false;
        }
        return "";
    } 



    private string GetFilePath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName;
    }

   /* private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save(file);
        }
    }*/
}
