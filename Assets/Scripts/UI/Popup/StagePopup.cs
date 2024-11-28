using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class StagePopup : UIBase
{
    
    public GameObject btnSelectMonster;
    public GameObject btnEnemyInfo;
    public GameObject btnStory;

    public GameObject displaySelectMonster;
    public GameObject displayEnemyInfo;
    public GameObject displayStory;

    public enum ShowDisplay
    {
        SelectMonster,
        EnemyInfo,
        Story
    }

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(btnSelectMonster);

        btnSelectMonster.GetComponent<Button>().onClick.AddListener(ShowSelectDisplay);
        btnSelectMonster.GetComponent<Button>().onClick.AddListener(HideSelectedDisplay);

        btnEnemyInfo.GetComponent<Button>().onClick.AddListener(ShowSelectDisplay);
        
        btnStory.GetComponent<Button>().onClick.AddListener(ShowSelectDisplay);
    }

    public void ShowSelectDisplay()
    { 
        //버튼관련화면 활성화
    }

    public void HideSelectedDisplay()
    {
        //이전화면 비활성화
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            
        }
    }
}
