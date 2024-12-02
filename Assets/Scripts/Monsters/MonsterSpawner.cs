using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private StageManager _stageManager;
    private MonsterManager _monsterManager;
    private Transform[] _spawnPoints;
    
    private void Start()
    {
        _stageManager = StageManager.Instance;
        _monsterManager = MonsterManager.Instance;
        
        GetSpawnPointsFromCurrentStage();
    }
    
    private void GetSpawnPointsFromCurrentStage()
    {
        GameObject currentStage = GameObject.Find("Stage1"); //_stageManager.CurrentStage; //REPLACE FIND LATER
        if (currentStage != null)
        {
            // Get all child transforms tagged as "SpawnPoint"
            Transform[] allTransforms = currentStage.GetComponentsInChildren<Transform>();
            _spawnPoints = System.Array.FindAll(allTransforms, t => t.CompareTag("SpawnPoint"));
        }
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //when player touch
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchPosition.z = 0f;
    
            foreach (Transform spawnPoint in _spawnPoints)
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
        MonsterSO selectedMonsterData = _monsterManager.Monsters[selectedMonsterIndex];
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