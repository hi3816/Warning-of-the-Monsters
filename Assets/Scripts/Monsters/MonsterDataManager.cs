using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDataManager : MonoBehaviour
{
    public static MonsterDataManager Instance { get; private set; }
    private Dictionary<string, Monster_Data.Minion_Data> _minionDataDictionary;
    private MonsterSO[] baseMonsterSOs;

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
            baseMonsterSo.fearInflicted = monsterData.fearInflicted;
            baseMonsterSo.cooldown = monsterData.cooldown;
            baseMonsterSo.humanScaringRange = monsterData.humanScaringRange;
            baseMonsterSo.requiredCoins = monsterData.requiredCoins;
            baseMonsterSo.maxLevel = monsterData.maxLevel;
            baseMonsterSOList.Add(baseMonsterSo);
        }

        baseMonsterSOs = baseMonsterSOList.ToArray();
    }

    public MonsterSO[] GetBaseMonsterSOs()
    {
        return baseMonsterSOs;
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

    public Monster_Data.Minion_Data GetMinionData(string minionTag)
    {
        if (_minionDataDictionary.TryGetValue(minionTag, out var minionData))
        {
            return minionData;
        }
        return null;
        
    }

    public Monster_Data.Evolution_Data GetEvolutionData(int monsterId, int upgradeLevel)
    {
        var evolutions = Monster_Data.Evolution_Data.GetList();
        foreach (var evolution in evolutions)
        {
            int baseEvolutionId = Mathf.FloorToInt(evolution.evolution_id); // base id (1, 2, etc.)
            int level = Mathf.RoundToInt((evolution.evolution_id - baseEvolutionId) * 10); // level (1, 2, etc.)
            if (baseEvolutionId == monsterId && level == upgradeLevel)
            {
                return evolution;
            }
        }
        return null;
    }
}
