using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private StageManager _stageManager;
    private Transform[] spawnPoints;
    private MonsterManager _monsterManager;
    
    private void Start()
    {
        _stageManager = StageManager.Instance;
        _monsterManager = MonsterManager.Instance;
        
        GetSpawnPointsFromCurrentStage();
    }
    
    private void GetSpawnPointsFromCurrentStage()
    {
        // GameObject Stage = _stageManager.CurrentStage;
        // spawnPoints = Stage.GetComponentsInChildren<Transform>(); // This gets all child transforms (spawn points)
        // spawnPoints = System.Array.FindAll(spawnPoints, t => t.CompareTag("SpawnPoint")); //NO FIND. REPLACE LATER
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //when player touch
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchPosition.z = 0f;

            foreach (Transform spawnPoint in spawnPoints)
            {
                if (Vector2.Distance(touchPosition, spawnPoint.position) < 0.5f)
                {
                    SpawnMonster(spawnPoint.position);
                    return;
                }
            }
        }
    }
    
    public void SpawnMonster(Vector3 spawnPosition)
    {
        int selectedMonsterIndex = _monsterManager.SelectedMonsterIndex;
        MonsterSO selectedMonsterData = _monsterManager.selectedMonsters[selectedMonsterIndex];
        string poolTag = selectedMonsterData.poolTag;
        
        GameObject spawnedMonster = PoolManager.Instance.SpawnFromPool(poolTag, spawnPosition, Quaternion.identity);
        if (spawnedMonster != null)
        {
            Monster monster = spawnedMonster.GetComponent<Monster>();
            if (monster != null)
            {
                monster.data = selectedMonsterData;
                monster.SetState(MonsterState.Idle);
            }
        }
    }
}