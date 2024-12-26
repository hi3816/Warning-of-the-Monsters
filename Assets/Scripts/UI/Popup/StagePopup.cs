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
    [SerializeField] TextMeshProUGUI selectTitle;

    [SerializeField] GameObject displaySelectMonster;
    [SerializeField] GameObject displayStageInfo;
    [SerializeField] GameObject displayStory;
    private int _stageIdx;

    [Header("MonsterSelectedSlot")]
    public List<GameObject> monsterSelectedSlots;
    
    private int _crrSlotIdx;

    [Header("MonsterList")]
    public GameObject monsterListSlot;
    public Transform monsterListScroll;
    Dictionary<int, (int MonsterId, string MonsterName)> _selectedListData = new Dictionary<int, (int, string)>();

    private TestSO[] _testSOs;
    private List<DataTable.Monster_Data> _monsterSOs;
    private Dictionary<string, int> _monsterListData;
    private SpriteAtlas _sprites;

    [Header("disPlayStageInfo")]
    [SerializeField] TextMeshProUGUI titleTxt;
    [SerializeField] TextMeshProUGUI stageInfoWave;
    [SerializeField] TextMeshProUGUI stageInfoHealth;
    [SerializeField] TextMeshProUGUI stageInfoGold;

    [Header("disPlayStroy")]
    [SerializeField] TextMeshProUGUI storyTxt;

    [Header("etc")]
    [SerializeField] TextMeshProUGUI warningTxt;
    [SerializeField] TextMeshProUGUI testTxt;
    
    [SerializeField] Image[] arrowPoint;

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

    public void SetStageIdx(int Idx)
    {
        _stageIdx = Idx;
        SetStageInfo(_stageIdx);
        SetStageStory(_stageIdx);
    }

    private void LoadGameScene()
    {
        if (_selectedListData.Count != 4) 
        {
            warningTxt.text = "몬스터를 모두 선택하세요";
            return; 
        }
        DataManager.Instance.selectedStageIdx = _stageIdx;              //선택된 스테이지
        DataManager.Instance.selectedMonsterData = _selectedListData;    //선택된 몬스터
        
        SceneManager.LoadScene("MainScene");
    }

    private void ShowSelectMonster()
    {
        HideSelectedDisplay(_currentDisplay);
        displaySelectMonster.SetActive(true);
        _currentDisplay = Display.SelectMonster;
        selectTitle.text = "SELCECT MONSTER";
    }

    private void SetMonsterScroll()
    {
        //_testSOs = DataManager.Instance.GetTestSprite();
        _monsterSOs = DataManager.Instance.GetMonsterSOs();
        _sprites = Resources.Load<SpriteAtlas>("UI/UISprites/MonsterSprites");

        _monsterListData = new Dictionary<string, int>();
        for (int i = 0; i < _monsterSOs.Count; i++)
        {
            if (_monsterSOs[i].monsterType == MonsterType.Stationary)
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
        for (int i = 0; i < arrowPoint.Length; i++) 
        {
            Color color1 = new Color(1, 1, 1, 0);
            arrowPoint[i].color = color1;
        }
        Color color = new Color(1, 1, 1, 1);
        arrowPoint[slotIdx].color = color;
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
                        warningTxt.text = "This monster \r\nis already selected!";

                        testTxt.text = "";
                        foreach (var asdf in _selectedListData)
                        {
                            testTxt.text += $"{asdf.Key} : {asdf.Value}\n";
                        }
                        return;
                    }
                }
                //이미 슬롯이 채워져있을 시 채워져있는 몬스터를 삭제해주는 로직.
                if (_selectedListData.ContainsKey(_crrSlotIdx))
                {
                    _selectedListData.Remove(_crrSlotIdx);
                }
                _selectedListData.Add(_crrSlotIdx, (Data.Value, Data.Key));
                warningTxt.text = "";

                testTxt.text = "";
                foreach (var asdf in _selectedListData)
                {
                    testTxt.text += $"{asdf.Key} : {asdf.Value}\n";
                }
            }
        }
        UpdateSelectedSlot(listSlotSprite);

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

    private void UpdateSelectedSlot(Sprite listSlotSprite)
    {
        var slotImg = monsterSelectedSlots[_crrSlotIdx].transform.GetChild(0).GetComponent<Image>();
        slotImg.sprite = listSlotSprite;
        slotImg.color = new Color(1, 1, 1, 1);
    }

    private void ShowStageInfo()
    {
        HideSelectedDisplay(_currentDisplay);
        displayStageInfo.SetActive(true);
        _currentDisplay = Display.EnemyInfo;
        selectTitle.text = "STAGE INFO";
    }

    public void SetStageInfo(int index)
    {
        //StageInfo Load
        int a = index;
        DataTable.Stage_Data stageSO = DataManager.Instance.GetStageByIndex(index);
        titleTxt.text = $"{stageSO.id}";
        stageInfoWave.text = $"{stageSO.wave}";
        stageInfoHealth.text = $"{stageSO.health}";
        stageInfoGold.text = $"{stageSO.gold}";
    }

    private void ShowStory()
    {
        HideSelectedDisplay(_currentDisplay);
        displayStory.SetActive(true);
        _currentDisplay = Display.Story;
        selectTitle.text = "STORY";
    }

    private void SetStageStory(int idx)
    {
        //수정필요함
        string stage1Story = $"괴물과 인간은 \r\n두려움 속에 공존하며 \r\n서로를 피하며 살아간다. \r\n\r\n어느 날, \r\n호기심 많은 몬스터 친구들은 \r\n인간 마을 근처로 놀러 갔다가 \r\n인간과 마주치게 된다. \r\n\r\n겁에 질린 주민은 마을로 돌아가\r\n 자신이 본 일을 이야기한다. \r\n\r\n이 소식에 마을 사람들은 \r\n몬스터 토벌대를 꾸려 숲으로 향하게 된다. \r\n\r\n몬스터들은 자신들의 터전을 지키기 위해 \r\n맞서 싸우고자 하지만, \r\n\r\n인간을 해치고 싶지 않은 그들은 \r\n놀라게 해서 인간을 내쫓기로 결정한다.";
        string stage2Story = $"준비중입니다. \r\n\r\n\r\n";

        if (idx == 0)
        {
            storyTxt.text = stage1Story;
        }
        else 
        {
            storyTxt.text = stage2Story;
        }
        
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

    public void ResetSelectedMonster()
    {
        foreach (var slot in monsterSelectedSlots)
        {
            //스프라이트 지워주기
            var slotImg = slot.transform.GetChild(0).GetComponent<Image>();
            slotImg.sprite = null;
            slotImg.color = new Color(1, 1, 1, 0);
        }
        //정보 없애기
        _selectedListData.Clear();
    }
}
