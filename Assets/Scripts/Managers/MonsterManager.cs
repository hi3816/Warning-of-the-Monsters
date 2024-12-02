using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : SingletonBase<MonsterManager>
{
    [Header("Monster Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    
    [Header("References")]
    [SerializeField] private MonsterSpawner monsterSpawner;
    
    private List<MonsterSO> Monsters { get; set; } = new List<MonsterSO>();
    private StageManager _stageManager;
    
    private int _selectedMonsterIndex = 0;
    public int SelectedMonsterIndex => _selectedMonsterIndex; // Current monster selected by the player
    
    private void Start()
    {
        LoadMonsterData();
    }
    
    private void LoadMonsterData()
    {
        MonsterDataManager monsterDataManager = FindObjectOfType<MonsterDataManager>(); //DON'T USE FINDDDDDDD
        
        if (monsterDataManager != null)
        {
            Monsters = monsterDataManager.LoadMonstersFromAssets();
        }
    }

    public void SelectMonster(int index)
    { 
        if (index >= 0 && index < Monsters.Count)
        {
            _selectedMonsterIndex = index;
        }
    }
    
    public void SpawnMonster(Vector3 spawnPosition, MonsterSO selectedMonsterData)
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

    // public void SpawnMonsterAtPosition(Vector3 spawnPosition)
    // {
    //     MonsterSO selectedMonsterData = Monsters[_selectedMonsterIndex];
    //
    //     if (_stageManager.currGold >= selectedMonsterData.requiredCoins)
    //     {
    //         _stageManager.currGold -= selectedMonsterData.requiredCoins;
    //     
    //         if (monsterSpawner != null)
    //         {
    //             monsterSpawner.SpawnMonster(spawnPosition, selectedMonsterData);
    //         }
    //     }
    //     else
    //     {
    //         //print "Not enough coins to spawn this monster."
    //     }
    // }
}