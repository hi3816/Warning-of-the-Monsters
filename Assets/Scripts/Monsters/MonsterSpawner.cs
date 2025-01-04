using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public List<Transform> SpawnPointList { get; private set; }
    protected StageManager StageManager;
    private Monster _monster;
    
    private void Start()
    {
        SpawnPointList = StageManager.Instance.SpawnPointList;
        StageManager = StageManager.Instance;
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
            Monster monster = PoolManager.Instance.SpawnFromPool<Monster>(selectedMonsterData.name, spawnPosition, Quaternion.identity);            if (monster != null)
            {
                monster.transform.position = spawnPosition;
                monster.gameObject.SetActive(true);

                SetMonsterData(monster.data, selectedMonsterData);
                monster.Reset();
                
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
        data.walkSpeed = selectedMonsterData.walkSpeed; 
        data.requiredCoins = selectedMonsterData.requiredCoins; 
        data.maxLevel = selectedMonsterData.maxLevel; 
    }
}