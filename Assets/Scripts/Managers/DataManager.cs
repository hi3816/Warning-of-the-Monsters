using System.Collections.Generic;
using UnityEngine;
using DataTable;

public class DataManager : SingletonBase<DataManager>
{
    private List<Human_Data> _humanDataList;
    private Dictionary<int, Wave_Data> _waveDataDictionary;
    private List<Monster_Data> _baseMonsterDataList;
    private List<Upgrade_Data> _upgradeMonsterDataList;
    private List<Monster_Data> _minionDataList;
    private Dictionary<string, Monster_Data> _minionDataDictionary;
    private List<Summon_Data> _summonDataList;
    private Dictionary<float, Summon_Data> _summonDataDictionary;
    private Dictionary<SfxType, float> _individualSfxVolumeDict;
    public Dictionary<int, (int, string)> selectedMonsterData;
    public int selectedStageIdx;

    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        
        LoadHumanData();
        LoadWaveData();
        LoadBaseMonsterData();
        LoadUpgradeMonsterData();
        LoadMinionData();
        LoadSummonData();
    }

    private void LoadHumanData()
    {
        _humanDataList = Human_Data.Human_DataList;
        if (_humanDataList.Count <= 0)
        {
            _humanDataList = Human_Data.GetList();
        }
        if (_humanDataList.Count <= 0)
        {
            Debug.LogAssertion($"Human data not Loaded");
        }
    }
    
    public Human_Data GetHumanByIndex(int idx)
    {
        if (_humanDataList.Count <= 0)
        {
            LoadHumanData();
        }

        for (int i = 0; i < _humanDataList.Count; i++)
        {
            if (_humanDataList[i].id == idx)
                return _humanDataList[i];
        }
        return null;
    }

    private void LoadWaveData()
    {
        _waveDataDictionary = Wave_Data.Wave_DataMap;
        if (_waveDataDictionary.Count <= 0)
        {
            _waveDataDictionary = Wave_Data.GetDictionary();
        }
        if (_waveDataDictionary.Count <= 0)
        {
            Debug.LogAssertion($"Wave data not Loaded");
        }
    }
    
    public Wave_Data GetWaveByIndex(int waveIdx)
    {
        if ((_waveDataDictionary).Count <= 0)
        {
            LoadWaveData();
        }
        if (_waveDataDictionary.TryGetValue(waveIdx, out var waveData))
        {
            return waveData;
        }
        Debug.LogAssertion($"Wave data not Found: {waveIdx}");
        return null;
    }
    
    private void LoadBaseMonsterData()
    {
        _baseMonsterDataList = Monster_Data.GetList();
    }

    private void LoadUpgradeMonsterData()
    {
        _upgradeMonsterDataList = Upgrade_Data.GetList();
    }

    private void LoadMinionData()
    {
        _minionDataList = Monster_Data.GetList();
        _minionDataDictionary = new Dictionary<string, Monster_Data>();

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
            _summonDataDictionary[summonData.monsterId] = summonData;
        }
    }

    public List<Monster_Data> GetBaseMonsters()
    {
        return _baseMonsterDataList;
    }

    public Upgrade_Data GetUpgradeMonsters(int monsterId, int level)
    {
        var upgrades = Upgrade_Data.GetList();
         foreach (var upgrade in upgrades)
         {
             int baseMonsterId = upgrade.monsterId / 1000; //base id (1, 2, etc.)
             int upgradePart = upgrade.monsterId % 1000 / 100; //upgrade level (1, 2, etc.)
             if (baseMonsterId == monsterId && upgradePart == level)
             {
                 return upgrade;
             }
         }
         return null;
    }

    public Monster_Data GetBaseMonsterById(int id)
    {
        return id >= 0 && id < _baseMonsterDataList.Count ? _baseMonsterDataList[id] : null;
    }

    public List<Monster_Data> GetMinions()
    {
        return _minionDataList;
    }

    public Monster_Data GetMinionData(string minionTag)
    {
        return _minionDataDictionary.TryGetValue(minionTag, out var minionData) ? minionData : null;
    }

    public Summon_Data GetSummonData(float monsterId)
    {
        return _summonDataDictionary.TryGetValue(monsterId, out var summonData) ? summonData : null;
    }
    
    
    private void SetIndividualSfxVolumeDict()
    {
        List<SfxVolume_Data> sfxVolumeDataList = SfxVolume_Data.GetList();

        Dictionary<SfxType, float> individualSfxVolumeDict = new Dictionary<SfxType, float>();
        for (int i = 0; i < sfxVolumeDataList.Count; i++)
        {
            individualSfxVolumeDict.Add(sfxVolumeDataList[i].sfxType, sfxVolumeDataList[i].volume);
        }

        _individualSfxVolumeDict = individualSfxVolumeDict;
    }

    public Stage_Data GetStageByIndex(int idx)
    {
        return Stage_Data.GetList()[idx];
    }

    public Skill_Data GetSkillByIndex(int idx)
    {
        return Skill_Data.GetList()[idx];
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
    public Evolution_Data GetEvolutionData(int monsterId, int upgradeLevel)
    {
        foreach (var evolution in Evolution_Data.GetList())
        {
            int baseEvolutionId = evolution.evolutionId / 10000; // base id (1, 2, etc.)
            int level = evolution.upgradeLevel;
            if (baseEvolutionId == monsterId && level == upgradeLevel)
            {
                return evolution;
            }
        }
        return null;
    }

    // 진화 데이터 확인 (EvolutionType 있을 때) -> 진화 버튼 클릭 시 확인용
    public Evolution_Data GetEvolutionData(int monsterId, int upgradeLevel, EvolutionType evolutionType)
    {
        foreach (var evolution in Evolution_Data.GetList())
        {
            int baseEvolutionId = evolution.evolutionId / 10000; // base id (1, 2, etc.)
            int level = evolution.upgradeLevel;
            EvolutionType type = evolution.evolutionType;
            if (baseEvolutionId == monsterId && level == upgradeLevel && type == evolutionType)
            {
                return evolution;
            }
        }
        return null;
    }
   
    public List<Monster_Data> GetMonsterSOs()
    {
        List<Monster_Data> data = Monster_Data.GetList();

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
