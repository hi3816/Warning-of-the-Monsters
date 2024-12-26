using System.Linq;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] protected Transform[] SpawnPoints;
    protected StageManager StageManager;
    private Monster _monster;
    
    private void Start()
    {
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("MonsterSpawnPoint"); //NO FIND...
        SpawnPoints = spawnPointObjects.Select(go => go.transform).ToArray();
        StageManager = StageManager.Instance;
        
        // if (SpawnPoints == null || SpawnPoints.Length == 0) return;
        // StageManager = StageManager.Instance;
    }
    
    protected bool IsSpawnPointOccupied(Vector3 spawnPosition, float checkRadius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, checkRadius, LayerMask.GetMask("Monster"));
        foreach (var collider in colliders)
        {
            if (Vector3.Distance(spawnPosition, collider.transform.position) < checkRadius)
            {
                return true;
            }
        }
        return false;
    }
    
    protected void SpawnMonster(Vector3 spawnPosition, int selectedMonsterId)
    {
        var selectedMonsterData = MonsterManager.Instance.GetSelectedMonsterData();
        if (IsSpawnPointOccupied(spawnPosition, 0.5f))
        {
            print("Spawn point is already occupied by another monster.");
            return;
        }
        
        if (StageManager.CurrGold >= selectedMonsterData.requiredCoins)
        {
            StageManager.ChangeGold(-selectedMonsterData.requiredCoins);
            GameObject monster = PoolManager.Instance.SpawnFromPool(selectedMonsterData.name, spawnPosition, Quaternion.identity);
            if (monster != null)
            {
                monster.transform.position = spawnPosition;
                monster.SetActive(true);

                var monsterComponent = monster.GetComponent<Monster>();
                if (monsterComponent != null)
                {
                    SetMonsterData(monsterComponent.data, selectedMonsterData);
                    monsterComponent.Reset();
                }
                
                // 나중에 big 몬스터인지 small 몬스터인지 판별하는 조건 추가
                SoundManager.Instance.PlaySFX(SfxType.SpawnSmallMonster);
            }
            else
            {
                print("You do not have enough gold to spawn monster");
            }
        }
    }

    private void SetMonsterData(MonsterData data,DataTable.Monster_Data selectedMonsterData)
    {
        data.id = selectedMonsterData.id;
        data.currentLevel = 0;
        data.poolTag = selectedMonsterData.name;
        data.fatigue = selectedMonsterData.fatigue; 
        data.minFearInflicted = selectedMonsterData.minFearInflicted;
        data.maxFearInflicted = selectedMonsterData.maxFearInflicted;
        data.cooldown = selectedMonsterData.cooldown; 
        data.humanScaringRange = selectedMonsterData.humanScaringRange; 
        data.walkSpeed = selectedMonsterData.walkspeed; 
        data.requiredCoins = selectedMonsterData.requiredCoins; 
        data.maxLevel = selectedMonsterData.maxLevel; 
    }
}