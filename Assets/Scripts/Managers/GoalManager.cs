using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Goal
{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public bool isCompleted;
    
    public Goal(Sprite goalSprite)
    {
        this.numberNeeded = 1;
        this.numberNeeded = 0;
        this.goalSprite = goalSprite;
    }
}

public class GoalManager : MonoBehaviour
{
    public Text moveCountText;
    public int moveCount;
    
    private Dictionary<string, Goal> levelGoals;
    private Dictionary<string, GoalPanel> currentGoals;
    public GameObject goalPrefab;
    public GameObject goalGameParent;

    public static GoalManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    
    public void SetupGoal(string key, Sprite goalSprite)
    {
        if (levelGoals == null)
        {
            levelGoals = new Dictionary<string, Goal>();
            currentGoals = new Dictionary<string, GoalPanel>();
        }
        
        if (levelGoals.ContainsKey(key))
        {
            Debug.Log("Create Goal for" + key);
            levelGoals[key].numberNeeded += 1;
            currentGoals[key].thisString = "" + levelGoals[key].numberNeeded;
        }
        else
        {
            levelGoals.Add(key,new Goal(goalSprite));
            GameObject goal = Instantiate(goalPrefab, goalGameParent.transform.position, goalGameParent.transform.rotation);
            goal.transform.SetParent(goalGameParent.transform);
            
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[key].goalSprite;
            levelGoals[key].numberNeeded = 1;
            panel.thisString = "" + levelGoals[key].numberNeeded;
            currentGoals.Add(key,panel);

        }
    }

    public void UpdateGoal(string key)
    {
        if (levelGoals.ContainsKey(key))
        {
            levelGoals[key].numberCollected += 1;
            if (levelGoals[key].numberCollected >= levelGoals[key].numberNeeded)
            {
                currentGoals[key].thisString = "" + 0;
                levelGoals[key].isCompleted = true;
                currentGoals[key].CompleteGoal();
                Debug.Log("Goal " + key + " completed!");
                if (CompareGoal())
                {
                    LevelManager.Instance.updateLevel();
                    FadePanelController.Instance.WinGame();
                    //LevelManager.Instance.goToMainMenu();
                }
                
            }
            else
            {
                currentGoals[key].thisString = "" + (levelGoals[key].numberNeeded - levelGoals[key].numberCollected);
                currentGoals[key].UpdateGoal();
            }
        }
    }
    
    public bool CompareGoal()
    {
        foreach (Goal goal in levelGoals.Values)
        {
            if (!goal.isCompleted)
            {
                return false;
            }
        }
        
        Debug.Log("Level Completed");
        return true;
    }

    public void setMoveCount(int moveCount)
    {
        this.moveCount = moveCount;
        moveCountText.text = moveCount.ToString();
    }

    public void decreaseMoveCount(int amountToDecrease)
    {
        moveCount -= amountToDecrease;

        if (moveCount == 0)
        {
            if (CompareGoal())
            {
                LevelManager.Instance.updateLevel();
                FadePanelController.Instance.WinGame();
            }
            Debug.Log("No move left");
            FadePanelController.Instance.Fail();
            //LevelManager.Instance.goToMainMenu();
        }
        moveCountText.text = moveCount.ToString();
    }
}
