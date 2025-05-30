using System.Collections.Generic;
using DataTable;
using System.Linq;

public class DataManager : SingletonBase<DataManager>
{
    private List<Human_Data> _humanDataList;
    private Dictionary<int, Wave_Data> _waveDataDictionary;
    private List<Monster_Data> _baseMonsterDataList;
    private Dictionary<int, Monster_Data> _baseMonsterDataDictionary;
    private List<Monster_Data> _minionDataList;
    private Dictionary<string, Monster_Data> _minionDataDictionary;
    private List<Summon_Data> _summonDataList;
    private Dictionary<int, Summon_Data> _summonDataDictionary;
    private List<int> _summonDataDictKeyList;
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
        return null;
    }
    
    private void LoadBaseMonsterData()
    {
        _baseMonsterDataList = Monster_Data.GetList();
        _baseMonsterDataDictionary = new Dictionary<int, Monster_Data>();

        foreach (var baseMonsterData in _baseMonsterDataList)
        {
            _baseMonsterDataDictionary[baseMonsterData.id] = baseMonsterData;
        }
    }

    private void LoadMinionData()
    {
        _minionDataList = Monster_Data.GetList();
        _minionDataDictionary = new Dictionary<string, Monster_Data>();

        foreach (var minionData in _minionDataList)
        {
            if (minionData.monsterType == MonsterType.Minion)
            {
                _minionDataDictionary[minionData.name] = minionData;
            }
        }
    }

    private void LoadSummonData()
    {
        _summonDataList = Summon_Data.GetList();
        _summonDataDictionary = new Dictionary<int, Summon_Data>();

        foreach (var summonData in _summonDataList)
        {
            _summonDataDictionary[summonData.monsterId] = summonData;
        }

        _summonDataDictKeyList = _summonDataDictionary.Keys.ToList<int>();
    }

    public List<Monster_Data> GetBaseMonsters()
    {
        return _baseMonsterDataList;
    }

    public Monster_Data GetBaseMonsterByIdx(int id)
    {
        return id >= 0 && id < _baseMonsterDataList.Count ? _baseMonsterDataList[id] : null;
    }

    public Monster_Data GetBaseMonsterById(int id)
    {
        if (_baseMonsterDataDictionary == null) LoadBaseMonsterData();
        return id >= 0 && _baseMonsterDataDictionary.ContainsKey(id) ? _baseMonsterDataDictionary[id] : null;
    }

    public List<Monster_Data> GetMinions()
    {
        return _minionDataList;
    }

    public Monster_Data GetMinionData(string minionTag)
    {
        return _minionDataDictionary.TryGetValue(minionTag, out var minionData) ? minionData : null;
    }

    public Summon_Data GetSummonData(int monsterId)
    {
        return _summonDataDictionary.TryGetValue(monsterId, out var summonData) ? summonData : null;
    }

    public string[] GetEvolutionMinionNameBySummonerId(int summonerId)
    {
        if (!_summonDataDictionary.ContainsKey(summonerId * 1000)) return null;
        if (!_summonDataDictKeyList.Contains(summonerId * 1000)) return null;

        int index = _summonDataDictKeyList.IndexOf(summonerId * 1000);
        string[] evolutionMinions = new string[2];
        evolutionMinions[0] = _summonDataDictionary[_summonDataDictKeyList[index + 1]].minionTag[0];
        evolutionMinions[1] = _summonDataDictionary[_summonDataDictKeyList[index + 2]].minionTag[0];
        return evolutionMinions;
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
            int baseEvolutionId = evolution.evolutionId / 1000; // base id (1, 2, etc.)
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
            int baseEvolutionId = evolution.evolutionId / 1000; // base id (1, 2, etc.)
            int level = evolution.upgradeLevel;
            EvolutionType type = evolution.evolutionType;
            if (baseEvolutionId == monsterId && level == upgradeLevel && type == evolutionType)
            {
                return evolution;
            }
        }
        return null;
    }

    public List<Projectile_Data> GetProjectileData()
    {
        return Projectile_Data.GetList();
    }
   
    public List<Monster_Data> GetMonsterSOs()
    {
        List<Monster_Data> data = Monster_Data.GetList();

        return data;
    }

    public Dictionary<int, (int, string)> GetSelectedMonstersData()
    {
        return selectedMonsterData;
    }
}
