using System.Collections.Generic;
using UnityEngine;

public class DataManager2 : SingletonBase<DataManager2>
{
    public Dictionary<int, (int, string)> SelectedMonsterData;
    private Dictionary<SfxType, float> _individualSfxVolumeDict;
    private Dictionary<string, DataTable.Monster_Data> _minionDataDictionary;
    private MonsterSO[] minionSOs;
    private EvolutionSO[] evolutionSOs;
    private Dictionary<float, SummonSO> summonDataDict;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void SetMinionData()
    {
        List<MonsterSO> minionSOList = new List<MonsterSO>();
        List<DataTable.Monster_Data> minionDataList = DataTable.Monster_Data.GetList();
        _minionDataDictionary = new Dictionary<string, DataTable.Monster_Data>();

        foreach (var minionData in minionDataList)
        {
            MonsterSO minionSO = ScriptableObject.CreateInstance<MonsterSO>();
            minionSO.poolTag = minionData.name;
            minionSO.minFearInflicted = minionData.minFearInflicted;
            minionSO.maxFearInflicted = minionData.minFearInflicted;
            minionSO.cooldown = minionData.cooldown;
            minionSO.humanDetectRange = minionData.humanDetectRange;
            minionSO.humanScaringRange = minionData.humanScaringRange;
            minionSO.walkSpeed = minionData.walkspeed;

            minionSOList.Add(minionSO);
            _minionDataDictionary[minionData.name] = minionData;
        }
        minionSOs = minionSOList.ToArray();
    }

    public DataTable.Stage_Data GetStageByIndex(int idx)
    {
        return DataTable.Stage_Data.Stage_DataList[idx];
    }

    public List<DataTable.Monster_Data> GetMonsterSOs()
    {
        return DataTable.Monster_Data.Monster_DataList;
    }

    public Dictionary<int, (int, string)> GetSelectedMonstersData()
    {
        if (SelectedMonsterData == null)
        {
            Debug.Log("선택된 몬스터 정보를 가져오지 못했습니다.");
        }
        return SelectedMonsterData;
    }

    public Dictionary<SfxType, float> GetIndvidualSfxVolumeDict()
    {
        return _individualSfxVolumeDict;
    }

    public DataTable.Skill_Data GetSkillByIndex(int idx)
    {
        return DataTable.Skill_Data.Skill_DataList[idx];
    }

    public DataTable.Upgrade_Data GetUpgradeData(int monsterId, int upgradeLevel)
    {
        var upgrades = DataTable.Upgrade_Data.GetList();
        foreach (var upgrade in upgrades)
        {
            int baseMonsterId = Mathf.FloorToInt(upgrade.monster_id); //base id (1, 2, etc.)
            int upgradePart = Mathf.RoundToInt((upgrade.monster_id - baseMonsterId) * 10); //upgrade level (1, 2, etc.)
            if (baseMonsterId == monsterId && upgradePart == upgradeLevel)
            {
                return upgrade;
            }
        }
        return null;
    }

    public DataTable.Monster_Data GetMinionData(string minionTag)
    {
        if (_minionDataDictionary.TryGetValue(minionTag, out var minionData))
        {
            return minionData;
        }
        return null;

    }

    // 진화 데이터 확인 (EvolutionType 상관 없을 때)
    public EvolutionSO GetEvolutionSO(int monsterId, int upgradeLevel)
    {
        foreach (var evolution in evolutionSOs)
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
    public EvolutionSO GetEvolutionData(int monsterId, int upgradeLevel, EvolutionType evolutionType)
    {
        foreach (var evolution in evolutionSOs)
        {
            int baseEvolutionId = Mathf.FloorToInt(evolution.evolutionId); // base id (1, 2, etc.)
            int level = evolution.upgradeLevel;
            EvolutionType type = evolution.evolutionType;
            if (baseEvolutionId == monsterId && level == upgradeLevel && type == evolutionType)
            {
                return evolution;
            }
        }
        return null;
    }

    public SummonSO GetSummonData(float monsterId)
    {
        return summonDataDict[monsterId];
    }
}
