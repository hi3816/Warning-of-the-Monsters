using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : SingletonBase<MonsterManager>
{
    [Header("Monster Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    
    [Header("References")]
    [SerializeField] private MonsterSpawner monsterSpawner;
    
    public Dictionary<int, MonsterSO> MonstersById = new Dictionary<int, MonsterSO>();
    private StageManager _stageManager;
    
    private int _selectedMonsterId = 0;
    public int SelectedMonsterId => _selectedMonsterId; // Current monster selected by the player
    
    private void Start()
    {
        _stageManager = StageManager.Instance;
        LoadMonsterData();
    }
    
    private void LoadMonsterData()
    {
        if (MonsterDataManager.Instance != null)
        {
            List<MonsterSO> monsters = MonsterDataManager.Instance.LoadMonstersFromAssets();
            foreach (MonsterSO monster in monsters)
            {
                MonstersById[monster.id] = monster;
            }
        }
    }

    public void SelectMonster(int id)
    {
        if (MonstersById.ContainsKey(id))
        {
            _selectedMonsterId = id;
        }
    }
    
    public void SpawnMonsterAtPosition(Vector3 spawnPosition)
    {
        if (MonstersById.TryGetValue(_selectedMonsterId, out MonsterSO selectedMonsterData))
        {
            if (_stageManager.currGold >= selectedMonsterData.requiredCoins)
            {
                _stageManager.currGold -= selectedMonsterData.requiredCoins;

                if (monsterSpawner != null)
                {
                    monsterSpawner.SpawnMonster(spawnPosition);
                }
            }
            else
            {
                //print "Not enough coins to spawn this monster."
            }
        }
    }
}