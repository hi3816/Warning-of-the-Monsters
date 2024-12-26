using System.Collections.Generic;
using UnityEngine;
using DataTable;

public class DataManager : SingletonBase<DataManager>
{
    public static DataManager Instance { get; private set; }
    private List<DataTable.Monster_Data> _baseMonsterDataList;
    private List<DataTable.Upgrade_Data> _upgradeMonsterDataList;
    private List<DataTable.Monster_Data> _minionDataList;
    private Dictionary<string, DataTable.Monster_Data> _minionDataDictionary;
    private List<DataTable.Summon_Data> _summonDataList;
    private Dictionary<float, DataTable.Summon_Data> _summonDataDictionary;
    private Dictionary<SfxType, float> _individualSfxVolumeDict;
    public Dictionary<int, (int, string)> selectedMonsterData;
    public int selectedStageIdx;

    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        
        LoadBaseMonsterData();
        LoadUpgradeMonsterData();
        LoadMinionData();
        LoadSummonData();
    }
    
    public DataTable.Human_Data GetHumanByIndex(int idx)
    {
        if (DataTable.Human_Data.GetList() == null)
        {
            Debug.LogAssertion($"Human data not Loaded");
            return null;
        }
        return DataTable.Human_Data.Human_DataList[idx];
    }
    
    public DataTable.Wave_Data GetWaveByIndex(int waveIdx)
    {
        if (DataTable.Wave_Data.GetList() == null)
        {
            Debug.LogAssertion($"Wave data not Loaded");
            return null;
        }
        if (DataTable.Wave_Data.Wave_DataMap.TryGetValue(waveIdx, out var waveData))
        {
            return waveData;
        }
        Debug.LogAssertion($"Wave data not Found: {waveIdx}");
        return null;
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
    
    
    private void SetIndividualSfxVolumeDict()
    {
        List<DataTable.SfxVolume_Data> sfxVolumeDataList = DataTable.SfxVolume_Data.GetList();

        Dictionary<SfxType, float> individualSfxVolumeDict = new Dictionary<SfxType, float>();
        for (int i = 0; i < sfxVolumeDataList.Count; i++)
        {
            individualSfxVolumeDict.Add(sfxVolumeDataList[i].sfxType, sfxVolumeDataList[i].volume);
        }

        _individualSfxVolumeDict = individualSfxVolumeDict;
    }

    public DataTable.Stage_Data GetStageByIndex(int idx)
    {
        return DataTable.Stage_Data.GetList()[idx];
    }

    public DataTable.Skill_Data GetSkillByIndex(int idx)
    {
        return DataTable.Skill_Data.GetList()[idx];
    }

    public Dictionary<SfxType, float> GetIndvidualSfxVolumeDict()
    {
        if (_individualSfxVolumeDict == null)
        {
            SetIndividualSfxVolumeDict();
        }
        return _individualSfxVolumeDict;
    }

    // 진화 데이터 확인 (EvolutionType 상관 없을 때)
    public DataTable.Evolution_Data GetEvolutionData(int monsterId, int upgradeLevel)
    {
        foreach (var evolution in DataTable.Evolution_Data.GetList())
        {
            int baseEvolutionId = Mathf.FloorToInt(evolution.evolutionId); // base id (1, 2, etc.)
            int level = evolution.upgradeLevel;
            if (baseEvolutionId == monsterId && level == upgradeLevel)
            {
                return evolution;
            }
        }
        return null;
    }

    // 진화 데이터 확인 (EvolutionType 있을 때) -> 진화 버튼 클릭 시 확인용
    public DataTable.Evolution_Data GetEvolutionData(int monsterId, int upgradeLevel, EvolutionType evolutionType)
    {
        foreach (var evolution in DataTable.Evolution_Data.GetList())
        {
            int baseEvolutionId = Mathf.FloorToInt(evolution.evolutionId); // base id (1, 2, etc.)
            int level = evolution.upgradeLevel;
            EvolutionType type = evolution.EvolutionType;
            if (baseEvolutionId == monsterId && level == upgradeLevel && type == evolutionType)
            {
                return evolution;
            }
        }
        return null;
    }
   
    public List<DataTable.Monster_Data> GetMonsterSOs()
    {
        List<DataTable.Monster_Data> data = DataTable.Monster_Data.GetList();

        return data;
    }

    public Dictionary<int, (int, string)> GetSelectedMonstersData()
    {
        if (selectedMonsterData == null)
        {
            Debug.Log("선택된 몬스터 정보를 가져오지 못했습니다.");
        }
        return selectedMonsterData;
    }
}
