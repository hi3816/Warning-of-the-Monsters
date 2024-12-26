using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class MonsterManager : SingletonBase<MonsterManager>
{
    [SerializeField]
    private DataManager4 _monsterDataManager;
    private Dictionary<int, DataTable.Monster_Data> _monstersById = new Dictionary<int, DataTable.Monster_Data>();
    private int _selectedMonsterId;
    public int SelectedMonsterId => _selectedMonsterId;

    [SerializeField] private PoolManager.PoolConfig[] _poolConfigs; // 몬스터 풀
    
    private void Start()
    {
        PoolManager.Instance.AddPoolS(_poolConfigs);
        LoadMonsterData();
        if (!_monstersById.ContainsKey(_selectedMonsterId) && _monstersById.Count > 0)
        {
            _selectedMonsterId = _monstersById.Keys.First();
        }
        
        SelectMonster(_selectedMonsterId);
    }
    
    private void LoadMonsterData()
    {
        if (_monsterDataManager == null) return;

        List<DataTable.Monster_Data> monsters = _monsterDataManager.GetBaseMonsters();
        if (monsters == null || monsters.Count == 0) return;

        foreach(DataTable.Monster_Data monster in monsters)
        {
            if (!_monstersById.ContainsKey(monster.id))
            {
                _monstersById[monster.id] = monster;
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
    
    public DataTable.Monster_Data GetSelectedMonsterData()
    {
        if (_monstersById.TryGetValue(_selectedMonsterId, out var monsterData))
        {
            return monsterData;
        }
        return null;
    }
}