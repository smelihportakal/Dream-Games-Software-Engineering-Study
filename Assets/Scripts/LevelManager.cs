using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    public void updateLevel()
    {
        currentLevel += 1;
        if (currentLevel >= levelFiles.Length)
        {
            Debug.Log("All Levels Completed");
            return;
        }
    }

    public Level getCurrentLevel()
    {
        Level level = new Level();
        level = JsonUtility.FromJson<Level>(levelFiles[GameData.Instance.saveData.level - 1].textJson.text);
        return level;
    }
}
