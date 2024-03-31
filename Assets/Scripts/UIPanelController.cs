using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIPanelController : MonoBehaviour
{
    public Animator panelAnim;
    public Animator popupAnim;
    public GameObject celebrationPanel;
    public static UIPanelController Instance;
    
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

    public void Fail()
    {
        if (panelAnim != null && popupAnim != null)
        {
            panelAnim.SetBool("Out", true);
            popupAnim.SetBool("Out", true);
        }
    }
    
    public void TryAgain()
    {
        if (panelAnim != null && popupAnim != null)
        {
            panelAnim.SetBool("Out", false);
            popupAnim.SetBool("Out", false);
        }

        StartCoroutine(GameRestart());
    }

    public void WinGame()
    {
        if (panelAnim != null && popupAnim != null)
        {
            panelAnim.SetBool("Out", true);
            StartCoroutine(SetStar());
        }

        StartCoroutine(WinGameCo());
    }

    IEnumerator GameRestart()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.RestartLevel();
    } 
    IEnumerator SetStar()
    {
        yield return new WaitForSeconds(1f);
        celebrationPanel.SetActive(true);
    } 

    IEnumerator WinGameCo()
    {
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene("Main");
    } 
    
}
