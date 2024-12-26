using System.Collections.Generic;
using DataTable;

public class DataManager4 : SingletonBase<DataManager4>
{
    public static DataManager4 Instance { get; private set; }
    private List<DataTable.Monster_Data> _baseMonsterDataList;
    private List<DataTable.Upgrade_Data> _upgradeMonsterDataList;
    private List<DataTable.Monster_Data> _minionDataList;
    private Dictionary<string, DataTable.Monster_Data> _minionDataDictionary;
    private List<DataTable.Summon_Data> _summonDataList;
    private Dictionary<float, DataTable.Summon_Data> _summonDataDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        LoadBaseMonsterData();
        LoadUpgradeMonsterData();
        LoadMinionData();
        LoadSummonData();
    }

    private void LoadBaseMonsterData()
    {
        _baseMonsterDataList = DataTable.Monster_Data.GetList();
    }

    private void LoadUpgradeMonsterData()
    {
        _upgradeMonsterDataList = DataTable.Upgrade_Data.GetList();
    }

    private void LoadMinionData()
    {
        _minionDataList = DataTable.Monster_Data.GetList();
        _minionDataDictionary = new Dictionary<string, DataTable.Monster_Data>();

        foreach (var minionData in _minionDataList)
        {
            _minionDataDictionary[minionData.name] = minionData;
        }
    }

    private void LoadSummonData()
    {
        _summonDataList = Summon_Data.GetList();
        _summonDataDictionary = new Dictionary<float, Summon_Data>();

        foreach (var summonData in _summonDataList)
        {
            _summonDataDictionary[summonData.monster_id] = summonData;
        }
    }

    public List<DataTable.Monster_Data> GetBaseMonsters()
    {
        return _baseMonsterDataList;
    }

    public List<DataTable.Upgrade_Data> GetUpgradeMonsters(int monsterId, int level)
    {
        var upgrades = new List<DataTable.Upgrade_Data>();

        foreach (var upgrade in _upgradeMonsterDataList)
        {
            if (upgrade.monster_id == monsterId && upgrade.upgrade_level == level)
            {
                upgrades.Add(upgrade);
            }
        }
        return upgrades;
    }

    public DataTable.Monster_Data GetBaseMonsterById(int id)
    {
        return id >= 0 && id < _baseMonsterDataList.Count ? _baseMonsterDataList[id] : null;
    }

    public List<DataTable.Monster_Data> GetMinions()
    {
        return _minionDataList;
    }

    public DataTable.Monster_Data GetMinionData(string minionTag)
    {
        return _minionDataDictionary.TryGetValue(minionTag, out var minionData) ? minionData : null;
    }

    public Summon_Data GetSummonData(float monsterId)
    {
        return _summonDataDictionary.TryGetValue(monsterId, out var summonData) ? summonData : null;
    }
}