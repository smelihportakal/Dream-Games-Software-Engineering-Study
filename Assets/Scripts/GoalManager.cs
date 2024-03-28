using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BlankGoal
{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public string matchValue;
}

public class GoalManager : MonoBehaviour
{
    public Text moveCountText;
    public int moveCount;
    
    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    public GameObject goalPrefab;
    public GameObject goalGameParent;
    
    // Start is called before the first frame update
    void Start()
    {
        SetupGoals();
    }
    
    void SetupGoals()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            GameObject goal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalGameParent.transform);
            
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "" + levelGoals[i].numberNeeded;
        }
    }

    public void UpdateGoals()
    {
        int goalsCompleted = 0;

        for (int i = 0; i < levelGoals.Length; i++)
        {
            currentGoals[i].thisText.text = "" + (levelGoals[i].numberNeeded - levelGoals[i].numberCollected);
            if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = "0";
            }
        }

        if (goalsCompleted >= levelGoals.Length)
        {
            Debug.Log("You Win");
        }
    }

    public void CompareGoal(string goalToCompare)
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            if (goalToCompare == levelGoals[i].matchValue)
            {
                levelGoals[i].numberCollected++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveCountText.text = moveCount.ToString();
    }

    public void setMoveCount(int moveCount)
    {
        this.moveCount = moveCount;
    }

    public void decreaseMoveCount(int amountToDecrease)
    {
        moveCount -= amountToDecrease;
    }
}
