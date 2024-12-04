using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
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
    public GameObject displayStageInfo;
    public GameObject displayStory;
    private int _stageIdx;

    [Header("MonsterSelectedSlot")]
    public Transform SelectedMonster;
    private List<GameObject> monsterSelectedSlots;

    [Header("MonsterList")]
    public GameObject monsterListSlot;
    public Transform monsterListScroll;
    //private List<MonsterSpriteData> _monstersSprite;
    private TestSO[] _testSOs;

    [Header("disPlayStageInfo")]
    [SerializeField] TextMeshProUGUI stageInfoTxt;
    [SerializeField] TextMeshProUGUI enemyInfoTxt;


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
        btnEnemyInfo.GetComponent<Button>().onClick.AddListener(ShowStageInfo);
        btnStory.GetComponent<Button>().onClick.AddListener(ShowStory);
        btnGo.GetComponent<Button>().onClick.AddListener(LoadGameScene);
    }

    public void SetStageIdx(int Idx)
    {
        _stageIdx = Idx;
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("StageScene");
    }

    private void ShowSelectMonster()
    {
        HideSelectedDisplay(_currentDisplay);
        displaySelectMonster.SetActive(true);
        _currentDisplay = Display.SelectMonster;

        SetMonsterScroll();
    }

    private void SetMonsterScroll()
    {
        _testSOs = DataManager.Instance.GetTestSprite();
        SpriteAtlas sprites = Resources.Load<SpriteAtlas>("UI/UISprites/MonsterList");

        for (int i = 0; i < _testSOs.Length; i++)
        {
            GameObject Instance = Instantiate(monsterListSlot);

            Instance.transform.SetParent(monsterListScroll);
            Instance.transform.localScale = Vector3.one;

            var sprite = Instance.transform.GetChild(0).GetComponent<Image>();
            sprite.sprite = sprites.GetSprite(_testSOs[i].testSpriteName);
        }
    }

    private void ShowStageInfo()
    {
        HideSelectedDisplay(_currentDisplay);
        displayStageInfo.SetActive(true);
        _currentDisplay = Display.EnemyInfo;

        SetStageInfo(_stageIdx);
    }

    public void SetStageInfo(int index)
    {
        //StageInfo Load
        StageSO stageSO = DataManager.Instance.GetStageByIndex(index);
        stageInfoTxt.text = $"{stageSO.name}\n{stageSO.wave}\n{stageSO.health}";

        //EnemyInfo Load

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
                displayStageInfo.SetActive(false);
                break;

            case Display.Story:
                displayStory.SetActive(false);
                break;
        }
    }
}
