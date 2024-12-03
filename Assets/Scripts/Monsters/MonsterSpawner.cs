using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject currentStage;
    
    private MonsterManager _monsterManager;
    private Transform[] _spawnPoints;
    
    private void Start()
    {
        _monsterManager = MonsterManager.Instance;
        GetSpawnPointsFromCurrentStage();
    }
    
    private void GetSpawnPointsFromCurrentStage()
    {
        if (currentStage != null)
        {
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
        if (_monsterManager.SelectedMonsterId != 0 &&
            _monsterManager.MonstersById.TryGetValue(_monsterManager.SelectedMonsterId,
                out MonsterSO selectedMonsterData))
        {
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
}