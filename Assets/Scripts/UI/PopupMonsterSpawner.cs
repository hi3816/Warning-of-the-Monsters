using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using TMPro;
using UnityEngine.EventSystems;

public class PopupMonsterSpawner : MonsterSpawner, IManagebleUI
{
    [SerializeField] private GameObject _monsterSelectionPopup; // 몬스터 선택 팝업 UI
    private Vector3 _pendingSpawnPosition; // 선택된 스폰 위치
    private Transform _pendingSpawnPoint; // 선택된 스폰 포인트

    private Dictionary<int, (int monsterId, string monsterName)> _selectedMonsterDict;
    [SerializeField] private List<GameObject> _slots;
    [SerializeField] private List<GameObject> _slotsOverlay;
    [SerializeField] private List<GameObject> _checkList;
    [SerializeField] private List<TextMeshProUGUI> _costTxts;
    [SerializeField] private GameObject _statUIObj;

    private StatUI _statUI;

    private void Awake()
    {
        _selectedMonsterDict = DataManager.Instance.selectedMonsterData;

        if (_selectedMonsterDict == null) return;

        StageManager.Instance.SetMonsterUI(this, null);
        StageManager.Instance.OnChangeGold += Show;
        _statUI = _statUIObj.GetComponent<StatUI>();

        SetMonsterSprite();
    }

    public bool SearchSpawanPoint(Vector2 touchPosition)
    {
        foreach (Transform spawnPoint in SpawnPointList)
        {
            if (Vector2.Distance(touchPosition, spawnPoint.position) < 0.5f)
            {
                // 스폰 포인트와 위치 저장
                _pendingSpawnPosition = spawnPoint.position;
                _pendingSpawnPoint = spawnPoint;

                return true;
            }
        }
        return false;
    }

    public void Show()
    {
        // 이미 스폰 되어있거나 현재 클릭한 스폰 포인트가 없을 때
        if (IsSpawnPointOccupied(_pendingSpawnPosition, 0.5f) || _pendingSpawnPoint == null) return;
        
        UpdateMonsterImgState();
        if (_monsterSelectionPopup != null)
        {
            _monsterSelectionPopup.transform.position = _pendingSpawnPosition;
            _monsterSelectionPopup.SetActive(true);
        }
    }

    public void OnMonsterSelected(int slotIdx)
    {
        if (!_checkList[slotIdx].activeSelf)
        {
            foreach (GameObject check in _checkList)
            {
                check.SetActive(false);
            }
            _checkList[slotIdx].SetActive(true);
            _statUIObj.SetActive(true);
            
            if (_selectedMonsterDict.TryGetValue(slotIdx, out var monsterInfo))
            {
                DataTable.Monster_Data selectedMonsterData = MonsterManager.Instance.GetSelectedMonsterData(monsterInfo.monsterId);
                _statUI.Show(_monsterSelectionPopup.transform.position, selectedMonsterData);
                MonsterUIManager.Instance.ShowRangeIndicator(_pendingSpawnPosition, selectedMonsterData.humanDetectRange[0]);
            }
            return;
        }

        // 몬스터 선택 후 부모 클래스의 SpawnMonster 호출
        if (_pendingSpawnPoint != null 
            //&& selectedMonsterData != null
            )
        {
            if (_selectedMonsterDict.TryGetValue(slotIdx, out var monsterInfo))
            {
                MonsterManager.Instance.SelectMonster(monsterInfo.monsterId);
            }
            DataTable.Monster_Data selectedMonsterData = MonsterManager.Instance.GetSelectedMonsterData();
            base.SpawnMonster(_pendingSpawnPosition, selectedMonsterData.id);

            Hide();
            MonsterUIManager.Instance.HideRangeIndicator();
        }
    }

    private void SetMonsterSprite()
    {
        SpriteAtlas _sprites = Resources.Load<SpriteAtlas>("UI/UISprites/MonsterSprites");

        for (int i = 0; i < _slots.Count; i++)
        {
            if (_selectedMonsterDict.TryGetValue(i, out var monsterData))
            {
                var slotImg = _slots[i].transform.GetChild(0).GetComponent<Image>();
                slotImg.sprite = _sprites.GetSprite(monsterData.monsterName);
            }
        }
    }

    //몬스터 비활성화 업데이트
    private void UpdateMonsterImgState()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_selectedMonsterDict.TryGetValue(i, out var monsterInfo))
            {
                MonsterManager.Instance.SelectMonster(monsterInfo.monsterId);
                var selectedMonsterData = MonsterManager.Instance.GetSelectedMonsterData();

                //선택가능 몬스터 검사
                bool isAvailable = IsMonsterSelectable(i, selectedMonsterData);

                _slotsOverlay[i].SetActive(!isAvailable);
            }
        }
    }

    private bool IsMonsterSelectable(int idx, DataTable.Monster_Data data)
    {
        _costTxts[idx].text = data.requiredCoins[0].ToString();
        return _stageManager.CurrGold >= data.requiredCoins[0];
    }

    public void Hide()
    {
        if (!_monsterSelectionPopup.activeSelf) return;

        // 팝업 닫기
        if (_monsterSelectionPopup != null)
        {
            foreach(GameObject check in _checkList)
            {
                check.SetActive(false);
            }
            _statUIObj.SetActive(false);
            _monsterSelectionPopup.SetActive(false);
        }

        _pendingSpawnPoint = null;
    }
}
