using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SaveData
{
    public bool isFinished;
    public int level;
}
public class GameData : MonoBehaviour
{
    public static GameData Instance;
    public SaveData saveData;
    
    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        Load();
    }

    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/gameData.dat", FileMode.Create);

        SaveData data = new SaveData();
        data = saveData;
        formatter.Serialize(file,data);
        file.Close();
        
        Debug.Log("Saved");
    }

    private void OnDisable()
    {
        Save();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/gameData.dat"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameData.dat", FileMode.Open);
            saveData = formatter.Deserialize(file) as SaveData;
            file.Close();
            
            Debug.Log("Loaded");
        }
    }

    public SaveData GetSaveData()
    {
        return saveData;
    }
    
}
