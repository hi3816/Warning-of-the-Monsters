using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : SingletonBase<MonsterManager>
{
    [SerializeField]
    private MonsterDataManager monsterDataManager;
    
    private Dictionary<int, MonsterSO> _monstersById = new Dictionary<int, MonsterSO>();
    private int _selectedMonsterId;
    public int SelectedMonsterId => _selectedMonsterId;

    [SerializeField] private PoolManager.PoolConfig[] poolConfigs; // 몬스터 풀
    
    private void Start()
    {
        PoolManager.Instance.AddPoolS(poolConfigs);
        LoadMonsterData();
        SelectMonster(_selectedMonsterId);
    }
    
    private void LoadMonsterData()
    {
        if (monsterDataManager != null)
        {
            MonsterSO[] monsters = monsterDataManager.LoadMonsterData();
            foreach (MonsterSO monster in monsters)
            {
                if (!_monstersById.ContainsKey(monster.id))
                {
                    _monstersById[monster.id] = monster;
                }
            }
        }
    }

    public void SelectMonster(int id)
    {
        if (_monstersById.ContainsKey(id))
        {
            _selectedMonsterId = id;
        }
    }
    
    public MonsterSO GetSelectedMonsterData()
    {
        if (_monstersById.TryGetValue(_selectedMonsterId, out var monsterSo))
        {
            return monsterSo;
        }
        return null;
    }
}