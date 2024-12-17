using Monster_Data;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDataManager : MonoBehaviour
{
    public static MonsterDataManager Instance { get; private set; }
    private Dictionary<string, Monster_Data.Monster_Data> _minionDataDictionary;
    private MonsterSO[] baseMonsterSOs;
    private MonsterSO[] minionSOs;
    private SummonSO[] summonSOs;
    private Dictionary<float, SummonSO> summonDataDict; 

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

        SetBaseMonsterSOs();
        SetMinionData();
        SetSummonDataDict();
    }

    private void SetBaseMonsterSOs()
    {
        List<MonsterSO> baseMonsterSOList = new List<MonsterSO>();
        List<Monster_Data.Monster_Data> baseMonsterDataList = Monster_Data.Monster_Data.GetList();

        foreach (var monsterData in baseMonsterDataList)
        {
            MonsterSO baseMonsterSo = ScriptableObject.CreateInstance<MonsterSO>();
            baseMonsterSo.id = monsterData.id;
            baseMonsterSo.upgradeLevel = 0;
            baseMonsterSo.poolTag = monsterData.name;
            baseMonsterSo.fatigue = monsterData.fatigue;
            baseMonsterSo.minFearInflicted = monsterData.minFearInflicted;
            baseMonsterSo.maxFearInflicted = monsterData.maxFearInflicted;
            baseMonsterSo.cooldown = monsterData.cooldown;
            baseMonsterSo.humanDetectRange = monsterData.humanDetectRange;
            baseMonsterSo.humanScaringRange = monsterData.humanScaringRange;
            baseMonsterSo.requiredCoins = monsterData.requiredCoins;
            baseMonsterSo.maxLevel = monsterData.maxLevel;
            baseMonsterSo.walkSpeed = monsterData.walkspeed;
            baseMonsterSo.monsterType = monsterData.MonsterType;
            baseMonsterSOList.Add(baseMonsterSo);
        }

        baseMonsterSOs = baseMonsterSOList.ToArray();
    }

    private void SetMinionData()
    {
        List<MonsterSO> minionSOList = new List<MonsterSO>();
        List<Monster_Data.Monster_Data> minionDataList = Monster_Data.Monster_Data.GetList();
        _minionDataDictionary = new Dictionary<string, Monster_Data.Monster_Data>();

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

    private void SetSummonDataDict()
    {
        List<SummonSO> summonSOList = new List<SummonSO>();
        List<Summon_Data> summonDataList = Summon_Data.GetList();
        summonDataDict = new Dictionary<float, SummonSO>();

        foreach (var summonData in summonDataList)
        {
            SummonSO summonSO = ScriptableObject.CreateInstance<SummonSO>();
            summonSO.monsterID = summonData.monster_id;
            summonSO.evolutionType = summonData.EvolutionType;
            summonSO.minionList = summonData.minionMonsterId    ;
            summonSO.minionTagList = summonData.minionTag;
            summonSO.minionCountList = summonData.count;
            summonSO.evolutionMinionIdList = summonData.evolutionMinionMonsterId;
            summonSO.evolutionMinionTagList = summonData.evolutionMinionTag;
            summonSO.eovlutionMinionCountList = summonData.evolutionCount;

            summonSOList.Add(summonSO);
            summonDataDict[summonSO.monsterID] = summonSO;
        }
        summonSOs = summonSOList.ToArray();
    }

    public MonsterSO[] GetBaseMonsterSOs()
    {
        return baseMonsterSOs;
    }

    public MonsterSO GetBaseMonsterByIndex(int idx)
    {
        return baseMonsterSOs[idx];
    }

    public Monster_Data.Upgrade_Data GetUpgradeData(int monsterId, int upgradeLevel)
    {
        var upgrades = Monster_Data.Upgrade_Data.GetList();
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

    public Monster_Data.Monster_Data GetMinionData(string minionTag)
    {
        if (_minionDataDictionary.TryGetValue(minionTag, out var minionData))
        {
            return minionData;
        }
        return null;
        
    }

    // 진화 데이터 확인 (EvolutionType 상관 없을 때)
    public Monster_Data.Evolution_Data GetEvolutionData(int monsterId, int upgradeLevel)
    {
        var evolutions = Monster_Data.Evolution_Data.GetList();
        foreach (var evolution in evolutions)
        {
            int baseEvolutionId = Mathf.FloorToInt(evolution.evolution_id); // base id (1, 2, etc.)
            int level = evolution.upgrade_level;
            if (baseEvolutionId == monsterId && level == upgradeLevel)
            {
                return evolution;
            }
        }
        return null;
    }

    // 진화 데이터 확인 (EvolutionType 있을 때) -> 진화 버튼 클릭 시 확인용
    public Monster_Data.Evolution_Data GetEvolutionData(int monsterId, int upgradeLevel, EvolutionType evolutionType)
    {
        var evolutions = Monster_Data.Evolution_Data.GetList();
        foreach (var evolution in evolutions)
        {
            int baseEvolutionId = Mathf.FloorToInt(evolution.evolution_id); // base id (1, 2, etc.)
            int level = evolution.upgrade_level;
            EvolutionType type = evolution.EvolutionType;
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
