using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalPanel : MonoBehaviour
{
    public Image goalCheckImage;
    public Image thisImage;
    public Sprite thisSprite;
    public Text thisText;
    public string thisString;
    
    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        thisText.gameObject.SetActive(true);
        goalCheckImage.gameObject.SetActive(false);
        thisImage.sprite = thisSprite;
        thisText.text = thisString;
    }

    public void UpdateGoal()
    {
        thisText.text = thisString;
    }
    
    public void CompleteGoal()
    {
        thisText.text = "";
        thisText.gameObject.SetActive(false);
        goalCheckImage.gameObject.SetActive(true);
    }

    public void Clear()
    {
        Destroy(gameObject);
    }

}
