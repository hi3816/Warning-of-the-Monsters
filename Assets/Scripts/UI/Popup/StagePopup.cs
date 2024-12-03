using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class StagePopup : UIBase
{
    [Header("Button")]
    public GameObject btnSelectMonster;
    public GameObject btnEnemyInfo;
    public GameObject btnStory;
    public GameObject btnGo;

    [Header("Display")]
    public GameObject displaySelectMonster;
    public GameObject displayEnemyInfo;
    public GameObject displayStory;
    
    private enum Display
    {
        SelectMonster,
        EnemyInfo,
        Story
    }
    private Display _currentDisplay;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(btnSelectMonster);

        btnSelectMonster.GetComponent<Button>().onClick.AddListener(ShowSelectMonster);
        btnEnemyInfo.GetComponent<Button>().onClick.AddListener(ShowEnemyInfo);
        btnStory.GetComponent<Button>().onClick.AddListener(ShowStory);
        btnGo.GetComponent<Button>().onClick.AddListener(LoadGameScene);
    }

    private void LoadGameScene()
    {
        Debug.Log("∞‘¿”æ¿ ∑ŒµÂ");
    }

    private void ShowSelectMonster()
    {
        HideSelectedDisplay(_currentDisplay);
        displaySelectMonster.SetActive(true);
        _currentDisplay = Display.SelectMonster;
    }

    private void ShowEnemyInfo()
    {
        HideSelectedDisplay(_currentDisplay);
        displayEnemyInfo.SetActive(true);
        _currentDisplay = Display.EnemyInfo;
        
    }

    private void ShowStory()
    {
        HideSelectedDisplay(_currentDisplay);
        displayStory.SetActive(true);
        _currentDisplay = Display.Story;
    }

    private void HideSelectedDisplay(Display display)
    {
        switch (display)
        {
            case Display.SelectMonster:
                displaySelectMonster.SetActive(false);
                break;

            case Display.EnemyInfo:
                displayEnemyInfo.SetActive(false);
                break;

            case Display.Story:
                displayStory.SetActive(false);
                break;
        }


    }
}
