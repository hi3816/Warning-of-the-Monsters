using System.Collections.Generic;

public class MonsterManager : SingletonBase<MonsterManager>
{
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
        if (MonsterDataManager.Instance != null)
        {
            List<MonsterSO> monsters = MonsterDataManager.Instance.LoadMonstersFromAssets();
            foreach (MonsterSO monster in monsters)
            {
                _monstersById[monster.id] = monster;
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
        if (_monstersById.ContainsKey(_selectedMonsterId))
        {
            return _monstersById[_selectedMonsterId];
        }
        return null;
    }
}