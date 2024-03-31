using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadePanelController : MonoBehaviour
{
    public Animator panelAnim;

    public Animator popupAnim;

    public GameObject celebrationPanel;
    public GameObject TopUI;

    public static FadePanelController Instance;
    
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    } 
    IEnumerator SetStar()
    {
        yield return new WaitForSeconds(1f);
        TopUI.SetActive(false);
        celebrationPanel.SetActive(true);
    } 

    IEnumerator WinGameCo()
    {
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene("Main");
    } 
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
