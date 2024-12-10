using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class PopupMonsterSpawner : MonsterSpawner
{
    [SerializeField] private GameObject monsterSelectionPopup; // 몬스터 선택 팝업 UI
    [SerializeField] private List<GameObject> slot;
    private Vector3 _pendingSpawnPosition; // 선택된 스폰 위치
    private Transform _pendingSpawnPoint; // 선택된 스폰 포인트

    private Dictionary<int, int> _selectedMonsterList;

    private void Awake()
    {
        _selectedMonsterList = DataManager.Instance.SelectedMonsterData;
        Debug.Log("들어온 데이터 확인");
        foreach (var Data in _selectedMonsterList)
        {
            Debug.Log($"{Data.Key} , {Data.Value}");
        }
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

    public void OnMonsterSelected()
    {
        // 몬스터 선택 후 부모 클래스의 SpawnMonster 호출
        if (_pendingSpawnPoint != null 
            //&& selectedMonsterData != null
            )
        {
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
        TestSO[] _testSOs = DataManager.Instance.GetTestSprite();
        SpriteAtlas _sprites = Resources.Load<SpriteAtlas>("UI/UISprites/MonsterList");

        for (int i = 0; i < _testSOs.Length; i++)
        {
            if (_selectedMonsterList.ContainsValue(_testSOs[i].id))
            {
                //_selectedMonsterList.
                //var sprite = slot[].GetComponent<SpriteAtlas>();
            }
        }
    }
}
