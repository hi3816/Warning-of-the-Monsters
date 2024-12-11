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
    public List<GameObject> monsterSelectedSlots;
    
    private int _crrSlotIdx;

    [Header("MonsterList")]
    public GameObject monsterListSlot;
    public Transform monsterListScroll;
    Dictionary<int, (int MonsterId, string MonsterName)> _selectedListData = new Dictionary<int, (int, string)>();

    private TestSO[] _testSOs;
    private Dictionary<string, int> _monsterListData;
    private SpriteAtlas _sprites;

    [Header("disPlayStageInfo")]
    [SerializeField] TextMeshProUGUI stageInfoTxt;
    [SerializeField] TextMeshProUGUI enemyInfoTxt;

    [SerializeField] TextMeshProUGUI testText;

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

        UIManager.Instance.OnClickListSlot = SelectListSlot;

        SetMonsterScroll();
        SetStageInfo(_stageIdx);
    }

    public void SetStageIdx(int Idx)
    {
        _stageIdx = Idx;
    }

    private void LoadGameScene()
    {
        if (_selectedListData.Count != 4) { Debug.Log("몬스터를 모두 선택하세요"); return; }
        DataManager.Instance.selectedStageIdx = _stageIdx;              //선택된 스테이지
        DataManager.Instance.SelectedMonsterData = _selectedListData;    //선택된 몬스터
        
        SceneManager.LoadScene("MainScene");
    }

    private void ShowSelectMonster()
    {
        HideSelectedDisplay(_currentDisplay);
        displaySelectMonster.SetActive(true);
        _currentDisplay = Display.SelectMonster;
    }

    private void SetMonsterScroll()
    {
        _testSOs = DataManager.Instance.GetTestSprite();
        _sprites = Resources.Load<SpriteAtlas>("UI/UISprites/MonsterList");

        _monsterListData = new Dictionary<string, int>();
        for (int i = 0; i < _testSOs.Length; i++)
        {
            GameObject Instance = Instantiate(monsterListSlot);

            Instance.transform.SetParent(monsterListScroll);
            Instance.transform.localScale = Vector3.one;

            var sprite = Instance.transform.GetChild(0).GetComponent<Image>();
            sprite.sprite = _sprites.GetSprite(_testSOs[i].testSpriteName);

            _monsterListData.Add(_testSOs[i].testSpriteName, _testSOs[i].id);
        }
    }
    public void SelectSlot(int slotIdx)
    {
        _crrSlotIdx = slotIdx;
    }

    public void SelectListSlot(Sprite listSlotSprite)
    {
        string name = listSlotSprite.name.Replace("(Clone)","").Trim();
        //이미 슬롯이 채워져있을 시 채워져있는 몬스터를 삭제해주는 로직.
        if (_selectedListData.ContainsKey(_crrSlotIdx))
        {
            _selectedListData.Remove(_crrSlotIdx);
        }

        //선택한 몬스터 데이터를 _selectedListData 넣어줌.
        foreach (var Data in _monsterListData) 
        {
            if (Data.Key == name) 
            {
                foreach (var data in _selectedListData)
                {
                    if (data.Value.MonsterId == Data.Value)
                    {
                        Debug.Log("이미 선택한 몬스터입니다!!");
                        return;
                    }
                }
                _selectedListData.Add(_crrSlotIdx, (Data.Value, Data.Key)); 
            }
        }

        //디버그용
        foreach (var Data in _selectedListData)
        {
            Debug.Log($"{Data.Key} , {Data.Value}");
        }

        UpdateSelectedSlot(listSlotSprite);
    }

    private void UpdateSelectedSlot(Sprite listSlotSprite)
    {
        var slotImg = monsterSelectedSlots[_crrSlotIdx].transform.GetChild(0).GetComponent<Image>();
        slotImg.sprite = listSlotSprite;
    }

    private void ShowStageInfo()
    {
        HideSelectedDisplay(_currentDisplay);
        displayStageInfo.SetActive(true);
        _currentDisplay = Display.EnemyInfo;
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
