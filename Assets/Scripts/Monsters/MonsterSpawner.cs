using System.Linq;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    private StageManager _stageManager;

    private void Awake()
    {
        spawnPoints = GetComponentsInChildren<Transform>().Where(t => t.CompareTag("MonsterSpawnPoint")).ToArray();
    }
    
    private void SpawnMonster(Vector3 spawnPosition, MonsterSO selectedMonsterData)
    {
        GameObject monster = PoolManager.Instance.SpawnFromPool(selectedMonsterData.poolTag, spawnPosition, Quaternion.identity);
        if (monster != null)
        {
            monster.transform.position = spawnPosition;
            monster.SetActive(true);
            
            var monsterComponent = monster.GetComponent<Monster>();
            if (monsterComponent != null)
            {
                monsterComponent.data = selectedMonsterData;
                monsterComponent.SetState(MonsterState.Idle);
            }
        }
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //when player touch
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchPosition.z = 0f;
            
            foreach (Transform spawnPoint in spawnPoints)
            {
                MonsterSO selectedMonsterData = MonsterManager.Instance.GetSelectedMonsterData();
                if (Vector2.Distance(touchPosition, spawnPoint.position) < 0.5f)
                {
                    if (MonsterManager.Instance.SelectedMonsterId != 0)
                    {
                        if (_stageManager.currGold >= selectedMonsterData.requiredCoins)
                        {
                            _stageManager.currGold -= selectedMonsterData.requiredCoins;
                            Vector3 spawnPosition = spawnPoint.position;
                            SpawnMonster(spawnPosition, selectedMonsterData);
                        }
                        else
                        {
                            print("You do not have enough gold to spawn monster");
                        }
                    }
                }
            }
        }
    }
}