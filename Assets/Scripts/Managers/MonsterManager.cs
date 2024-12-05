using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : SingletonBase<MonsterManager>
{
    [SerializeField]
    private MonsterDataManager monsterDataManager;
    
    private Dictionary<int, MonsterSO> _monstersById = new Dictionary<int, MonsterSO>();
    private int _selectedMonsterId;
    public int SelectedMonsterId => _selectedMonsterId;
    
    private void Start()
    {
        LoadMonsterData();
        SelectMonster(1); //for testing
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

    private void SelectMonster(int id)
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