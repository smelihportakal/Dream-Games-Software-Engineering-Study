using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartLevelButton : MonoBehaviour
{
    private Button _button;
    public Text thisText;
    public string thisString;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ButtonClick);
    }

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    void Setup()
    {
        SaveData save = GameData.Instance.GetSaveData();
        if (save.isFinished)
        {
            thisString = "Finished";
        }
        else
        {
            thisString = "Level " + save.level;
        }
        thisText.text = thisString;
    }

    public void ButtonClick()
    {
        Debug.Log("Button Pressed");
        SceneManager.LoadScene("Level");
    }

    // Update is called once per frame
    void Update()
    {
        Setup();
    }
}
