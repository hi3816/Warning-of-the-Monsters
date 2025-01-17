using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public List<Transform> SpawnPointList { get; private set; }
    protected StageManager _stageManager;
    
    private void Start()
    {
        SpawnPointList = StageManager.Instance.SpawnPointList;
        _stageManager = StageManager.Instance;
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
            //print("Spawn point is already occupied by another monster.");
            return;
        }
        
        if (_stageManager.CurrGold >= selectedMonsterData.requiredCoins[0])
        {
            _stageManager.ChangeGold(-selectedMonsterData.requiredCoins[0]);
            Monster monster = PoolManager.Instance.SpawnFromPool<Monster>(selectedMonsterData.name, spawnPosition, Quaternion.identity);            if (monster != null)
            {
                GameManager.Instance.AddActiveList(monster);
                SetMonsterData(monster.data, selectedMonsterData);
                summonerMonster summoner = monster as summonerMonster;
                if (summoner != null) summoner.InitializeSummonableMinions();
                
                // 나중에 big 몬스터인지 small 몬스터인지 판별하는 조건 추가
                SoundManager.Instance.PlaySFX(SfxType.SpawnSmallMonster);
            }
            else
            {
                //print("You do not have enough gold to spawn monster");
            }
        }
    }

    private void SetMonsterData(MonsterData data, DataTable.Monster_Data selectedMonsterData)
    {
        data.id = selectedMonsterData.id;
        data.currentLevel = 0;
        data.poolTag = selectedMonsterData.name;
        data.fatigue = selectedMonsterData.fatigue[data.currentLevel]; 
        data.minFearInflicted = selectedMonsterData.minFearInflicted[data.currentLevel];
        data.maxFearInflicted = selectedMonsterData.maxFearInflicted[data.currentLevel];
        data.cooldown = selectedMonsterData.cooldown[data.currentLevel]; 
        data.humanDetectRange = selectedMonsterData.humanDetectRange[data.currentLevel];
        data.humanScaringRange = selectedMonsterData.humanScaringRange[data.currentLevel]; 
        data.walkSpeed = selectedMonsterData.walkSpeed[data.currentLevel]; 
        data.requiredCoins = selectedMonsterData.requiredCoins[data.currentLevel]; 
        data.maxLevel = selectedMonsterData.maxLevel; 
    }
}