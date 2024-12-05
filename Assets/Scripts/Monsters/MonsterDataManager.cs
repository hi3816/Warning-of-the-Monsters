using System.Collections.Generic;
using UnityEngine;

public class MonsterDataManager : MonoBehaviour
{
    private List<MonsterSO> _monsterSOs;
    
    public MonsterSO[] LoadMonsterData()
    {
        List<Monster_Data.Monster_Data> monsterDataList = Monster_Data.Monster_Data.GetList();

        _monsterSOs = new List<MonsterSO>();
        foreach (var monsterData in monsterDataList)
        {
            MonsterSO monsterSo = ScriptableObject.CreateInstance<MonsterSO>();
            monsterSo.id = monsterData.id;
            monsterSo.poolTag = monsterData.name;
            monsterSo.fatigue = monsterData.fatigue;
            monsterSo.fearInflicted = monsterData.fearInflicted;
            monsterSo.cooldown = monsterData.cooldown;
            monsterSo.humanScaringRange = monsterData.humanScaringRange;
            monsterSo.requiredCoins = monsterData.requiredCoins;

            _monsterSOs.Add(monsterSo);
        }
        return _monsterSOs.ToArray();
    }
}