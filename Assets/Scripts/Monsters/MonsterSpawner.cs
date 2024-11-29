using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private MonsterManager _monsterManager;
    private StageManager _stageManager;
    private Transform[] _spawnPoints;
    private int _selectedMonsterIndex = 0;

    private void Start()
    {
        _monsterManager = MonsterManager.Instance;
        _stageManager = FindObjectOfType<StageManager>();
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

    public void SelectMonster(int monsterIndex)
    {
        _selectedMonsterIndex = monsterIndex;
    }
    
    public void SpawnMonster(Vector3 spawnPosition)
    {
        MonsterSO selectedMonsterData = _monsterManager.selectedMonsters[_selectedMonsterIndex];
        string poolTag = selectedMonsterData.poolTag;
        
        GameObject spawnedMonster = PoolManager.Instance.SpawnFromPool(poolTag, spawnPosition, Quaternion.identity);
        if (spawnedMonster != null)
        {
            Monster monster = spawnedMonster.GetComponent<Monster>();
            if (monster != null)
            {
                monster.data = selectedMonsterData;
                monster.SetState(MonsterState.Detecting);
            }
        }
    }
}