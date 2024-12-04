using System.Collections.Generic;
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
    public GameObject displayEnemyInfo;
    public GameObject displayStory;

    [Header("MonsterSelectedSlot")]
    public Transform SelectedMonster;
    private List<GameObject> monsterSelectedSlots;

    [Header("MonsterList")]
    public GameObject monsterListSlot;
    public Transform monsterListScroll;
    private List<MonsterSpriteData> _monstersSprite;


    private enum Display
    {
        SelectMonster,
        EnemyInfo,
        Story
    }
    private Display _currentDisplay;

    void Start()
    {
        ShowMonsterScroll();
        EventSystem.current.SetSelectedGameObject(btnSelectMonster);

        btnSelectMonster.GetComponent<Button>().onClick.AddListener(ShowSelectMonster);
        btnEnemyInfo.GetComponent<Button>().onClick.AddListener(ShowEnemyInfo);
        btnStory.GetComponent<Button>().onClick.AddListener(ShowStory);
        btnGo.GetComponent<Button>().onClick.AddListener(LoadGameScene);
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
    }

    private void ShowMonsterScroll()
    {
        _monstersSprite = DataManager.Instance.GetMonsterSpriteData();
        SpriteAtlas sprites = Resources.Load<SpriteAtlas>("UI/UISprites/MonsterList");

        for (int i = 0; i < _monstersSprite.Count; i++)
        {
            //����
            GameObject Instance = Instantiate(monsterListSlot);
            //������Ʈ ��ġ
            Instance.transform.SetParent(monsterListScroll);
            Instance.transform.localScale = Vector3.one;
            //�̹��� ��ȯ
            //Instance.GetComponent<MonsterListSlot>().setSlotImage(sprites.GetSprite(_monstersSprite[i].spriteName));
            var sprite = Instance.transform.GetChild(0).GetComponent<Image>();
            sprite.sprite = sprites.GetSprite(_monstersSprite[i].spriteName);
        }
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
