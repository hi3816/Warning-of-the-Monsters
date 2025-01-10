using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;
using DG.Tweening;


public class StagePopup : UIBase
{
    [Header("StagePopup")]
    [SerializeField] Transform _PopupTransform;

    [Header("Button")]
    public GameObject btnSelectMonster;
    public GameObject btnEnemyInfo;
    public GameObject btnStory;
    public GameObject btnGo;

    [Header("Display")]
    [SerializeField] TextMeshProUGUI _selectTitle;

    [SerializeField] GameObject _displaySelectMonster;
    [SerializeField] GameObject _displayStageInfo;
    [SerializeField] GameObject _displayStory;
    private int _stageIdx;

    [Header("MonsterSelectedSlot")]
    public List<MonsterSelectedSlot> monsterSelectedSlots;
    
    private int _crrSlotIdx;

    [Header("MonsterList")]
    public GameObject monsterListSlot;
    public Transform monsterListScroll;
    Dictionary<int, (int MonsterId, string MonsterName)> _selectedListData = new Dictionary<int, (int, string)>();
    
    private List<DataTable.Monster_Data> _monsterSOs;
    private Dictionary<string, int> _monsterListData;
    private SpriteAtlas _sprites;

    [Header("disPlayStageInfo")]
    [SerializeField] TextMeshProUGUI _titleTxt;
    [SerializeField] TextMeshProUGUI _stageInfoWave;
    [SerializeField] TextMeshProUGUI _stageInfoHealth;
    [SerializeField] TextMeshProUGUI _stageInfoGold;

    [Header("disPlayStroy")]
    [SerializeField] TextMeshProUGUI _storyTxt;
    [SerializeField] RectTransform _storyContent;

    [Header("etc")]
    [SerializeField] Transform _warningPopupPosition;
    [SerializeField] string _warningTxt;
    [SerializeField] TextMeshProUGUI _testTxt;

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
        SelectSlotWithArrow(_crrSlotIdx);

