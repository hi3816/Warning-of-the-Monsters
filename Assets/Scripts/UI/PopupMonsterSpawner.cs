using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class PopupMonsterSpawner : MonsterSpawner
{
    [SerializeField] private GameObject monsterSelectionPopup; // 몬스터 선택 팝업 UI
    private Vector3 _pendingSpawnPosition; // 선택된 스폰 위치
    private Transform _pendingSpawnPoint; // 선택된 스폰 포인트

    private Dictionary<int, (int monsterId, string monsterName)> _selectedMonsterList;
    [SerializeField] private List<GameObject> _slots;
    [SerializeField] private List<GameObject> _slotsOverlay;

    private void Awake()
    {
        _selectedMonsterList = DataManager.Instance.SelectedMonsterData;

        if (_selectedMonsterList == null) return;
        Debug.Log("들어온 데이터 확인");
        foreach (var Data in _selectedMonsterList)
        {
            Debug.Log($"{Data.Key} , {Data.Value}");
        }

        SetMonsterSprite();
    }

    void Update()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0)) // 마우스 클릭
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchPosition.z = 0f;

            foreach (Transform spawnPoint in SpawnPoints)
            {
                if (Vector2.Distance(touchPosition, spawnPoint.position) < 0.5f)
                {
                    // 스폰 포인트와 위치 저장
                    _pendingSpawnPosition = spawnPoint.position;
                    _pendingSpawnPoint = spawnPoint;

                    //몬스터 비활성화 업데이트
                    UpdateMonsterImgState();

                    // 몬스터 선택 팝업 활성화
                    ShowMonsterSelectionPopup(_pendingSpawnPosition);
                    return; // 다른 스폰 포인트 처리 중단
                }
            }
        }
    }

    private void ShowMonsterSelectionPopup(Vector2 position)
    {
        if (IsSpawnPointOccupied(_pendingSpawnPosition, 0.5f))
        {
            print("Spawn point is already occupied by another monster.");
            return;
        }

        if (monsterSelectionPopup != null)
        {
            monsterSelectionPopup.transform.position = position;
            monsterSelectionPopup.SetActive(true);
        }
    }

    public void OnMonsterSelected(int slotIdx)
    {
        // 몬스터 선택 후 부모 클래스의 SpawnMonster 호출
        if (_pendingSpawnPoint != null 
            //&& selectedMonsterData != null
            )
        {
            if (_selectedMonsterList.TryGetValue(slotIdx, out var monsterInfo))
            {
                MonsterManager.Instance.SelectMonster(monsterInfo.monsterId);
            }
            MonsterSO selectedMonsterData = MonsterManager.Instance.GetSelectedMonsterData();
            base.SpawnMonster(_pendingSpawnPosition, selectedMonsterData);

            // 팝업 닫기
            if (monsterSelectionPopup != null)
            {
                monsterSelectionPopup.SetActive(false);
            }
        }
    }

    private void SetMonsterSprite()
    {
        SpriteAtlas _sprites = Resources.Load<SpriteAtlas>("UI/UISprites/MonsterSprites");

        for (int i = 0; i < _slots.Count; i++)
        {
            if (_selectedMonsterList.TryGetValue(i, out var monsterData))
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
            if (_selectedMonsterList.TryGetValue(i, out var monsterInfo))
            {
                MonsterManager.Instance.SelectMonster(monsterInfo.monsterId);
                MonsterSO selectedMonsterData = MonsterManager.Instance.GetSelectedMonsterData();

                //선택가능 몬스터 검사
                bool isAvailable = IsMonsterSelectable(selectedMonsterData);

                _slotsOverlay[i].SetActive(!isAvailable);
            }
        }
    }

    private bool IsMonsterSelectable(MonsterSO data)
    {
        return StageManager.CurrGold >= data.requiredCoins;
    }

}
