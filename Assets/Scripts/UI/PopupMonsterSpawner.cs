using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupMonsterSpawner : MonsterSpawner
{
    [SerializeField] private GameObject monsterSelectionPopup; // 몬스터 선택 팝업 UI
    private Vector3 _pendingSpawnPosition; // 선택된 스폰 위치
    private Transform _pendingSpawnPoint; // 선택된 스폰 포인트

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
        if (monsterSelectionPopup != null)
        {
            monsterSelectionPopup.transform.position = position;
            monsterSelectionPopup.SetActive(true);
        }
    }

    public void OnMonsterSelected(MonsterSO selectedMonsterData)
    {
        // 몬스터 선택 후 부모 클래스의 SpawnMonster 호출
        if (_pendingSpawnPoint != null && selectedMonsterData != null)
        {
            base.SpawnMonster(_pendingSpawnPosition, selectedMonsterData);

            // 팝업 닫기
            if (monsterSelectionPopup != null)
            {
                monsterSelectionPopup.SetActive(false);
            }
        }
    }
}