        ShowSelectMonster();
    }

    private void OnEnable()
    {
        _PopupTransform.localPosition = new Vector3(0, -1200, 0);
        _PopupTransform.DOLocalMove(new Vector3(0, 0, 0), 0.5f)
            .SetEase(Ease.OutBack);
    }

    public void Close()
    {
        ResetSelectedMonster();

        _PopupTransform.DOLocalMove(new Vector3(0, -1200, 0), 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }

    public void SetCurSlotIdx(int Idx)
    {
        _crrSlotIdx = Idx;
        SelectSlotWithArrow(_crrSlotIdx);
    }

    public void SetStageIdx(int Idx)
    {
        _stageIdx = Idx;

        InitializeStagePopup();
    }

    private void InitializeStagePopup()
    {
        SetStageInfo(_stageIdx);
        SetStageStoryHeight(_stageIdx);
        ShowSelectMonster();
    }

    private void LoadGameScene()
    {
        if (_selectedListData.Count != 4) 
        {
            _warningTxt = "몬스터를 모두 선택하세요";

            WarningBox warningBox = PoolManager.Instance.SpawnFromPool<WarningBox>("WarningBox");
            warningBox.SetText(_warningTxt);

            return; 
        }
        DataManager.Instance.selectedStageIdx = _stageIdx;              //선택된 스테이지
        DataManager.Instance.selectedMonsterData = _selectedListData;    //선택된 몬스터
        
        LoadingManager.Instance.ChangeScene("MainScene");
    }

    private void ShowSelectMonster()
    {
        HideSelectedDisplay(_currentDisplay);
        _displaySelectMonster.SetActive(true);
        _currentDisplay = Display.SelectMonster;
        _selectTitle.text = "SELCECT MONSTER";
    }

    private void SetMonsterScroll()
    {
        //_testSOs = DataManager.Instance.GetTestSprite();
        _monsterSOs = DataManager.Instance.GetMonsterSOs();
        _sprites = Resources.Load<SpriteAtlas>("UI/UISprites/MonsterSprites");

        _monsterListData = new Dictionary<string, int>();
        for (int i = 0; i < _monsterSOs.Count; i++)
        {
            if (_monsterSOs[i].monsterType == MonsterType.Stationary || _monsterSOs[i].monsterType == MonsterType.Summoner)
            {
                GameObject Instance = Instantiate(monsterListSlot);

                Instance.transform.SetParent(monsterListScroll);
                Instance.transform.localScale = Vector3.one;

                var sprite = Instance.transform.GetChild(0).GetComponent<Image>();
                sprite.sprite = _sprites.GetSprite(_monsterSOs[i].name);

                _monsterListData.Add(_monsterSOs[i].name, _monsterSOs[i].id);
            }
        }
    }
    public void SelectSlotWithArrow(int slotIdx)
    {
        _crrSlotIdx = slotIdx;
        for (int i = 0; i < monsterSelectedSlots.Count; i++) 
        {
            Color color1 = new Color(1, 1, 1, 0);
            monsterSelectedSlots[i].arrowImg.color = color1;
        }
        Color color = new Color(1, 1, 1, 1);
        monsterSelectedSlots[slotIdx].arrowImg.color = color;
    }

    public void SelectListSlot(Sprite listSlotSprite)
    {
        string name = listSlotSprite.name.Replace("(Clone)","").Trim();

        //선택한 몬스터 데이터를 _selectedListData 넣어줌.
        foreach (var Data in _monsterListData) 
        {
            if (Data.Key == name) 
            {
                foreach (var data in _selectedListData)
                {
                    if (data.Value.MonsterId == Data.Value)
                    {
                        _warningTxt = "This monster \r\nis already selected!";
                        
                        UIBase warningBox = PoolManager.Instance.SpawnFromPool<WarningBox>("WarningBox");
                        warningBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _warningTxt;

                        //_testTxt.text = "";
                        //foreach (var asdf in _selectedListData)
                        //{
                        //    _testTxt.text += $"{asdf.Key} : {asdf.Value}\n";
                        //}
                        return;
                    }
                }
                //이미 슬롯이 채워져있을 시 채워져있는 몬스터를 삭제해주는 로직.
                if (_selectedListData.ContainsKey(_crrSlotIdx))
                {
                    _selectedListData.Remove(_crrSlotIdx);
                }
                _selectedListData.Add(_crrSlotIdx, (Data.Value, Data.Key));
                _warningTxt = "";

                //_testTxt.text = "";
                //foreach (var asdf in _selectedListData)
                //{
                //    _testTxt.text += $"{asdf.Key} : {asdf.Value}\n";
                //}
            }
        }
        monsterSelectedSlots[_crrSlotIdx].UpdateSelectedSlot(listSlotSprite);

        //다음 선택으로
        if (_crrSlotIdx == 3)
        {
            SelectSlotWithArrow(0);
        }
        else 
        {
            _crrSlotIdx++;
            SelectSlotWithArrow(_crrSlotIdx);
        }

    }

    private void ShowStageInfo()
    {
        HideSelectedDisplay(_currentDisplay);
        _displayStageInfo.SetActive(true);
        _currentDisplay = Display.EnemyInfo;
        _selectTitle.text = "STAGE INFO";
    }

    public void SetStageInfo(int index)
    {
        //StageInfo Load
        int a = index;
        DataTable.Stage_Data stageSO = DataManager.Instance.GetStageByIndex(index);
        _titleTxt.text = $"{stageSO.id}";
        _stageInfoWave.text = $"{stageSO.wave}";
        _stageInfoHealth.text = $"{stageSO.health}";
        _stageInfoGold.text = $"{stageSO.gold}";
        _storyTxt.text = $"{stageSO.story}";
    }

    private void ShowStory()
    {
        HideSelectedDisplay(_currentDisplay);
        _displayStory.SetActive(true);
        _currentDisplay = Display.Story;
        _selectTitle.text = "STORY";
    }

    private void SetStageStoryHeight(int idx)
    {
        _storyContent.localPosition = new Vector2(0,0);
        int lineCount = _storyTxt.text.Split('\n').Length;

        float defaultwidth = 733f;
        float defaultheight = 300f;

        if (lineCount * 50 >= defaultheight)
        {
            _storyContent.sizeDelta = new Vector2(defaultwidth, lineCount * 50);
        }
        else 
        {
            _storyContent.sizeDelta = new Vector2(defaultwidth, defaultheight);
        }
    }

    private void HideSelectedDisplay(Display display)
    {
        switch (display)
        {
            case Display.SelectMonster:
                _displaySelectMonster.SetActive(false);
                break;

            case Display.EnemyInfo:
                _displayStageInfo.SetActive(false);
                break;

            case Display.Story:
                _displayStory.SetActive(false);
                break;
        }
    }

    private void ResetSelectedMonster()
    {
        foreach (var slot in monsterSelectedSlots)
        {
            //스프라이트 지워주기
            var slotImg = slot.slotImg;
            slotImg.sprite = null;
            slotImg.color = new Color(1, 1, 1, 0);
        }
        //정보 없애기
        _selectedListData.Clear();
    }
}
