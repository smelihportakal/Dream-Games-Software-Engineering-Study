using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Level
{
    public int level_number;
    public int grid_width;
    public int grid_height;
    public int move_count;
    public string[] grid;
}

[System.Serializable]
public class LevelJSON
{
    public int level_number;
    public TextAsset textJson;
}


public class LevelManager : MonoBehaviour
{
    public LevelJSON[] levelFiles;
    public int currentLevel;
    
    public static LevelManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        currentLevel = GameData.Instance.saveData.level;
    }

    public void updateLevel()
    {
        currentLevel += 1;
        if (currentLevel > levelFiles.Length)
        {
            GameData.Instance.GetSaveData().isFinished = true;
            Debug.Log("All Levels Completed");
            GameData.Instance.Save();
            return;
        }
        GameData.Instance.GetSaveData().level = currentLevel;
        GameData.Instance.Save();
    }

    public Level getCurrentLevel()
    {
        Level level = new Level();
        level = JsonUtility.FromJson<Level>(levelFiles[GameData.Instance.saveData.level - 1].textJson.text);
        return level;
    }

    public void goToMainMenu()
    {
        SceneManager.LoadScene("Main");
    }
}
